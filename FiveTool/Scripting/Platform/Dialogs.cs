using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FiveTool.Scripting.Platform
{
    internal static class Dialogs
    {
        private const string DialogTitle = "FiveTool";

        /// <summary>
        /// Asks the user a yes/no question.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <returns><c>true</c> if the user answered "yes".</returns>
        public static bool YesNoQuestion(string question)
        {
            return MessageBox.Show(question, DialogTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                   DialogResult.Yes;
        }

        /// <summary>
        /// Displays an error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Error(string message)
        {
            MessageBox.Show(message, DialogTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Asks the user to open an existing file.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="filter">The filter string to use.</param>
        /// <param name="path">The variable to hold the absolute path.</param>
        /// <returns><c>true</c> if a file was selected.</returns>
        public static bool OpenFile(string title, string filter, out string path)
        {
            path = null;
            var dialog = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = filter,
                Title = title,
            };
            if (dialog.ShowDialog() != DialogResult.OK)
                return false;
            path = Path.GetFullPath(dialog.FileName);
            return true;
        }

        /// <summary>
        /// Asks the user to save a file.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="filter">The filter string to use.</param>
        /// <param name="suggestedName">The suggested filename to use.</param>
        /// <param name="path">The variable to hold the absolute path.</param>
        /// <returns><c>true</c> if a file was selected.</returns>
        public static bool SaveFile(string title, string filter, string suggestedName, out string path)
        {
            path = null;
            var dialog = new SaveFileDialog
            {
                FileName = suggestedName,
                Filter = filter,
                Title = title,
            };
            if (dialog.ShowDialog() != DialogResult.OK)
                return false;
            path = Path.GetFullPath(dialog.FileName);
            return true;
        }

        /// <summary>
        /// Asks the user to open a folder.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="allowCreate">If set to <c>true</c>, allows the user to create a new folder.</param>
        /// <param name="path">The variable to hold the absolute path.</param>
        /// <returns><c>true</c> if a folder was selected.</returns>
        public static bool OpenFolder(string message, bool allowCreate, out string path)
        {
            path = null;
            var dialog = new FolderBrowserDialog
            {
                Description = message,
                ShowNewFolderButton = allowCreate,
            };
            if (dialog.ShowDialog() != DialogResult.OK)
                return false;
            path = Path.GetFullPath(dialog.SelectedPath);
            return true;
        }

        /// <summary>
        /// Asks the user to browse for a new game folder.
        /// </summary>
        /// <returns><c>true</c> if a folder was selected.</returns>
        public static bool ChooseGameFolder()
        {
            while (true)
            {
                string folder;
                if (!OpenFolder("Select the folder where you dumped the Halo 5: Forge files.", false, out folder))
                    return false;
                var exePath = Path.Combine(folder, "halo5forge.exe");
                if (!File.Exists(Path.Combine(folder, "halo5forge.exe")))
                {
                    if (
                        !YesNoQuestion(
                            "The folder you selected does not look like a valid Halo 5: Forge install. Use it anyways?"))
                        continue;
                }
                try
                {
                    if (File.Exists(exePath))
                        File.OpenRead(exePath).Close();
                }
                catch (Exception)
                {
                    Error(
                        "You do not have read access to the files in that folder.\nYou must dump the game files first before using FiveTool.");
                    continue;
                }
                Config.Current.GameRoot = folder;
                Config.Current.Save(Config.DefaultPath);
                return true;
            }
        }
    }
}
