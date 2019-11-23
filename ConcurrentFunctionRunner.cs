using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using ThreadStrike.Extensions;
using static LanguageExt.Prelude;
using ThreadStrike.Helpers;

namespace ThreadStrike
{
    public class ConcurrentFunctionRunner
    {
        public ConcurrentFunctionRunner(int maxThreads)
        {
            if (maxThreads <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxThreads), "Max threads cannot be less than 1.");
            
            MaxThreads = maxThreads;
        }
        
        public int MaxThreads { get; }
        
        public RunnerResult<Try<TResultType>[]> BeginRun<TResultType>(
            IEnumerable<Func<TResultType>> funcs,
            IProgress<ProgressItem<TResultType>> progress = null)
        {
            return new RunnerResult<Try<TResultType>[]>(Task.Run(() => Run(funcs, progress)));
        }

        /// <summary>
        /// Runs all the supplied functions concurrently limited to the specified Max Threads.
        /// Returns a collection of Try results, one for each request.
        /// </summary>
        /// <param name="funcs">The functions to run concurrently.</param>
        /// <param name="progress">IProgress implementation if required.</param>
        /// <typeparam name="TResultType">The functions result type.</typeparam>
        /// <returns>Returns a collection of results.</returns>
        public Try<TResultType>[] Run<TResultType>(
            IEnumerable<Func<TResultType>> funcs,
            IProgress<ProgressItem<TResultType>> progress = null)
        {
            var tasks = Execute(funcs, false, progress);
            return ProcessAllTasks(tasks);
        }

        public RunnerResult<Try<TResultType[]>> BeginRunUntilError<TResultType>(
            IEnumerable<Func<TResultType>> funcs,
            IProgress<ProgressItem<TResultType>> progress = null)
        {
            return new RunnerResult<Try<TResultType[]>>(Task.Run(() => RunUntilError(funcs, progress)));
        }
        
        /// <summary>
        /// Runs all the supplied functions concurrently limited to the specified Max Threads.
        /// Will run until a task fails with an exception, and will return early, not running remaining functions.
        /// </summary>
        /// <param name="funcs">The functions to run concurrently.</param>
        /// <param name="progress">IProgress implementation if required.</param>
        /// <typeparam name="TResultType">The functions result type.</typeparam>
        /// <returns>Returns a collection of results if all succeed, else first failing exception.</returns>
        public Try<TResultType[]> RunUntilError<TResultType>(
            IEnumerable<Func<TResultType>> funcs,
            IProgress<ProgressItem<TResultType>> progress = null)
        {
            var tasks = Execute(funcs, true, progress);
            return ProcessTasksAllOrFail(tasks);
        }
        
        private (int Idx, Try<TResultType> Task)[] Execute<TResultType>(
            IEnumerable<Func<TResultType>> funcs,
            bool failOnFirstError,
            IProgress<ProgressItem<TResultType>> progress = null)
        {
            var allFuncs = funcs.Select((func, idx) => new FunctionItem<TResultType>(func, idx)).ToArray(); 
            var funcQueue = new ConcurrentQueue<FunctionItem<TResultType>>(allFuncs);
            var tokenManager = new CancellationTokenManager(failOnFirstError);
            var progressManager = new ProgressManager<TResultType>(progress, allFuncs.Length);
            
            using (var semaphore = new SemaphoreSlim(MaxThreads))
            {
                var results = allFuncs
                    .Select((func, idx) => (Idx: idx, Task: Task.Run(() => ExecuteFunc(funcQueue, tokenManager, progressManager, semaphore))))
                    .ToArray();

                // Ensure that we wait until all the running tasks are complete so the
                // Semaphore Slim doesn't get disposed before it's finished running and we exit.
                var runningTasks = results.Select(_ => _.Task).ToArray<Task>();
                Task.WaitAll(runningTasks);

                return results.Select(_ => (_.Idx, _.Task.Result)).ToArray();
            }
        }
        
        private static Try<TResultType>[] ProcessAllTasks<TResultType>((int Idx, Try<TResultType> Result)[] taskResults)
        {
            return taskResults
                .OrderBy(_ => _.Idx)
                .Select(_ => _.Result)
                .ToArray();
        }
        
        private static Try<TResultType[]> ProcessTasksAllOrFail<TResultType>((int Idx, Try<TResultType> Result)[] taskResults)
        {
            var faultedTask = taskResults
                .Select(_ => _.Result)
                .FirstOrDefault(_ => _.IsFail());
            
            if (faultedTask != null)
                return Try<TResultType[]>(faultedTask.GetException());

            return Try(taskResults
                .OrderBy(_ => _.Idx)
                .Select(_ => _.Result.GetValue())
                .ToArray());
        }
        
        private static Try<TResultType> ExecuteFunc<TResultType>(
            ConcurrentQueue<FunctionItem<TResultType>> funcQueue,
            CancellationTokenManager tokenManager,
            ProgressManager<TResultType> progressManager,
            SemaphoreSlim semaphore)
        {
            try
            {
                // Wait so we limit the number of concurrent threads.
                semaphore.Wait();

                // Get the functions to run from a queue so they are started in the order received.
                if (!funcQueue.TryDequeue(out var funcItem))
                    return Try<TResultType>(new Exception("Concurrent Function Error. No items found in queue."));

                var result = tokenManager.IsCancellationRequested
                    ? Try<TResultType>(new TaskCanceledException())
                    : Try(() => funcItem.Func()).Strict();

                // On error, signal cancellation of remaining tasks if required.
                tokenManager.SetCancelIfError(result);
                
                // Report task completion to caller.
                progressManager.Report(result, funcItem.Idx);

                return result;
            }
            finally { semaphore.Release(); }
        }
    }
}