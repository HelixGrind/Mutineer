using System.IO;

namespace Mutineer
{
    public class OutputDirectory
    {
        private readonly string _path;

        public OutputDirectory(string unitTestsPath, string tempRoot)
        {
            if (!Directory.Exists(tempRoot)) throw new DirectoryNotFoundException($"The directory ({tempRoot}) does not exist.");

            _path = Path.Combine(tempRoot, Path.GetRandomFileName());
            Directory.CreateDirectory(_path);

            CopyUnitTestsDll(unitTestsPath);
        }

        private void CopyUnitTestsDll(string unitTestsPath)
        {
            var outputDir = Path.GetDirectoryName(unitTestsPath);
            var unitTestStub = Path.GetFileNameWithoutExtension(unitTestsPath);

            var unitTestFiles = Directory.GetFiles(outputDir, $"{unitTestStub}.*");

            foreach (var file in unitTestFiles)
            {
                var filename = Path.GetFileName(file);
                File.Copy(file, Path.Combine(_path, filename));
            }
        }

        public string GetPath(string filename) => Path.Combine(_path, filename);
    }
}
