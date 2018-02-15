using Chutzpah.Models;

namespace Chutzpah.JSRuntimeProviders
{
    public class JSRuntimeProviderFactory : IJSRuntimeProviderFactory
    {
        public IJSRuntimeProvider Create(JavaScriptEngine? javaScriptEngine,
                                         ITestCaseStreamReaderFactory testCaseStreamReaderFactory,
                                         IFileProbe fileProbe,
                                         IProcessHelper process,
                                         IUrlBuilder urlBuilder)
        {
            if (javaScriptEngine == JavaScriptEngine.NodeJS)
                return new NodeRuntimeProvider(testCaseStreamReaderFactory, fileProbe, process);
            
            return new PhantomRuntimeProvider(testCaseStreamReaderFactory, fileProbe, process, urlBuilder);
        }
    }
}
