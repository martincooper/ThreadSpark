using System.Threading;
using LanguageExt;

namespace ThreadSpark.Core.Helpers
{
    /// <summary>
    /// CancellationTokenManager class.
    /// Used to allow failed tasks to be able to cancel remaining pending tasks from running if required.
    /// </summary>
    public class CancellationTokenManager
    {
        public CancellationTokenManager(CancellationToken? externalCancellationToken, bool cancelOnError = false)
        {
            ExternalCancellationToken = externalCancellationToken;
            Source = new CancellationTokenSource();
            CancelOnError = cancelOnError;
        }

        /// <summary>
        /// Should subsequent tasks be cancelled if an error occurs.
        /// </summary>
        public bool CancelOnError { get; } 
        
        /// <summary>
        /// Cancellation Token Source to abort tasks when first fails.
        /// </summary>
        public CancellationTokenSource Source { get; }

        /// <summary>
        /// External Cancellation Token so tasks can be aborted by caller.
        /// </summary>
        public CancellationToken? ExternalCancellationToken { get; }

        /// <summary>
        /// Is cancellation requested.
        /// </summary>
        public bool IsCancellationRequested
        {
            get
            {
                // Return true if use has aborted.
                if (ExternalCancellationToken.HasValue && ExternalCancellationToken.Value.IsCancellationRequested)
                    return true;
                
                // Return true if an error was thrown and we want to abort subsequent tasks.
                return CancelOnError && Source.IsCancellationRequested;
            }
        }

        /// <summary>
        /// Signals the cancellation if the passed result is a Failure.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <typeparam name="TType">The result type.</typeparam>
        public void SetCancelIfError<TType>(Try<TType> result)
        {
            if (!CancelOnError) return;
            
            if (result.IsFail() && !Source.IsCancellationRequested)
                Source.Cancel();
        }
    }
}