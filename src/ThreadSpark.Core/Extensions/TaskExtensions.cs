using System;
using System.Threading.Tasks;

namespace ThreadSpark.Core.Extensions
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Gets the Inner Exception from a task Aggregate Exception.
        /// </summary>
        /// <param name="task">The Task</param>
        /// <returns>Returns first Exception inside the Aggregate Exception.</returns>
        public static Exception GetException(this Task task)
        {
            if (task?.Exception == null) return null;
            return task.Exception.InnerException ?? task.Exception;
        }
    }
}