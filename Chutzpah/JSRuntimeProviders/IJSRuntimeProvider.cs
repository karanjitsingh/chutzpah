using Chutzpah.Models;
using Chutzpah.Server.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chutzpah.JSRuntimeProviders
{
    public interface IJSRuntimeProvider
    {
        bool RequiresTestHarness();

        IList<TestFileSummary> InvokeTestRunner(TestOptions options,
                                                TestContext testContext,
                                                TestExecutionMode testExecutionMode,
                                                ITestMethodRunnerCallback callback,
                                                Action<int, string, IList<TestError>, ITestMethodRunnerCallback> HandleTestProcessExitCode,
                                                bool m_debugEnabled);
    }
}
