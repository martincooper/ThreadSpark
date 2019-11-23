using System.Threading.Tasks;

namespace ThreadStrike.Helpers
{
    public class RunnerResult<TResultType>
    {
        public RunnerResult(Task<TResultType> task)
        {
            Task = task;
        }
        
        /// <summary>
        /// The parent task, if required for awaiting etc.
        /// </summary>
        public Task<TResultType> Task { get; }

        /// <summary>
        /// Blocking call to get the unwrapped task result when complete.
        /// </summary>
        public TResultType Result => Task.Result;

        /// <summary>
        /// Has the Task completed.
        /// </summary>
        public bool IsCompleted => Task.IsCompleted;
    }
}