using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ThreadSpark.Core.Helpers
{
    /// <summary>
    /// FunctionItem class.
    /// Allows a clean way to bind a numeric index to a function to allow
    /// better visibility on outputting progress updates.
    /// </summary>
    /// <typeparam name="TResultType">The result type of the function.</typeparam>
    internal class FunctionResult<TResultType>
    {
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