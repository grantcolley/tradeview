using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Utilities
{
    internal class AssemblyLoader : AssemblyLoadContext
    {
        private readonly string folderPath;
        private readonly ISet<string> dependencies;

        internal AssemblyLoader(string folderPath, IEnumerable<string> dependencies)
        {
            this.folderPath = folderPath;
            this.dependencies = new HashSet<string>(dependencies ?? [], StringComparer.OrdinalIgnoreCase);
        }

        internal Assembly LoadFromMemoryStream(string assemblyPath)
        {
            using var fileStream = File.OpenRead(assemblyPath);
            using var ms = new MemoryStream();
            fileStream.CopyTo(ms);
            ms.Position = 0;

            return LoadFromStream(ms);
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            var name = assemblyName.Name;

            if (string.IsNullOrWhiteSpace(name)) return null;

            if (dependencies.Count > 0 
                && !dependencies.Contains(name))
            { 
                return null;
            }

            var candidate = Path.Combine(folderPath, $"{name}.dll");
            if (File.Exists(candidate))
            {
                return LoadFromMemoryStream(candidate);
            }

            return null;
        }
    }
}
