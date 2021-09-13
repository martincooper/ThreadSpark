using System;
using LanguageExt;

namespace ThreadSpark.Core.Helpers
{
    /// <summary>
    /// FunctionRequest class.
    /// An un-executed function. Used to bind a numeric index to
    /// a function to allow better visibility on outputting progress
    /// updates and correctly ordering returned results.
    /// </summary>
    /// <typeparam name="TResultType">The result type of the function.</typeparam>
    internal class FunctionRequest<TResultType>
    {
        public FunctionRequest(Func<Try<TResultType>> func, int idx)
        {
            Idx = idx;
            Func = func;
        }
        
        /// <summary>
        /// The index of the function in the processing queue.
        /// This should always be the same as the order the
        /// collection of functions were received. 
        /// </summary>
        public int Idx { get; }
        
        /// <summary>
        /// The function to run.
        /// </summary>
        public Func<Try<TResultType>> Func { get; }
    }
}