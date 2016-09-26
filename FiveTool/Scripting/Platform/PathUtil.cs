using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveTool.Scripting.Platform
{
    internal static class PathUtil
    {
        /// <summary>
        /// Attempts to simplify a relative path, removing components such as "." and "..".
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="simplifiedPath">The variable which will hold the simplified path.</param>
        /// <returns><c>true</c> if the path was simplified successfully.</returns>
        public static bool SimplifyRelativePath(string path, out string simplifiedPath)
        {
            simplifiedPath = null;

            // If the path is absolute, then it's invalid
            if (Path.IsPathRooted(path))
                return false;

            // Split up the path into its components and build an array containing the output components in order
            var inComponents = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var outComponents = new string[inComponents.Length];
            var outComponentCount = 0;
            foreach (var component in inComponents)
            {
                if (component == "..")
                {
                    // Remove the last component
                    if (outComponentCount == 0)
                        return false;
                    outComponentCount--;
                }
                else if (component != "." && !string.IsNullOrEmpty(component))
                {
                    // Add a component to the end
                    outComponents[outComponentCount] = component;
                    outComponentCount++;
                }
            }
            if (outComponentCount == 0)
                return false; // If the path is empty, then fail
            simplifiedPath = string.Join(Path.DirectorySeparatorChar.ToString(), outComponents, 0, outComponentCount);
            return true;
        }
    }
}
