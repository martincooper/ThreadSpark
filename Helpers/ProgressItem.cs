namespace ThreadStrike.Helpers
{
    public class ProgressItem<TResultType>
    {
        public ProgressItem(TResultType result, int idx)
        {
            Index = idx;
            Result = result;
        }
        
        /// <summary>
        /// The total number of functions to be executed.
        /// </summary>
        public int Total { get; }
        
        /// <summary>
        /// The index of the function just completed running.
        /// </summary>
        public int Index { get; }
        
        /// <summary>
        /// The returned result.
        /// </summary>
        public TResultType Result { get; }
    }
}