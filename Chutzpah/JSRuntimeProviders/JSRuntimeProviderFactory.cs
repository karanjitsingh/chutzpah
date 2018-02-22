using Chutzpah.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chutzpah.JSRuntimeProviders
{
    public class JSRuntimeProviderFactory : IJSRuntimeProviderFactory
    {
        private readonly ITestCaseStreamReaderFactory testCaseStreamReaderFactory;
        private readonly IFileProbe fileProbe;
        private readonly IProcessHelper process;
        private readonly IUrlBuilder urlBuilder;

        private IDictionary<JavaScriptEngine, IJSRuntimeProvider> EnumProviderMap = new Dictionary<JavaScriptEngine, IJSRuntimeProvider>();

        public JSRuntimeProviderFactory(ITestCaseStreamReaderFactory testCaseStreamReaderFactory,
                                        IFileProbe fileProbe,
                                        IProcessHelper process,
                                        IUrlBuilder urlBuilder,
                                        ConcurrentBag<TestContext> testContexts,
                                        TestOptions options)
        {
            this.testCaseStreamReaderFactory = testCaseStreamReaderFactory;
            this.fileProbe = fileProbe;
            this.process = process;
            this.urlBuilder = urlBuilder;

            InitProviders(testContexts, options);
        }

        private void InitProviders(ConcurrentBag<TestContext> testContexts, TestOptions options)
        {

            if (options.TestLaunchMode == TestLaunchMode.FullBrowser)
                return;

            var requiredEngines = new HashSet<JavaScriptEngine>();

            Parallel.ForEach(testContexts, context =>
            {
                requiredEngines.Add(context.TestFileSettings.JavaScriptEngine);
            });

            foreach (JavaScriptEngine engine in System.Enum.GetValues(typeof(JavaScriptEngine)))
            {
                IJSRuntimeProvider provider;

                if (!requiredEngines.Contains(engine))
                    continue;

                switch(engine)
                {
                    case JavaScriptEngine.NodeJS:
                        provider = new NodeRuntimeProvider(testCaseStreamReaderFactory, fileProbe, process);
                        break;
                    case JavaScriptEngine.PhantomJS:
                        provider = new PhantomRuntimeProvider(testCaseStreamReaderFactory, fileProbe, process, urlBuilder);
                        break;
                    default:
                        provider = new PhantomRuntimeProvider(testCaseStreamReaderFactory, fileProbe, process, urlBuilder);
                        break;
                }

                EnumProviderMap.Add(engine, provider);
            }
        }

        public IJSRuntimeProvider GetRuntimeProvider(JavaScriptEngine javaScriptEngine)
        {
            return EnumProviderMap[javaScriptEngine];
        }
    }
}
