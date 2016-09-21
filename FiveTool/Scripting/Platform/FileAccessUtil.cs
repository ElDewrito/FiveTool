using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MoonSharp.Interpreter;

namespace FiveTool.Scripting.Platform
{
    internal static class FileAccessUtil
    {
        private const string DialogTitle = "FiveTool";

        /// <summary>
        /// Gets the absolute path of a file, throwing a <see cref="ScriptRuntimeException"/> if it is not accessible.
        /// </summary>
        /// <param name="relativePath">The relative path to the file.</param>
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
        /// <param name="relativePath">The relative path to the file.</param>
        /// <param name="result">The variable which will hold the absolute path.</param>
        /// <returns><c>true</c> if the file is accessible.</returns>
        public static bool TryResolvePath(string relativePath, out string result)
        {
            result = null;
            try
            {
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

        /// <summary>
        /// Asks the user to browse for a new game folder.
        /// </summary>
        /// <returns><c>true</c> if a folder was selected.</returns>
        public static bool ChooseGameFolder()
        {
            while (true)
            {
                // TODO: Decouple this from winforms...
                var dialog = new FolderBrowserDialog
                {
                    Description = "Select the folder where you dumped the Halo 5: Forge files.",
                    ShowNewFolderButton = false,
                };
                if (dialog.ShowDialog() != DialogResult.OK)
                    return false;
                var folder = dialog.SelectedPath;
                var exePath = Path.Combine(folder, "halo5forge.exe");
                if (!File.Exists(Path.Combine(folder, "halo5forge.exe")))
                {
                    if (
                        MessageBox.Show("The folder you selected does not look like a valid Halo 5: Forge install. Use it anyways?",
                            DialogTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        continue;
                }
                try
                {
                    if (File.Exists(exePath))
                        File.OpenRead(exePath).Close();
                }
                catch (Exception)
                {
                    MessageBox.Show(
                        "You do not have read access to the files in that folder.\nYou must dump the game files first before using FiveTool.",
                        DialogTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }
                Config.Current.GameRoot = folder;
                Config.Current.Save(Config.DefaultPath);
                return true;
            }
        }
    }
}
