using System;
using System.Collections.Generic;
using Mutineer.Interface;
using Mutineer.Walkers;

namespace Mutineer
{
    internal static class MutineerMain
    {
        private static void Main()
        {
            var walkers      = new List<IMutagen> { new BinaryExpressionMutagen(), new OperatorTokenMutagen() };
            IProject project = new Project(@"D:\Projects\Mutineer\Sample\CoreLogic\CoreLogic.csproj");

            project.GenerateMutations(walkers);
            project.CreateMutatedAssemblies();
            //project.RunMutatedAssemblies();

            Environment.Exit(1);
        }
    }
}
