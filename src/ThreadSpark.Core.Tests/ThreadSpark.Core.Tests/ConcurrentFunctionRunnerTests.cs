using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

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
                Task.Delay(400).Wait();
                return 1;
            };
        }
    }
}