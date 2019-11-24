using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace ThreadSpark.Core.Tests 
{
    [TestFixture]
    public class ConcurrentFunctionRunnerTests
    {
        [Test]
        public void SampleTest()
        {
            var runner = new ConcurrentFunctionRunner(5);

            var funcs = Enumerable.Range(1, 5).Select((rng, idx) => createFunc(idx));
            runner.Run(funcs);
            
            Console.WriteLine("Finished");
            
            Assert.True(true);
        }

        Func<int> createFunc(int idx)
        {
            return () =>
            {
                Thread.Sleep(100);
                Console.WriteLine($"Running {idx}.");
                return 1;
            };
        }
    }
}