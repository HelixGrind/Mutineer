using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Mutineer.Interface;
using Mutineer.UnitTesting;

namespace Mutineer
{
    public class Project : IProject
    {
        public string Name { get; }
        public ISourceFile[] Files { get; }
        private readonly List<IMutation> _mutations;

        private const string UnitTestsPath     = @"D:\Projects\Mutineer\Sample\UnitTests\bin\Debug\netcoreapp2.0\UnitTests.dll";
        private const string MutineerTempPath  = @"E:\Data\MutineerTemp";
        private const string DllFilename       = "CoreLogic.dll";
        private const string UnitTestsFilename = "UnitTests.dll";

        public Project(string csprojPath)
        {
            Name       = Path.GetFileNameWithoutExtension(csprojPath);
            Files      = GetSourceFiles(csprojPath);
            _mutations = new List<IMutation>();

            DeleteTempDirectory();
        }

        private static void DeleteTempDirectory()
        {
            foreach (var dir in Directory.GetDirectories(MutineerTempPath)) Directory.Delete(dir, true);
        }

        private SyntaxTree[] GetSyntaxTrees()
        {
            int numFiles = Files.Length;
            var syntaxTrees = new SyntaxTree[numFiles];
            for (int i = 0; i < numFiles; i++) syntaxTrees[i] = Files[i].SyntaxTree;
            return syntaxTrees;
        }

        private static void DumpErrors(EmitResult result, IEnumerable<SyntaxTree> syntaxTrees)
        {
            var failures = result.Diagnostics.Where(diagnostic =>
                diagnostic.IsWarningAsError ||
                diagnostic.Severity == DiagnosticSeverity.Error);

            foreach (var diagnostic in failures)
            {
                Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
            }

            foreach (var syntaxTree in syntaxTrees) Console.WriteLine(syntaxTree);

            Environment.Exit(1);
        }

        public void GenerateMutations(List<IMutagen> mutagens)
        {
            Console.WriteLine("Generating mutations:");

            foreach (var file in Files)
            {
                var rootNode = file.SyntaxTree.GetRoot();
                int numMutations = 0;

                foreach (var mutagen in mutagens)
                {
                    mutagen.NodePairs.Clear();
                    mutagen.Walker.Visit(rootNode);
                    numMutations += AddMutations(file, mutagen.NodePairs);
                }

                Console.WriteLine($"- {file.Filename}: {numMutations} mutations");
            }
        }

        private int AddMutations(ISourceFile file, List<INodePair> nodePairs)
        {
            foreach (var nodePair in nodePairs) _mutations.Add(new Mutation(file, nodePair));
            return nodePairs.Count;
        }

        public void CreateMutatedAssemblies()
        {
            Console.WriteLine();

            AddSyntaxTreesToMutations();

            var references    = GetReferences();
            int mutationIndex = 0;

            foreach (var mutation in _mutations)
            {
                Console.Write($"Evaluating mutation index {mutationIndex}: ");
                var outputDirectory = new OutputDirectory(UnitTestsPath, MutineerTempPath);

                var compilation = CSharpCompilation.Create(Name, mutation.SyntaxTrees, references,
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                var dllPath = outputDirectory.GetPath(DllFilename);

                using (var stream = new FileStream(dllPath, FileMode.Create))
                {
                    var result = compilation.Emit(stream);
                    if (!result.Success) DumpErrors(result, mutation.SyntaxTrees);
                }

                int numFailedUnitTests = GetNumFailedUnitTests(outputDirectory.GetPath(UnitTestsFilename));
                mutation.Result        = MutationState.Killed;

                if (numFailedUnitTests == 0)
                {
                    mutation.Result = MutationState.Survived;
                    var location = mutation.NodePair.Original.GetHierarchicalLocation();

                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("MUTATION SURVIVED!");
                    Console.ResetColor();
                    Console.WriteLine($"Location: {location.Namespace}.{location.Class}.{location.Method} (line {location.LineNumber})");
                    Console.WriteLine($"mutation: [{mutation.NodePair.Original}] --> [{mutation.NodePair.Mutated}]");
                    Console.WriteLine();
                }

                mutationIndex++;
            }
        }

        private void AddSyntaxTreesToMutations()
        {
            var syntaxTrees = GetSyntaxTrees();

            foreach (var mutation in _mutations)
            {
                var newSyntaxTrees = new SyntaxTree[syntaxTrees.Length];
                for (int i = 0; i < syntaxTrees.Length; i++) newSyntaxTrees[i] = syntaxTrees[i];

                var rootNode    = syntaxTrees[mutation.File.FileIndex].GetRoot();
                var programText = rootNode.ReplaceNode(mutation.NodePair.Original, mutation.NodePair.Mutated).ToFullString();
                var syntaxTree  = CSharpSyntaxTree.ParseText(programText);

                newSyntaxTrees[mutation.File.FileIndex] = syntaxTree;
                mutation.SyntaxTrees = newSyntaxTrees;
            }
        }

        private static int GetNumFailedUnitTests(string unitTestsPath)
        {
            var runner  = new MarathonRunner(new[] { unitTestsPath });
            var summary = runner.Run();

            Console.WriteLine($"Total: {summary.Total}, Errors: {summary.Errors}, Failed: {summary.Failed}, Skipped: {summary.Skipped}, Time: {summary.Time:0.00}s");
            return summary.Failed;
        }

        private static List<MetadataReference> GetReferences() =>
            new List<MetadataReference> { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) };

        private static ISourceFile[] GetSourceFiles(string csprojPath)
        {
            var sourceFiles = new List<ISourceFile>();
            var projectDir  = Path.GetDirectoryName(csprojPath);
            var csFiles     = Directory.GetFiles(projectDir, "*.cs", SearchOption.AllDirectories);
            int fileIndex   = 0;

            foreach (var csFile in csFiles)
            {
                if (csFile.Contains("netcoreapp")) continue;
                string programText = File.ReadAllText(csFile);
                var syntaxTree = CSharpSyntaxTree.ParseText(programText);

                sourceFiles.Add(new SourceFile(syntaxTree, Path.GetFileName(csFile), fileIndex));
                fileIndex++;
            }

            return sourceFiles.ToArray();
        }
    }
}
