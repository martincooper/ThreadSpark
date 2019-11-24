using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using ThreadSpark.Core.Extensions;

namespace ThreadSpark.Core.Tests 
{
    [TestFixture]
    public class ConcurrentFunctionRunnerTests
    {
        [Test]
        public void SampleTest()
        {
            var runner = new ConcurrentFunctionRunner(3);

            var funcs = Enumerable.Range(1, 20).Select((rng, idx) => createFunc(idx));
            var result = runner.BeginRun(funcs);

            Console.WriteLine("Starting");
            var x = result.Result;
            Console.WriteLine("Finished");
            
            Assert.True(true);
        }

        Func<int> createFunc(int idx)
        {
            return () =>
            {
                Console.WriteLine($"Running {idx}.");
                Task.Delay(1000).Wait();
                return 1;
            };
        }

        void Test()
        {
            var runner = new ConcurrentFunctionRunner(3);
            
            // Using the Tuple extension methods.
            var (firstCall, secondCall) = runner.Run(() => 1, () => 2);
        }
     
    }
}