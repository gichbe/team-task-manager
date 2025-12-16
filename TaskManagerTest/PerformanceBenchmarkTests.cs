using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TaskManagerTest
{
    [TestClass]
    public class PerformanceBenchmarkTests
    {
        [TestMethod]
        public void RunBenchmark()
        {
            BenchmarkRunner.Run();
            Assert.IsTrue(true);
        }
    }
}
