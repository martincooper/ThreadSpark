using System;
using System.Threading;

namespace ThreadSpark.Helpers
{
    /// <summary>
    /// Settings for Concurrent Function Runner.
    /// </summary>
    /// <typeparam name="TResultType"></typeparam>
    public class ConcurrentFunctionSettings<TResultType>
    {
        /// <summary>
        /// Set to get progress notifications on completed tasks.
        /// </summary>
        public IProgress<ProgressItem<TResultType>> Progress { get; set; }
        
        /// <summary>
        /// Set to abort un-executed tasks if required.
        /// </summary>
        public CancellationToken? CancellationToken { get; set; }
    }
}