// Note: this is called AusarModule instead of just Module because there's already a .NET class called Module.
// Other class names shouldn't really begin with "Ausar".

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Ausar.Module.Structures;

namespace FiveLib.Ausar.Module
{
    /// <summary>
    /// An Ausar module file.
    /// </summary>
    public class AusarModule
    {
        private readonly Stream _stream;
        private readonly ModuleBlockCompressor _blockCompressor;
        private readonly List<ModuleEntry> _entries = new List<ModuleEntry>();
        private readonly List<ModuleEntry> _resources = new List<ModuleEntry>();

        private readonly Dictionary<string, ModuleEntry> _entriesByName; 
        private readonly Dictionary<int, ModuleEntry> _entriesByGlobalTagId; 

        private AusarModule(Stream stream, IEnumerable<ModuleEntry> entries, IEnumerable<ModuleEntry> resources)
        {
            _stream = stream;
            DataBaseOffset = stream.Position;
            _blockCompressor = new ModuleBlockCompressor(_stream, DataBaseOffset);

            _entries.AddRange(entries);
            _resources.AddRange(resources);

            _entriesByName = _entries.ToDictionary(e => e.Name, e => e);
            _entriesByGlobalTagId = _entries
                .Where(e => e.GlobalTagId != -1)
                .ToDictionary(e => e.GlobalTagId, e => e);

            Entries = _entries.AsReadOnly();
            Resources = _resources.AsReadOnly();
        }

        /// <summary>
        /// Gets the entries for files stored in the module. Read-only.
        /// </summary>
        public IList<ModuleEntry> Entries { get; }

        /// <summary>
        /// Gets the entries for resource files stored in the module. Read-only.
        /// These entries can also be found in the main <see cref="Entries"/> list.
        /// </summary>
        public IList<ModuleEntry> Resources { get; }

        /// <summary>
        /// Gets the file offset where compressed data begins.
        /// </summary>
        public long DataBaseOffset { get; }

        /// <summary>
        /// Looks up an entry by its filename.
        /// </summary>
        /// <param name="name">The filename to search for.</param>
        /// <param name="entry">The result variable.</param>
        /// <returns><c>true</c> if the entry was found.</returns>
        public bool GetEntryByName(string name, out ModuleEntry entry)
        {
            return _entriesByName.TryGetValue(name, out entry);
        }

        /// <summary>
        /// Looks up an entry by its global tag ID.
        /// </summary>
        /// <param name="id">The global tag ID to search for.</param>
        /// <param name="entry">The result variable.</param>
        /// <returns><c>true</c> if the entry was found.</returns>
        public bool GetEntryByGlobalTagId(int id, out ModuleEntry entry)
        {
            return _entriesByGlobalTagId.TryGetValue(id, out entry);
        }

        /// <summary>
        /// Opens a stream on an entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>A stream which can be used to read the entry data.</returns>
        public ModuleBlockStream OpenEntry(ModuleEntry entry)
        {
            return new ModuleBlockStream(_blockCompressor, entry);
        }

        /// <summary>
        /// Extracts an entire entry to a stream.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="outStream">The stream to extract to.</param>
        public void ExtractEntry(ModuleEntry entry, Stream outStream)
        {
            using (var entryStream = OpenEntry(entry))
                entryStream.CopyTo(outStream);
        }

        /// <summary>
        /// Extracts an entry to a file. All directories in the path will be created.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="filePath">The path of the output file to create.</param>
        public void ExtractEntry(ModuleEntry entry, string filePath)
        {
            var directories = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directories))
                Directory.CreateDirectory(directories);
            using (var fileStream = File.Create(filePath))
                ExtractEntry(entry, fileStream);
        }

        /// <summary>
        /// Creates an empty module file.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <returns>The created module.</returns>
        public static AusarModule Create(Stream stream)
        {
            // NOTE: Untested
            var header = new ModuleFileHeaderStruct();
            var writer = new BinaryWriter(stream, Encoding.UTF8, /* leaveOpen */ true);
            header.Write(writer);
            return new AusarModule(stream, Enumerable.Empty<ModuleEntry>(), Enumerable.Empty<ModuleEntry>());
        }

        /// <summary>
        /// Opens an existing module file.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The opened module.</returns>
        public static AusarModule Open(Stream stream)
        {
            var reader = new BinaryReader(stream, Encoding.UTF8, /* leaveOpen */ true);
            var moduleStruct = new ModuleStruct();
            moduleStruct.Read(reader);
            var entries = moduleStruct.Entries.Select(e => new ModuleEntry(e, moduleStruct)).ToList();
            var resources = moduleStruct.ResourceEntries.Select(i => entries[i]);
            return new AusarModule(stream, entries, resources);
        }
    }
}
