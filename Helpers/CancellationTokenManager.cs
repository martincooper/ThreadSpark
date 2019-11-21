using System.Threading;
using LanguageExt;

namespace ThreadStrike.Helpers
{
    /// <summary>
    /// CancellationTokenManager class.
    /// Used to allow failed tasks to be able to cancel remaining pending tasks from running if required.
    /// </summary>
    public class CancellationTokenManager
    {
        public CancellationTokenManager(bool cancelOnError = false)
        {
            Source = new CancellationTokenSource();
            CancelOnError = cancelOnError;
        }

        /// <summary>
        /// Should subsequent tasks be cancelled if an error occurs.
        /// </summary>
        public bool CancelOnError { get; } 
        
        /// <summary>
        /// Underlying Cancellation Token Source.
        /// </summary>
        public CancellationTokenSource Source { get; }

        /// <summary>
        /// Is cancellation requested.
        /// </summary>
        public bool IsCancellationRequested => CancelOnError && Source.IsCancellationRequested;

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