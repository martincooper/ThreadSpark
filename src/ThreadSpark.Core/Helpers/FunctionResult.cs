using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ThreadSpark.Core.Helpers
{
    /// <summary>
    /// FunctionResult class.
    /// Result of running a function, either the returned value, or an exception.
    /// </summary>
    /// <typeparam name="TResultType">The result type of the function.</typeparam>
    internal class FunctionResult<TResultType>
    {
        /// <summary>
        /// Generates the Function Result as an exception.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        public FunctionResult(string errorMessage)
        {
            Result = Try<TResultType>(new Exception(errorMessage));
        }
        
        public FunctionResult(Try<TResultType> result, int idx)
        {
            Idx = idx;
            Result = result;
        }
        
        /// <summary>
        /// The index of the function in the processing queue.
        /// This should always be the same as the order the
        /// collection of functions were received. 
        /// </summary>
        public int Idx { get; }
        
        /// <summary>
        /// The function result.
        /// </summary>
        public Try<TResultType> Result { get; }
    }
}