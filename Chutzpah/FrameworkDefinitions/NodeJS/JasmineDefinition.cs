using Chutzpah.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Chutzpah.FileProcessors;

namespace Chutzpah.FrameworkDefinitions.NodeJS
{

    /// <summary>
    /// Definition that describes the Jasmine framework.
    /// </summary>
    public class JasmineDefinition : BaseFrameworkDefinition
    {
        private IEnumerable<string> fileDependencies = new List<string>();

        /// <summary>
        /// Initializes a new instance of the JasmineDefinition class.
        /// </summary>
        public JasmineDefinition()
        {

        }

        /// <summary>
        /// Gets a list of file dependencies to bundle with the Jasmine test harness.
        /// </summary>
        /// <param name="chutzpahTestSettings"></param>
        public override IEnumerable<string> GetFileDependencies(ChutzpahTestSettingsFile chutzpahTestSettings)
        {
            return fileDependencies;
        }

        public override string GetTestHarness(ChutzpahTestSettingsFile chutzpahTestSettings)
        {
            throw new System.NotImplementedException();
        }
        

        public override string GetBlanketScriptName(ChutzpahTestSettingsFile chutzpahTestSettings)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the runtime for the supported framework
        /// </summary>
        public override JavaScriptEngine JavaScriptEngine {
            get
            {
                return JavaScriptEngine.NodeJS;
            }
        }

        /// <summary>
        /// Gets a short, file system friendly key for the Jasmine library.
        /// </summary>
        public override string FrameworkKey
        {
            get
            {
                return "jasmine";
            }
        }

        /// <summary>
        /// Gets a regular expression pattern to match a testable Jasmine file in a JavaScript file.
        /// </summary>
        protected override Regex FrameworkSignatureJavaScript
        {
            get
            {
                return RegexPatterns.JasmineTestRegexJavaScript;
            }
        }

        /// <summary>
        /// Gets a regular expression pattern to match a testable Jasmine file in a CoffeeScript file.
        /// </summary>
        protected override Regex FrameworkSignatureCoffeeScript
        {
            get
            {
                return RegexPatterns.JasmineTestRegexCoffeeScript;
            }
        }

        /// <summary>
        /// Gets a list of file processors to call within the Process method.
        /// </summary>
        protected override IEnumerable<IReferencedFileProcessor> FileProcessors
        {
            get
            {
                return null;
            }
        }
    }
}
