using System;

namespace ThreadSpark.Helpers
{
    /// <summary>
    /// FunctionItem class.
    /// Allows a clean way to bind a numeric index to a function to allow
    /// better visibility on outputting progress updates.
    /// </summary>
    /// <typeparam name="TResultType">The result type of the function.</typeparam>
    internal class FunctionItem<TResultType>
    {
        public FunctionItem(Func<TResultType> func, int idx)
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
        public Func<TResultType> Func { get; }
    }
}