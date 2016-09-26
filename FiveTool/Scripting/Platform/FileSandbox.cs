using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoonSharp.Interpreter;

namespace FiveTool.Scripting.Platform
{
    internal static class FileSandbox
    {
        /// <summary>
        /// Gets the absolute path of a file, throwing a <see cref="ScriptRuntimeException"/> if it is not accessible.
        /// </summary>
        /// <param name="relativePath">The relative path to the file. If it begins with a path token, the token will be resolved as well.</param>
        /// <returns>The absolute path.</returns>
        public static string ResolvePath(string relativePath)
        {
            string absolutePath;
            if (!TryResolvePath(relativePath, out absolutePath))
                throw new ScriptRuntimeException($"File \"{relativePath}\" is not accessible");
            return absolutePath;
        }

        /// <summary>
        /// Gets the absolute path of a file if it is accessible.
        /// </summary>
        /// <param name="relativePath">The relative path to the file. If it begins with a path token, the token will be resolved as well.</param>
        /// <param name="result">The variable which will hold the absolute path.</param>
        /// <returns><c>true</c> if the file is accessible.</returns>
        public static bool TryResolvePath(string relativePath, out string result)
        {
            result = null;
            try
            {
                // If the path contains a path token, resolve the path relative to it
                if (PathToken.IsInPath(relativePath))
                    return PathToken.TryResolvePath(relativePath, out result);

                // Resolve the path relative to the game root
                string simplifiedPath;
                if (!PathUtil.SimplifyRelativePath(relativePath, out simplifiedPath))
                    return false;
                result = Path.Combine(Config.Current.GameRoot, simplifiedPath);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}