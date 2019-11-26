using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using ThreadSpark.Core.Extensions;
using ThreadSpark.Core.Tests.Helpers;

namespace ThreadSpark.Core.Tests 
{
    [TestFixture]
    public class ConcurrentFunctionRunnerTests
    {
        private readonly Random _rnd = new Random();
        
        [Test]
        public void CheckAllPassOnSuccess()
        {
            var numTasks = 200;
            
            var runner = new ConcurrentFunctionRunner(5);
            
            var funcs = Enumerable
                .Range(0, numTasks)
                .Select(idx => TestFunctionBuilder.Create(idx)).ToArray();
            
            var results = runner.Run(funcs);

            Assert.IsTrue(results.Length == numTasks);
            Assert.IsTrue(results.All(_ => _.IsSucc()));

            var values = results.AllOrFirstFail().GetValue();

            for (int idx = 0; idx < values.Length; idx++)
                Assert.AreEqual(values[idx], idx * 5, $"Got {values[idx]}, Expected {idx * 5}");
        }

        [Test]
        public void Test()
        {
            var runner = new ConcurrentFunctionRunner(3);
            
            // Using the Tuple extension methods.
            var (firstCall, secondCall, thirdCall) = runner.Run(() => 1, () => 2, () => 3);
        }
    }
}