using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using ThreadStrike.Helpers;

namespace ThreadStrike
{
    public class ConcurrentFunctionRunner
    {
        public ConcurrentFunctionRunner(int maxThreads)
        {
            MaxThreads = maxThreads;
        }
        
        public int MaxThreads { get; }
        
        /// <summary>
        /// Runs all the supplied functions concurrently limited to the specified Max Threads.
        /// Returns a collection of Try results, one for each request.
        /// </summary>
        /// <param name="funcs">The functions to run concurrently.</param>
        /// <typeparam name="TReturnType">The functions return type.</typeparam>
        /// <returns>Returns a collection of results.</returns>
        public Try<TReturnType>[] Run<TReturnType>(IEnumerable<Func<TReturnType>> funcs)
        {
            var tasks = Execute(funcs, false);
            return ProcessAllTasks(tasks);
        }
        
        /// <summary>
        /// Runs all the supplied functions concurrently limited to the specified Max Threads.
        /// Will run until one fails with an exception, and will return early, not running remaining functions.
        /// </summary>
        /// <param name="funcs">The functions to run concurrently.</param>
        /// <typeparam name="TReturnType">The functions return type.</typeparam>
        /// <returns>Returns a collection of results if all succeed, else first failing exception.</returns>
        public Try<TReturnType[]> RunUntilFirstFail<TReturnType>(IEnumerable<Func<TReturnType>> funcs)
        {
            var tasks = Execute(funcs, true);
            return ProcessTasksAllOrFail(tasks);
        }
        
        private (int Idx, Try<TReturnType> Task)[] Execute<TReturnType>(
            IEnumerable<Func<TReturnType>> funcs,
            bool failOnFirstError)
        {
            var allFuncs = funcs.ToArray();
            var funcQueue = new ConcurrentQueue<Func<TReturnType>>(allFuncs);
            var tokenManager = new CancellationTokenManager(failOnFirstError);
            
            using (var semaphore = new SemaphoreSlim(MaxThreads))
            {
                var results = allFuncs
                    .Select((func, idx) => (Idx: idx, Task: Task.Run(() => ExecuteFunc(funcQueue, tokenManager, semaphore))))
                    .ToArray();

                // Ensure that we wait until all the running tasks are complete so the
                // Semaphore Slim doesn't get disposed before it's finished running and we exit.
                var runningTasks = results.Select(_ => _.Task).ToArray<Task>();
                Task.WaitAll(runningTasks);

                return results.Select(_ => (_.Idx,  _.Task.Result)).ToArray();
            }
        }
        
        private static Try<TReturnType>[] ProcessAllTasks<TReturnType>((int Idx, Try<TReturnType> Result)[] taskResults)
        {
            return taskResults
                .OrderBy(_ => _.Idx)
                .Select(_ => _.Result)
                .ToArray();
        }
        
        private static Try<TReturnType[]> ProcessTasksAllOrFail<TReturnType>((int Idx, Try<TReturnType> Result)[] taskResults)
        {
            var faultedTask = taskResults
                .Select(_ => _.Result)
                .FirstOrDefault(_ => _.IsFail());
            
            if (faultedTask != null)
                return Try<TReturnType[]>(faultedTask.GetException());

            return Try(taskResults
                .OrderBy(_ => _.Idx)
                .Select(_ => _.Result.GetValue())
                .ToArray());
        }
        
        private static Try<TReturnType> ExecuteFunc<TReturnType>(
            ConcurrentQueue<Func<TReturnType>> funcQueue,
            CancellationTokenManager tokenManager,
            SemaphoreSlim semaphore)
        {
            try
            {
                // Wait so we limit the number of concurrent threads.
                semaphore.Wait();

                if (tokenManager.IsCancellationRequested)
                    Try<TReturnType>(new TaskCanceledException());
                
                // Get the functions to run from a queue so they are started in the order received.
                if (funcQueue.TryDequeue(out var func))
                {
                    // Run the function wrapped in a Try, which ensures we catch
                    // any exceptions ourselves and the Task will never be faulted.
                    var result = Try(() => func()).Strict();
                    
                    // If the task threw an exception, if required,
                    // set the cancellation token to cancel any pending tasks. 
                    tokenManager.SetCancelIfError(result);
                    
                    return result;
                }

                return Try<TReturnType>(new Exception("Concurrent Function Error. No items found in queue."));
            }
            finally { semaphore.Release(); }
        }
    }
}