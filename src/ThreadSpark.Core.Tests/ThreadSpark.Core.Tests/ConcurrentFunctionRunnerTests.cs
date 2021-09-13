using System;
using System.Linq;
using NUnit.Framework;
using ThreadSpark.Core.Extensions;
using ThreadSpark.Core.Tests.Helpers;

namespace ThreadSpark.Core.Tests 
{
    [TestFixture]
    public class ConcurrentFunctionRunnerTests
    {
        [Test]
        public void TestAllPassOnBasicRun()
        {
            var numTasks = 10;
            var funcs = TestFunctionBuilder.CreateMany(numTasks);
            
            var runner = new ConcurrentFunctionRunner(2);
            var results = runner.Run(funcs);

            // Check there is the expected number of results returned.
            Assert.IsTrue(results.Length == numTasks);
            
            // Check that they have all succeeded.
            Assert.IsTrue(results.All(_ => _.IsSucc()));
        }
        
        [Test]
        public void TestResultsReturnedInSameOrder()
        {
            var numTasks = 20;
            var runner = new ConcurrentFunctionRunner(3);
            
            var funcs = TestFunctionBuilder.CreateMany(numTasks);
            var results = runner.Run(funcs);

            Assert.IsTrue(results.Length == numTasks);
            Assert.IsTrue(results.All(_ => _.IsSucc()));

            // Get all the result values.
            var values = results.AllOrFirstFail().GetValue().ToArray();

            // Check the index of each result, and check against it's return value to ensure
            // that the order of the returned results is the same as the order the tasks were sent.
            for (int idx = 0; idx < values.Length; idx++)
                Assert.AreEqual(values[idx], idx * 5, $"Got {values[idx]}, Expected {idx * 5}");
        }
        
        [Test]
        public void TestNumThreadsCreated()
        {
            Console.WriteLine($"Start Threads = {System.Diagnostics.Process.GetCurrentProcess().Threads.Count}");
            
            var numTasks = 150;
            var runner = new ConcurrentFunctionRunner(5);
            
            var funcs = TestFunctionBuilder.CreateMany(numTasks);
            var results = runner.Run(funcs);

            Assert.IsTrue(results.Length == numTasks);
            Assert.IsTrue(results.All(_ => _.IsSucc()));
            
            Console.WriteLine($"End Threads = {System.Diagnostics.Process.GetCurrentProcess().Threads.Count}");
        }        
    }
}