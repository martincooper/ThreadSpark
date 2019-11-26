using NUnit.Framework;
using ThreadSpark.Core.Extensions;

namespace ThreadSpark.Core.Tests 
{
    [TestFixture]
    public class ConcurrentFunctionRunnerExtensionsTests
    {
        [Test]
        public void TestThreeTupleResultsAreInCorrectOrder()
        {
            var runner = new ConcurrentFunctionRunner(3);
            
            // Using the Tuple extension methods.
            var (firstCall, secondCall, thirdCall) = runner.Run(() => 1, () => 2, () => 3);
            
            Assert.IsTrue(firstCall.IsSucc());
            Assert.AreEqual(firstCall.GetValue(), 1);
            
            Assert.IsTrue(secondCall.IsSucc());
            Assert.AreEqual(secondCall.GetValue(), 2);
            
            Assert.IsTrue(thirdCall.IsSucc());
            Assert.AreEqual(thirdCall.GetValue(), 3);
        }
    }
}