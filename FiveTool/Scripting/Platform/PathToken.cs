using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FiveTool.Scripting.Platform
{
    internal static class PathToken
    {
        private static readonly Regex PathTokenRegex = new Regex("^([^:]+):([a-z0-9]+):");

        // Maps path token strings to absolute paths
        private static readonly Dictionary<string, string> PathTokens = new Dictionary<string, string>();

        /// <summary>
        /// Generates a token string that points to an absolute path.
        /// </summary>
        /// <param name="absolutePath">The absolute path to point to.</param>
        /// <returns>The token string.</returns>
        public static string Generate(string absolutePath)
        {
            if (!Path.IsPathRooted(absolutePath))
                throw new ArgumentException("Cannot create a path token for a relative path", nameof(absolutePath));
            var fileName = Path.GetFileName(absolutePath);
            var randomStr = RandomHexString(16);
            var token = $"{fileName}:{randomStr}:";
            PathTokens[token] = absolutePath;
            return token;
        }

        /// <summary>
        /// Tries to get the absolute path associated with a path token.
        /// </summary>
        /// <param name="token">The path token.</param>
        /// <param name="absolutePath">The variable to hold the absolute path.</param>
        /// <returns><c>true</c> if the token was resolved.</returns>
        public static bool TryResolve(string token, out string absolutePath)
        {
            return PathTokens.TryGetValue(token, out absolutePath);
        }

        /// <summary>
        /// Determines whether a path begins with a path token.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if the path begins with a path token.</returns>
        public static bool IsInPath(string path)
        {
            return PathTokenRegex.IsMatch(path);
        }

        /// <summary>
        /// Tries to resolve a path which begins with a path token.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="absolutePath">The variable to hold the absolute path.</param>
        /// <returns><c>true</c> if the path was resolved successfully.</returns>
        public static bool TryResolvePath(string path, out string absolutePath)
        {
            absolutePath = null;

            // Split the path into its token and relative path components
            var match = PathTokenRegex.Match(path);
            if (!match.Success)
                return false;
            var token = match.Value;
            var relativePath = path.Substring(match.Length);

            // Get the absolute path corresponding to the token
            if (!TryResolve(token, out absolutePath))
                return false;

            // Safely attach the relative path to it
            if (!string.IsNullOrWhiteSpace(relativePath))
            {
                if (!PathUtil.SimplifyRelativePath(relativePath, out relativePath))
                    return false;
                absolutePath = Path.Combine(absolutePath, relativePath);
            }
            return true;
        }

        /// <summary>
        /// Revokes a path token so that it can no longer be used.
        /// </summary>
        /// <param name="token">The token to revoke.</param>
        public static void Revoke(string token)
        {
            PathTokens.Remove(token);
        }

        private static string RandomHexString(int size)
        {
            var rng = new RNGCryptoServiceProvider();
            var bytes = new byte[size];
            rng.GetBytes(bytes);
            var str = new StringBuilder();
            foreach (var b in bytes)
                str.Append(b.ToString("x2"));
            return str.ToString();
        }
    }
}
