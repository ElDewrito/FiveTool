using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Platforms;

namespace FiveTool.Scripting.Platform
{
    internal class FiveToolPlatformAccessor : PlatformAccessorBase
    {
        private const string TempFileDirName = "FiveTool";

        private readonly TempFileCollection _tempFiles = new TempFileCollection();

        /// <summary>
        /// Removes any temporary files created by scripts.
        /// </summary>
        public static void ClearTempFiles()
        {
            try
            {
                var path = Path.Combine(Path.GetTempPath(), TempFileDirName);
                if (Directory.Exists(path))
                    Directory.Delete(path, true);
            }
            catch
            {
#if DEBUG
                throw;
#endif
            }
        }

        public override string GetPlatformNamePrefix()
        {
            return "FiveTool";
        }

        public override void DefaultPrint(string content)
        {
            Console.WriteLine(content);
        }

        public override string DefaultInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        public override Stream IO_OpenFile(Script script, string filename, Encoding encoding, string mode)
        {
            filename = FileSandbox.ResolvePath(filename);
            var openMode = ParseFileMode(mode);
            return File.Open(filename, openMode.Mode, openMode.Access);
        }

        public override Stream IO_GetStandardStream(StandardFileType type)
        {
            switch (type)
            {
                case StandardFileType.StdIn:
                    return Console.OpenStandardInput();
                case StandardFileType.StdOut:
                    return Console.OpenStandardOutput();
                case StandardFileType.StdErr:
                    return Console.OpenStandardError();
                default:
                    throw new InvalidOperationException($"Unrecognized StandardFileType {type}");
            }
        }

        public override string IO_OS_GetTempFilename()
        {
            var dir = Path.Combine(Path.GetTempPath(), TempFileDirName);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            var fileName = Path.Combine(dir, Path.GetRandomFileName());
            File.Create(fileName).Dispose();
            _tempFiles.AddFile(fileName, false);
            return PathToken.Generate(fileName);
        }

        public override void OS_ExitFast(int exitCode)
        {
            Environment.Exit(exitCode);
        }

        public override bool OS_FileExists(string file)
        {
            return FileSandbox.TryResolvePath(file, out file) && File.Exists(file);
        }

        public override void OS_FileDelete(string file)
        {
            file = FileSandbox.ResolvePath(file);
            File.Delete(file);
        }

        public override void OS_FileMove(string src, string dst)
        {
            src = FileSandbox.ResolvePath(src);
            dst = FileSandbox.ResolvePath(dst);
            File.Move(src, dst);
        }

        public override int OS_Execute(string cmdline)
        {
            throw new ScriptRuntimeException("Executing programs is not allowed");
        }

        public override CoreModules FilterSupportedCoreModules(CoreModules module)
        {
            return module & ~CoreModules.LoadMethods;
        }

        public override string GetEnvironmentVariable(string envvarname)
        {
            return null;
        }

        private class FileOpenMode
        {
            public FileMode Mode { get; set; }

            public FileAccess Access { get; set; }
        }

        private static FileOpenMode ParseFileMode(string mode)
        {
            // Ignore the binary suffix
            if (mode.EndsWith("b"))
                mode = mode.Substring(0, mode.Length - 1);

            switch (mode)
            {
                case "r":
                    return new FileOpenMode { Mode = FileMode.Open, Access = FileAccess.Read };
                case "r+":
                    return new FileOpenMode { Mode = FileMode.Open, Access = FileAccess.ReadWrite };
                case "w":
                    return new FileOpenMode { Mode = FileMode.Create, Access = FileAccess.Write };
                case "w+":
                    return new FileOpenMode { Mode = FileMode.Create, Access = FileAccess.ReadWrite };
                case "a":
                    return new FileOpenMode { Mode = FileMode.Append, Access = FileAccess.Write };
                case "a+":
                    return new FileOpenMode { Mode = FileMode.Append, Access = FileAccess.ReadWrite };
                default:
                    throw new ScriptRuntimeException($"Unsupported file mode \"{mode}\"");
            }
        }
    }
}
