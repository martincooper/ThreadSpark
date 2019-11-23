using System;
using System.Threading;
using LanguageExt;

namespace ThreadSpark.Core.Helpers
{
    /// <summary>
    /// ProgressManager class.
    /// Manager the thread safe counter of tasks completed and notification of progress back to the caller.
    /// </summary>
    /// <typeparam name="TResultType">The result type.</typeparam>
    internal sealed class ProgressManager<TResultType>
    {
        private int _progressCounter = 0;
        private readonly int _taskCount;
        private readonly IProgress<ProgressItem<TResultType>> _progressReporter;
        
        /// <summary>
        /// Creates a new instance of the ProgressManager class.
        /// </summary>
        /// <param name="progressReporter">The IProgress interface optionally passed in by the caller.</param>
        /// <param name="taskCount">The total number of tasks to be completed.</param>
        public ProgressManager(IProgress<ProgressItem<TResultType>> progressReporter, int taskCount)
        {
            _taskCount = taskCount;
            _progressReporter = progressReporter ?? new Progress<ProgressItem<TResultType>>();
        }
        
        /// <summary>
        /// Reports to any listeners the current progress.
        /// </summary>
        /// <param name="result">The result of the calculated task.</param>
        /// <param name="idx">The index of the completed task.</param>
        public void Report(Try<TResultType> result, int idx)
        {
            var totalComplete = Interlocked.Increment(ref _progressCounter);
            _progressReporter.Report(new ProgressItem<TResultType>(result, _taskCount, idx, totalComplete));
        }
    }
}