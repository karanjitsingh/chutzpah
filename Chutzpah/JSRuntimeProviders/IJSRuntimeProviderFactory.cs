using Chutzpah.Models;

namespace Chutzpah.JSRuntimeProviders
{
    public interface IJSRuntimeProviderFactory
    {
        IJSRuntimeProvider Create(JavaScriptEngine? javaScriptEngine,
                                  ITestCaseStreamReaderFactory testCaseStreamReaderFactory,
                                  IFileProbe fileProbe,
                                  IProcessHelper process,
                                  IUrlBuilder urlBuilder);
    }
}