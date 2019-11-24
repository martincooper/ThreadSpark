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
        public void CheckRunAll()
        {
            var runner = new ConcurrentFunctionRunner(5);
            var funcs = Enumerable.Range(0, 20).Select(createFunc);
            var results = runner.Run(funcs);

            Assert.IsTrue(results.Length == 20);
            Assert.IsTrue(results.All(_ => _.IsSucc()));

            var values = results.AllOrFirstFail().GetValue();

            //for (int idx = 0; idx < values.Length; idx++)
                //Assert.AreEqual(idx, values[idx] * 5);
        }

        Func<int> createFunc(int idx)
        {
            return () =>
            {
                Console.WriteLine($"Task #{idx}.");
                Task.Delay(20).Wait();
                return idx * 5;
            };
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