// Note: this is called AusarModule instead of just Module because there's already a .NET class called Module.
// Other class names shouldn't really begin with "Ausar".

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Ausar.Module.Structures;
using FiveLib.IO;

namespace FiveLib.Ausar.Module
{
    /// <summary>
    /// An Ausar module file.
    /// </summary>
    public class AusarModule
    {
        private readonly List<ModuleEntry> _entries = new List<ModuleEntry>();

        private readonly Dictionary<string, ModuleEntry> _entriesByName; 
        private readonly Dictionary<uint, ModuleEntry> _entriesByGlobalTagId; 

        private AusarModule(long dataBaseOffset, ModuleFileHeaderStruct fileHeader, IEnumerable<ModuleEntry> entries)
        {
            Id = fileHeader.Id;
            LoadedTagCount = fileHeader.LoadedTagCount;
            BuildVersionId = fileHeader.BuildVersionId;
            _entries.AddRange(entries);
            Entries = _entries.AsReadOnly();
            DataBaseOffset = dataBaseOffset;

            _entriesByName = _entries.ToDictionary(e => e.Name, e => e);
            _entriesByGlobalTagId = _entries
                .Where(e => e.GlobalId != 0xFFFFFFFF)
                .ToDictionary(e => e.GlobalId, e => e);
        }

        /// <summary>
        /// Gets the module's unique ID number.
        /// </summary>
        public ulong Id { get; }

        /// <summary>
        /// Gets the number of tags in the module's loadmanifest.
        /// </summary>
        public int LoadedTagCount { get; }

        public ulong BuildVersionId { get; }

        /// <summary>
        /// Gets the entries for files stored in the module. Read-only.
        /// </summary>
        public IList<ModuleEntry> Entries { get; }

        /// <summary>
        /// Gets the file offset where compressed data begins.
        /// </summary>
        public long DataBaseOffset { get; }

        /// <summary>
        /// Checks if a <see cref="ModuleEntry"/> belongs to this module.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns><c>true</c> if the entry belongs to this module.</returns>
        public bool ContainsEntry(ModuleEntry entry)
        {
            if (entry.Index < 0 || entry.Index >= _entries.Count)
                return false;
            return _entries[entry.Index] == entry;
        }

        /// <summary>
        /// Checks if this module contains an entry with a given name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if this module contains an entry with the given name.</returns>
        public bool ContainsEntry(string name)
        {
            ModuleEntry entry;
            return _entriesByName.TryGetValue(name, out entry);
        }

        /// <summary>
        /// Checks if this module contains an entry with a given global ID.
        /// </summary>
        /// <param name="globalId">The global ID.</param>
        /// <returns><c>true</c> if this module contains an entry with the given global ID.</returns>
        public bool ContainsEntry(uint globalId)
        {
            if (globalId == 0xFFFFFFFF)
                return false;
            ModuleEntry entry;
            return _entriesByGlobalTagId.TryGetValue(globalId, out entry);
        }

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
        public bool GetEntryByGlobalTagId(uint id, out ModuleEntry entry)
        {
            return _entriesByGlobalTagId.TryGetValue(id, out entry);
        }

        /// <summary>
        /// Opens a stream on an entry.
        /// </summary>
        /// <param name="moduleStream">A stream open on the module file.</param>
        /// <param name="entry">The entry.</param>
        /// <param name="section">The section to open.</param>
        /// <returns>A stream which can be used to read the entry data. It will depend on <paramref name="moduleStream"/>.</returns>
        public ModuleBlockStream OpenEntry(Stream moduleStream, ModuleEntry entry, ModuleEntrySection section)
        {
            AssertContainsEntry(entry);
            var blockCompressor = new ModuleBlockCompressor(moduleStream, DataBaseOffset);
            return new ModuleBlockStream(blockCompressor, entry, section);
        }

        /// <summary>
        /// Extracts all or part of an entry to a stream.
        /// </summary>
        /// <param name="moduleStream">A stream open on the module file.</param>
        /// <param name="entry">The entry.</param>
        /// <param name="section">The section to extract.</param>
        /// <param name="outStream">The stream to extract to.</param>
        public void ExtractEntry(Stream moduleStream, ModuleEntry entry, ModuleEntrySection section, Stream outStream)
        {
            AssertContainsEntry(entry);
            using (var entryStream = OpenEntry(moduleStream, entry, section))
                entryStream.CopyTo(outStream);
        }

        /// <summary>
        /// Extracts an entry to a file. All directories in the path will be created.
        /// </summary>
        /// <param name="moduleStream">A stream open on the module file.</param>
        /// <param name="entry">The entry.</param>
        /// <param name="section">The section to extract.</param>
        /// <param name="filePath">The path of the output file to create.</param>
        public void ExtractEntry(Stream moduleStream, ModuleEntry entry, ModuleEntrySection section, string filePath)
        {
            AssertContainsEntry(entry);
            var directories = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directories))
                Directory.CreateDirectory(directories);
            using (var fileStream = File.Create(filePath))
                ExtractEntry(moduleStream, entry, section, fileStream);
        }

        /// <summary>
        /// Creates an empty module file.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <returns>The created module.</returns>
        public static AusarModule Create(Stream stream)
        {
            // NOTE: Untested
            var moduleStruct = new ModuleStruct();
            BinarySerializer.Serialize(stream, moduleStruct);
            var dataBaseOffset = stream.Position;
            return new AusarModule(dataBaseOffset, moduleStruct.FileHeader, Enumerable.Empty<ModuleEntry>());
        }

        /// <summary>
        /// Opens an existing module file.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The opened module.</returns>
        public static AusarModule Open(Stream stream)
        {
            var moduleStruct = BinarySerializer.Deserialize<ModuleStruct>(stream);
            var entries = moduleStruct.Entries.Select((e, i) => new ModuleEntry(i, e, moduleStruct)).ToList();
            var resources = moduleStruct.ResourceEntries.Select(i => entries[i]).ToList();
            for (var i = 0; i < moduleStruct.Entries.Length; i++)
                entries[i].BuildResourceList(moduleStruct.Entries[i], resources);
            var dataBaseOffset = stream.Position;
            return new AusarModule(dataBaseOffset, moduleStruct.FileHeader, entries);
        }

        /// <summary>
        /// Reads the ID number from a module file without fully loading it.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The module's ID number.</returns>
        public static ulong ReadId(Stream stream)
        {
            var header = BinarySerializer.Deserialize<ModuleFileHeaderStruct>(stream);
            return header.Id;
        }

        private void AssertContainsEntry(ModuleEntry entry)
        {
            if (!ContainsEntry(entry))
                throw new ArgumentException("The entry does not belong to the module");
        }
    }
}
