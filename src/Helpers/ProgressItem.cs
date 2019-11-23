using LanguageExt;

namespace ThreadStrike.Helpers
{
    public class ProgressItem<TResultType>
    {
        public ProgressItem(Try<TResultType> result, int total, int idx, int totalComplete)
        {
            Result = result;
            Total = total;
            Index = idx;
            TotalComplete = totalComplete;
        }
        
        /// <summary>
        /// The result returned by the task.
        /// </summary>
        public Try<TResultType> Result { get; }
        
        /// <summary>
        /// The total number of tasks to be executed.
        /// </summary>
        public int Total { get; }
        
        /// <summary>
        /// The index of the task just completed running.
        /// </summary>
        public int Index { get; }
        
        /// <summary>
        /// The total number of tasks completed.
        /// </summary>
        public int TotalComplete { get; }
    }
}