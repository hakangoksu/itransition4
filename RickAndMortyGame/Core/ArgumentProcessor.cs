using System;
using System.IO;
using System.Reflection;
using RickAndMortyGame.GameCore; 

namespace RickAndMortyGame.Core
{
    public static class ArgumentProcessor
    {
        public class GameConfiguration
        {
            public required int BoxCount { get; init; }
            public required Type MortyType { get; init; }
        }

        public static GameConfiguration Process(string[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException("Incorrect number of arguments provided. Usage: <BoxCount> <MortyType>");
            }

            if (!int.TryParse(args[0], out int boxCount) || boxCount < 3)
            {
                throw new ArgumentException($"Invalid box count '{args[0]}'. The number of boxes must be an integer greater than 2.");
            }

            string fullMortyArgument = args[1];

            string assemblyNameOrPath;
            string? className = null;

            int separatorIndex = fullMortyArgument.LastIndexOf(':');

            if (separatorIndex != -1)
            {
                assemblyNameOrPath = fullMortyArgument.Substring(0, separatorIndex);
                className = fullMortyArgument.Substring(separatorIndex + 1);
            }
            else
            {
                assemblyNameOrPath = Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty;
                className = fullMortyArgument;
            }

            string entryAssemblyName = Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty;

            Type mortyType;
            string simpleAssemblyName = Path.GetFileNameWithoutExtension(assemblyNameOrPath);
            if (simpleAssemblyName.Equals(entryAssemblyName, StringComparison.OrdinalIgnoreCase))
            {
                mortyType = LoadMortyImplementationFromLoadedAssembly(className);
            }
            else
            {
                if (!File.Exists(assemblyNameOrPath))
                {
                    throw new FileNotFoundException($"Morty implementation file not found: '{assemblyNameOrPath}'. The full argument provided was: '{fullMortyArgument}'.");
                }
                mortyType = LoadMortyImplementationFromFile(assemblyNameOrPath, className);
            }

            return new GameConfiguration { BoxCount = boxCount, MortyType = mortyType };
        }

        private static Type LoadMortyImplementationFromFile(string assemblyPath, string? className)
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(assemblyPath);
                return FindMortyTypeInAssembly(assembly, assemblyPath, className)!;
            }
            catch (Exception ex) when (ex is FileNotFoundException || ex is BadImageFormatException || ex is TypeLoadException)
            {
                throw new ApplicationException($"Failed to load Morty implementation from '{assemblyPath}'. Details: {ex.Message}");
            }
        }

        private static Type LoadMortyImplementationFromLoadedAssembly(string? className)
        {
            Assembly? assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
            {
                throw new ApplicationException("Cannot determine the running application assembly.");
            }
            return FindMortyTypeInAssembly(assembly, assembly.Location, className)!;
        }


        private static Type? FindMortyTypeInAssembly(Assembly assembly, string assemblySource, string? className)
        {
            Type? mortyType = null;

            if (!string.IsNullOrEmpty(className))
            {
                mortyType = assembly.GetType(className, throwOnError: false);

                if (mortyType == null && assembly == Assembly.GetEntryAssembly())
                {
                    const string defaultNamespace = "RickAndMortyGame.Morties";
                    string fullTypeName = $"{defaultNamespace}.{className}";
                    mortyType = assembly.GetType(fullTypeName, throwOnError: false);
                }
            }
            else
            {
                foreach (Type t in assembly.GetExportedTypes())
                {
                    if (typeof(IMorty).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                    {
                        mortyType = t;
                        break;
                    }
                }
            }

            if (mortyType == null || !typeof(IMorty).IsAssignableFrom(mortyType) || mortyType.IsAbstract)
            {
                string message = $"This Morty ('{className}') doesn't exist in any possible universe (could not find a concrete implementation of '{nameof(IMorty)}'";

                if (assembly != Assembly.GetEntryAssembly())
                {
                    message += $" in '{Path.GetFileName(assemblySource)}'";
                }

                message += ").";

                throw new TypeLoadException(message);
            }

            return mortyType;
        }
    }
}
