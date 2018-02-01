using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chutzpah.Models;
using Chutzpah.Server.Models;

namespace Chutzpah
{
    public class TestRunner : ITestRunner
    {
        public IChutzpahWebServerHost ActiveWebServerHost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public static ITestRunner Create()
        {
            return NodeTestRunner.Create();
        }

        public void CleanTestContext(TestContext context)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TestCase> DiscoverTests(string testPath)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TestCase> DiscoverTests(IEnumerable<string> testPaths)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TestCase> DiscoverTests(IEnumerable<string> testPaths, TestOptions options, ITestMethodRunnerCallback callback)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TestCase> DiscoverTests(IEnumerable<string> testPaths, TestOptions options)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TestCase> DiscoverTests(IEnumerable<string> testPaths, TestOptions options, out IList<TestError> errors)
        {
            throw new NotImplementedException();
        }

        public void EnableDebugMode()
        {
            throw new NotImplementedException();
        }

        public TestContext GetTestContext(string testFile)
        {
            throw new NotImplementedException();
        }

        public TestContext GetTestContext(string testFile, TestOptions options)
        {
            throw new NotImplementedException();
        }

        public bool IsTestFile(string testFile, ChutzpahSettingsFileEnvironments envionrments)
        {
            throw new NotImplementedException();
        }

        public TestCaseSummary RunTests(string testPath, ITestMethodRunnerCallback callback = null)
        {
            throw new NotImplementedException();
        }

        public TestCaseSummary RunTests(string testPath, TestOptions options, ITestMethodRunnerCallback callback = null)
        {
            throw new NotImplementedException();
        }

        public TestCaseSummary RunTests(IEnumerable<string> testPaths, TestOptions options, ITestMethodRunnerCallback callback = null)
        {
            throw new NotImplementedException();
        }

        public TestCaseSummary RunTests(IEnumerable<string> testPaths, ITestMethodRunnerCallback callback = null)
        {
            throw new NotImplementedException();
        }
    }
}
