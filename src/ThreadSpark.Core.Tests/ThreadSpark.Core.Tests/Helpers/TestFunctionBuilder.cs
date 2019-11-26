using System;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadSpark.Core.Tests.Helpers
{
    /// <summary>
    /// TestFunctionBuilder.
    /// Builds functions to use in the testing of the concurrent runners.
    /// </summary>
    public static class TestFunctionBuilder
    {
        private static readonly Random Rnd = new Random();

        /// <summary>
        /// Create a simple simulated unit of work to be used in testing.
        /// </summary>
        /// <param name="idx">The index of the created task.</param>
        /// <returns>Returns the function.</returns>
        public static Func<int> Create(int idx)
        {
            return Create(idx, idx * 5);
        }
        
        /// <summary>
        /// Create a simple simulated unit of work to be used in testing.
        /// </summary>
        /// <param name="idx">The index of the created task.</param>
        /// <param name="returnVal">The value the task returns when complete.</param>
        /// <returns>Returns the function.</returns>
        public static Func<int> Create(int idx, int returnVal)
        {
            return () =>
            {
                var currentThread = Thread.CurrentThread.ManagedThreadId;
                var threadCount = System.Diagnostics.Process.GetCurrentProcess().Threads.Count;
                
                // Useful output for debugging.
                Console.WriteLine($"TaskIdx #{idx} : Returns#{returnVal} : Thread #{currentThread} : ThreadCount #{threadCount}");
                
                // Wait a small, but random amount of time to simulate more realistic scenario
                // and ensuring that the tasks don't finish at the same time or in the same order.
                var randWaitTime = Rnd.Next(20, 100);
                Task.Delay(randWaitTime).Wait();
                
                return returnVal;
            };
        }
    }
}