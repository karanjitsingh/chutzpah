using Chutzpah.Models;
using System.Collections.Concurrent;

namespace Chutzpah.JSRuntimeProviders
{
    public interface IJSRuntimeProviderFactory
    {
        IJSRuntimeProvider GetRuntimeProvider(JavaScriptEngine javaScriptEngine);
    }
}