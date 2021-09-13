using System;
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
        public RunnerResult<Try<TResultType>[]> BeginRun<TResultType>(
            IEnumerable<Func<TResultType>> funcs,
            ConcurrentFunctionSettings<TResultType> settings = null) =>
                new RunnerResult<Try<TResultType>[]>(Task.Run(() => Run(funcs, settings)));

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
        public RunnerResult<Try<TResultType>[]> BeginRun<TResultType>(
            IEnumerable<Func<Try<TResultType>>> funcs,
            ConcurrentFunctionSettings<TResultType> settings = null) =>
                new RunnerResult<Try<TResultType>[]>(Task.Run(() => Run(funcs, settings)));

        /// <summary>
        /// Runs all the supplied functions concurrently, limited to the specified Max Threads.
        /// Blocks the executing thread until finished, then returns a
        /// collection of Try results, one for each request.
        /// </summary>
        /// <param name="funcs">The functions to run concurrently.</param>
        /// <param name="settings">Settings if required.</param>
        /// <typeparam name="TResultType">The functions result type.</typeparam>
        /// <returns>Returns a collection of results.</returns>
        public Try<TResultType>[] Run<TResultType>(
            IEnumerable<Func<TResultType>> funcs,
            ConcurrentFunctionSettings<TResultType> settings = null)
        {
            var tryFuncs = funcs.Select(f => fun(() => Try(() => f())));
            return Run(tryFuncs);
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
        public Try<TResultType>[] Run<TResultType>(
            IEnumerable<Func<Try<TResultType>>> funcs,
            ConcurrentFunctionSettings<TResultType> settings = null)
        {
            var tasks = Execute(funcs, false, settings);
            return ProcessAllTasks(tasks).ToArray();
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
        public RunnerResult<Try<TResultType[]>> BeginRunUntilError<TResultType>(
            IEnumerable<Func<TResultType>> funcs,
            ConcurrentFunctionSettings<TResultType> settings = null) =>
                new RunnerResult<Try<TResultType[]>>(Task.Run(() => RunUntilError(funcs, settings)));

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
        public RunnerResult<Try<TResultType[]>> BeginRunUntilError<TResultType>(
            IEnumerable<Func<Try<TResultType>>> funcs,
            ConcurrentFunctionSettings<TResultType> settings = null) =>
                new RunnerResult<Try<TResultType[]>>(Task.Run(() => RunUntilError(funcs, settings)));

        /// <summary>
        /// Runs all the supplied functions concurrently limited to the specified Max Threads.
        /// Blocks the executing thread until finished.
        /// Will run until a task fails with an exception, and will return early, not running remaining tasks.
        /// </summary>
        /// <param name="funcs">The functions to run concurrently.</param>
        /// <param name="settings">Settings if required.</param>
        /// <typeparam name="TResultType">The functions result type.</typeparam>
        /// <returns>Returns a collection of results if all succeed, else first failing exception.</returns>
        public Try<TResultType[]> RunUntilError<TResultType>(
            IEnumerable<Func<TResultType>> funcs,
            ConcurrentFunctionSettings<TResultType> settings = null)
        {
            var tryFuncs = funcs.Select(f => fun(() => Try(() => f())));
            return RunUntilError(tryFuncs);
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
        public Try<TResultType[]> RunUntilError<TResultType>(
            IEnumerable<Func<Try<TResultType>>> funcs,
            ConcurrentFunctionSettings<TResultType> settings = null)
        {
            var tasks = Execute(funcs, true, settings);
            return ProcessTasksAllOrFail(tasks).Map(_ => _.ToArray());
        }

        private Seq<FunctionResult<TResultType>> Execute<TResultType>(
            IEnumerable<Func<Try<TResultType>>> funcs,
            bool failOnFirstError,
            ConcurrentFunctionSettings<TResultType> settings = null)
        {
            var functionRequests = funcs.Select((func, idx) => new FunctionRequest<TResultType>(func, idx)).ToArray(); 
            var tokenManager = new CancellationTokenManager(settings?.CancellationToken, failOnFirstError);
            var progressManager = new ProgressManager<TResultType>(settings?.Progress, functionRequests.Length);
            
            var results = functionRequests
                .Select(func => ExecuteFuncWhenReady(func, tokenManager, progressManager))
                .ToArray();

            // Ensure that we wait until all the running tasks are complete so the
            // Semaphore Slim doesn't get disposed before it's finished running and we exit.
            Task.WaitAll(results.ToArray<Task>());
            
            return results.Map(c => c.Result).ToSeq();
        }

        private Task<FunctionResult<TResultType>> ExecuteFuncWhenReady<TResultType>(
            FunctionRequest<TResultType> funcRequest,
            CancellationTokenManager tokenManager,
            ProgressManager<TResultType> progressManager)
        {
            // Wait so we limit the number of concurrent threads.
            _semaphore.Wait();
            
            return Task
                .Run(() => ExecuteFunc(funcRequest, tokenManager, progressManager))
                .ContinueWith(task => { _semaphore.Release(); return task.Result; });
        }
        
        private static FunctionResult<TResultType> ExecuteFunc<TResultType>(
            FunctionRequest<TResultType> funcRequest,
            CancellationTokenManager tokenManager,
            ProgressManager<TResultType> progressManager)
        {
            var result = tokenManager.IsCancellationRequested
                ? Try<TResultType>(new TaskCanceledException())
                : funcRequest.Func().Strict();

            // On error, signal cancellation of remaining tasks if required.
            tokenManager.SetCancelIfError(result);
            
            // Report task completion to caller.
            progressManager.Report(result, funcRequest.Idx);

            return new FunctionResult<TResultType>(result, funcRequest.Idx);
        }

        private static IEnumerable<Try<TResultType>> ProcessAllTasks<TResultType>(IEnumerable<FunctionResult<TResultType>> taskResults) =>
            taskResults
                .OrderBy(_ => _.Idx)
                .Select(_ => _.Result);
        
        private static Try<IEnumerable<TResultType>> ProcessTasksAllOrFail<TResultType>(IEnumerable<FunctionResult<TResultType>> taskResults) =>
            ProcessAllTasks(taskResults)
                .AllOrFirstFail();
    }
}