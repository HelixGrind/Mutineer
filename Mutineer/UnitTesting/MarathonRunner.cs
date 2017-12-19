using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Mutineer.UnitTesting
{
    public class MarathonRunner
    {
        private readonly ConcurrentDictionary<string, ExecutionSummary> _completionMessages;
        private IMessageSinkWithTypes _reporterMessageHandler;
        private readonly XunitProject _project;        

        public MarathonRunner(IEnumerable<string> assemblyPaths)
        {
            _project            = GetProject(assemblyPaths);
            _completionMessages = new ConcurrentDictionary<string, ExecutionSummary>();
        }

        private static XunitProject GetProject(IEnumerable<string> assemblyPaths)
        {
            var project = new XunitProject();

            foreach (var assemblyPath in assemblyPaths)
            {
                if (File.Exists(assemblyPath)) project.Add(new XunitProjectAssembly { AssemblyFilename = assemblyPath });
            }

            return project;
        }

        private ExecutionSummary RunProject(XunitProject project)
        {
            var originalWorkingFolder = Directory.GetCurrentDirectory();
            foreach (var assembly in project.Assemblies) ExecuteAssembly(assembly);
            Directory.SetCurrentDirectory(originalWorkingFolder);
            return GetTotalExecutionSummary(_completionMessages.Values);
        }

        private static ExecutionSummary GetTotalExecutionSummary(IEnumerable<ExecutionSummary> summaries)
        {
            var totalSummary = new ExecutionSummary();

            foreach (var summary in summaries)
            {
                totalSummary.Errors  += summary.Errors;
                totalSummary.Failed  += summary.Failed;
                totalSummary.Skipped += summary.Skipped;
                totalSummary.Time    += summary.Time;
                totalSummary.Total   += summary.Total;
            }

            return totalSummary;
        }

        private void ExecuteAssembly(XunitProjectAssembly assembly)
        {
            try
            {
                // Turn off pre-enumeration of theories, since there is no theory selection UI in this runner
                assembly.Configuration.PreEnumerateTheories = false;

                // Setup discovery and execution options with command-line overrides
                var discoveryOptions = TestFrameworkOptions.ForDiscovery(assembly.Configuration);
                var executionOptions = TestFrameworkOptions.ForExecution(assembly.Configuration);

                var appDomainSupport = assembly.Configuration.AppDomainOrDefault;
                var shadowCopy       = assembly.Configuration.ShadowCopyOrDefault;

                using (AssemblyHelper.SubscribeResolveForAssembly(assembly.AssemblyFilename))
                using (var controller = new XunitFrontController(appDomainSupport, assembly.AssemblyFilename, assembly.ConfigFilename, shadowCopy))
                using (var discoverySink = new TestDiscoverySink())
                {
                    controller.Find(false, discoverySink, discoveryOptions);
                    discoverySink.Finished.WaitOne();

                    var numTestCases = discoverySink.TestCases.Count;
                    if (numTestCases == 0) return;

                    using (IExecutionSink executionMessageSink = new DelegatingExecutionSummarySink(
                        _reporterMessageHandler, null, (path, summary) => _completionMessages.TryAdd(path, summary)))
                    {
                        controller.RunTests(discoverySink.TestCases, executionMessageSink, executionOptions);
                        executionMessageSink.Finished.WaitOne();
                    }
                }
            }
            catch (Exception ex)
            {
                var e = ex;
                while (e != null)
                {
                    Console.WriteLine($"{e.GetType().FullName}: {e.Message}");
                    e = e.InnerException;
                }
            }
        }

        public ExecutionSummary Run()
        {
            ExecutionSummary summary = null;

            try
            {
                var reporter = new DefaultRunnerReporterWithTypes();
                _reporterMessageHandler = MessageSinkWithTypesAdapter.Wrap(reporter.CreateMessageHandler(new NullLogger()));
                summary = RunProject(_project);                
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: {e.Message}");
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                Console.ResetColor();
            }

            return summary;
        }
    }
}
