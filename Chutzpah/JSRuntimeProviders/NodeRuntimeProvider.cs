using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chutzpah.BatchProcessor;
using Chutzpah.Exceptions;
using Chutzpah.Models;
using Chutzpah.Utility;
using Chutzpah.Transformers;
using Chutzpah.Server;
using Chutzpah.Server.Models;

namespace Chutzpah.JSRuntimeProviders
{
    public class NodeRuntimeProvider: IJSRuntimeProvider
    {
        public static string HeadlessBrowserName = @"node.exe";
        public static string TestRunnerJsName = @"ChutzpahJSRunners\NodeJS\chutzpahRunner.js";

        private readonly ITestCaseStreamReaderFactory testCaseStreamReaderFactory;
        private readonly IFileProbe fileProbe;
        private readonly IProcessHelper process;

        private string headlessBrowserPath;

        public bool RequiresTestHarness()
        {
            return false;
        }
       
        public NodeRuntimeProvider(ITestCaseStreamReaderFactory testCaseStreamReaderFactory,
                                   IFileProbe fileProbe,
                                   IProcessHelper process)
        {
            this.testCaseStreamReaderFactory = testCaseStreamReaderFactory;
            this.fileProbe = fileProbe;
            this.process = process;

            headlessBrowserPath = fileProbe.FindFilePath(HeadlessBrowserName);
            if (headlessBrowserPath == null)
                throw new FileNotFoundException("Unable to find headless browser: " + HeadlessBrowserName);
            if (fileProbe.FindFilePath(TestRunnerJsName) == null)
                throw new FileNotFoundException("Unable to find test runner base js file: " + TestRunnerJsName);
        }

        public IList<TestFileSummary> InvokeTestRunner(TestOptions options,
                                                       TestContext testContext,
                                                       TestExecutionMode testExecutionMode,
                                                       ITestMethodRunnerCallback callback,
                                                       Action<int, string, IList<TestError>, ITestMethodRunnerCallback> HandleTestProcessExitCode,
                                                       bool m_debugEnabled)
        {
            //string runnerPath = fileProbe.FindFilePath(testContext.TestRunner);
            //string fileUrl = BuildHarnessUrl(testContext);
            string runnerPath = fileProbe.FindFilePath(testContext.TestRunner);
            string fileUrl = ConcatTestPaths(testContext);

            string runnerArgs = BuildRunnerArgs(options, testContext, fileUrl, runnerPath, testExecutionMode);

            Func<ProcessStream, IList<TestFileSummary>> streamProcessor =
            processStream => testCaseStreamReaderFactory.Create().Read(processStream, options, testContext, callback, m_debugEnabled);

            var envVars = BuildEnvironmentVariables();

            var processResult = process.RunExecutableAndProcessOutput(headlessBrowserPath, runnerArgs, streamProcessor, envVars);

            HandleTestProcessExitCode(processResult.ExitCode, testContext.FirstInputTestFile, processResult.Model.Select(x => x.Errors).FirstOrDefault(), callback);

            return processResult.Model;
        }

        private IDictionary<string, string> BuildEnvironmentVariables()
        {
            var envVars = new Dictionary<string, string>();

            var chutzpahNodeModules = fileProbe.FindFolderPath(@"ChutzpahJSRunners\NodeJS\node_modules");
            envVars.Add("NODE_PATH", chutzpahNodeModules);
            return envVars;
        }

        private static string ConcatTestPaths(TestContext testContext)
        {
            if(testContext.InputTestFiles.Count == 1)
            {
                return testContext.FirstInputTestFile;
            }
            var combinedPaths = new StringBuilder();
            foreach(var path in testContext.InputTestFiles)
            {
                combinedPaths.Append(path);
                combinedPaths.Append(";");
            }

            return combinedPaths.ToString().TrimEnd(';');
        }
        private static string BuildRunnerArgs(TestOptions options, TestContext context, string fileUrl, string runnerPath, TestExecutionMode testExecutionMode)
        {
            string runnerArgs;
            var testModeStr = testExecutionMode.ToString().ToLowerInvariant();
            var timeout = context.TestFileSettings.TestFileTimeout ?? options.TestFileTimeoutMilliseconds ?? Constants.DefaultTestFileTimeout;
            //var proxy = options.Proxy ?? context.TestFileSettings.Proxy;
            //var proxySetting = string.IsNullOrEmpty(proxy) ? "--proxy-type=none" : string.Format("--proxy={0}", proxy);

            runnerArgs = string.Format("{0} {1} {2} {3}",
                                        runnerPath,
                                        fileUrl,
                                        testModeStr,
                                        timeout);

            return runnerArgs;
        }

    }
}