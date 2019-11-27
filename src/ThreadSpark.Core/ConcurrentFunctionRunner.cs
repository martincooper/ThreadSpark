using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using ThreadSpark.Core.Extensions;
using ThreadSpark.Core.Helpers;
using static LanguageExt.Prelude;

namespace ThreadSpark.Core
{
    public class ConcurrentFunctionRunner
    {
        private readonly SemaphoreSlim _semaphore;
        
        public ConcurrentFunctionRunner(int maxThreads)
        {
            if (maxThreads <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxThreads), "Max threads cannot be less than 1.");
            
            _semaphore = new SemaphoreSlim(maxThreads);
            
            MaxThreads = maxThreads;
        }
        
        public int MaxThreads { get; }
        
        /// <summary>
        /// Runs all the supplied functions concurrently, limited to the specified Max Threads.
        /// Returns a RunnerResult object which can be used to wait for all tasks
        /// to be finished before checking the results.
        /// Runs all, any errors returned in the Try Result.
        /// </summary>
        /// <param name="funcs">The functions to run concurrently.</param>
        /// <param name="settings">Settings if required.</param>
        /// <typeparam name="TResultType">The functions result type.</typeparam>
        /// <returns>Returns a collection of results.</returns>
        public RunnerResult<Seq<Try<TResultType>>> BeginRun<TResultType>(
            IEnumerable<Func<TResultType>> funcs,
            ConcurrentFunctionSettings<TResultType> settings = null)
        {
            return new RunnerResult<Seq<Try<TResultType>>>(Task.Run(() => Run(funcs, settings)));
        }

        /// <summary>
        /// Runs all the supplied functions concurrently, limited to the specified Max Threads.
        /// Blocks the executing thread until finished, then returns a
        /// collection of Try results, one for each request.
        /// </summary>
        /// <param name="funcs">The functions to run concurrently.</param>
        /// <param name="settings">Settings if required.</param>
        /// <typeparam name="TResultType">The functions result type.</typeparam>
        /// <returns>Returns a collection of results.</returns>
        public Seq<Try<TResultType>> Run<TResultType>(
            IEnumerable<Func<TResultType>> funcs,
            ConcurrentFunctionSettings<TResultType> settings = null)
        {
            var tasks = Execute(funcs, false, settings);
            return ProcessAllTasks(tasks);
        }

        /// <summary>
        /// Runs all the supplied functions concurrently, limited to the specified Max Threads.
        /// Returns a RunnerResult object which can be used to wait for all tasks
        /// to be finished before checking the results.
        /// Runs until the first error and aborts, else returns all results.
        /// </summary>
        /// <param name="funcs">The functions to run concurrently.</param>
        /// <param name="settings">Settings if required.</param>
        /// <typeparam name="TResultType">The functions result type.</typeparam>
        /// <returns>Returns a collection of results.</returns>
        public RunnerResult<Try<Seq<TResultType>>> BeginRunUntilError<TResultType>(
            IEnumerable<Func<TResultType>> funcs,
            ConcurrentFunctionSettings<TResultType> settings = null)
        {
            return new RunnerResult<Try<Seq<TResultType>>>(Task.Run(() => RunUntilError(funcs, settings)));
        }
        
        /// <summary>
        /// Runs all the supplied functions concurrently limited to the specified Max Threads.
        /// Blocks the executing thread until finished.
        /// Will run until a task fails with an exception, and will return early, not running remaining tasks.
        /// </summary>
        /// <param name="funcs">The functions to run concurrently.</param>
        /// <param name="settings">Settings if required.</param>
        /// <typeparam name="TResultType">The functions result type.</typeparam>
        /// <returns>Returns a collection of results if all succeed, else first failing exception.</returns>
        public Try<Seq<TResultType>> RunUntilError<TResultType>(
            IEnumerable<Func<TResultType>> funcs,
            ConcurrentFunctionSettings<TResultType> settings = null)
        {
            var tasks = Execute(funcs, true, settings);
            return ProcessTasksAllOrFail(tasks);
        }
        
        private Seq<FunctionResult<TResultType>> Execute<TResultType>(
            IEnumerable<Func<TResultType>> funcs,
            bool failOnFirstError,
            ConcurrentFunctionSettings<TResultType> settings = null)
        {
            var functionRequests = funcs.Select((func, idx) => new FunctionRequest<TResultType>(func, idx)).ToArray(); 
            var funcQueue = new ConcurrentQueue<FunctionRequest<TResultType>>(functionRequests);
            var tokenManager = new CancellationTokenManager(settings?.CancellationToken, failOnFirstError);
            var progressManager = new ProgressManager<TResultType>(settings?.Progress, functionRequests.Length);
            
            var results = functionRequests
                .Select(func => ExecuteFuncWhenReady(funcQueue, tokenManager, progressManager))
                .ToArray();

            // Ensure that we wait until all the running tasks are complete so the
            // Semaphore Slim doesn't get disposed before it's finished running and we exit.
            Task.WaitAll(results.ToArray<Task>());
            
            return results.Map(c => c.Result).ToSeq();
        }

        private Task<FunctionResult<TResultType>> ExecuteFuncWhenReady<TResultType>(
            ConcurrentQueue<FunctionRequest<TResultType>> funcQueue,
            CancellationTokenManager tokenManager,
            ProgressManager<TResultType> progressManager)
        {
            // Wait so we limit the number of concurrent threads.
            _semaphore.Wait();
            
            return Task
                .Run(() => ExecuteFunc(funcQueue, tokenManager, progressManager))
                .ContinueWith(task => { _semaphore.Release(); return task.Result; });
        }
        
        private static FunctionResult<TResultType> ExecuteFunc<TResultType>(
            ConcurrentQueue<FunctionRequest<TResultType>> funcQueue,
            CancellationTokenManager tokenManager,
            ProgressManager<TResultType> progressManager)
        {
            // Get the functions to run from a queue so they are started in the order received.
            if (!funcQueue.TryDequeue(out var funcRequest))
                return new FunctionResult<TResultType>("Concurrent Function Error. No items found in queue.");

            var result = tokenManager.IsCancellationRequested
                ? Try<TResultType>(new TaskCanceledException())
                : Try(() => funcRequest.Func()).Strict();

            // On error, signal cancellation of remaining tasks if required.
            tokenManager.SetCancelIfError(result);
            
            // Report task completion to caller.
            progressManager.Report(result, funcRequest.Idx);

            return new FunctionResult<TResultType>(result, funcRequest.Idx);
        }
        
        private static Seq<Try<TResultType>> ProcessAllTasks<TResultType>(IEnumerable<FunctionResult<TResultType>> taskResults)
        {
            return taskResults
                .OrderBy(_ => _.Idx)
                .Select(_ => _.Result)
                .ToSeq();
        }
        
        private static Try<Seq<TResultType>> ProcessTasksAllOrFail<TResultType>(Seq<FunctionResult<TResultType>> taskResults)
        {
            return ProcessAllTasks(taskResults)
                .AllOrFirstFail();
        }
    }
}