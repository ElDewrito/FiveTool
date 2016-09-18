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
        private readonly List<ModuleEntry> _entries = new List<ModuleEntry>();
        private readonly List<ModuleEntry> _resources = new List<ModuleEntry>();

        private readonly Dictionary<string, ModuleEntry> _entriesByName; 
        private readonly Dictionary<int, ModuleEntry> _entriesByGlobalTagId; 

        private AusarModule(Stream stream, IEnumerable<ModuleEntry> entries, IEnumerable<ModuleEntry> resources)
        {
            _stream = stream;
            DataBaseOffset = stream.Position;
            _entries.AddRange(entries);
            _resources.AddRange(resources);
            _entriesByName = _entries.ToDictionary(e => e.Name, e => e);
            _entriesByGlobalTagId = _entries
                .Where(e => e.GlobalTagId != -1)
                .ToDictionary(e => e.GlobalTagId, e => e);
        }

        /// <summary>
        /// Gets the entries for files stored in the module.
        /// </summary>
        public IReadOnlyList<ModuleEntry> Entries => _entries;

        /// <summary>
        /// Gets the entries for resource files stored in the module.
        /// These entries can also be found in the main <see cref="Entries"/> list.
        /// </summary>
        public IReadOnlyList<ModuleEntry> Resources => _resources;

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
        public bool FindEntryByName(string name, out ModuleEntry entry)
        {
            return _entriesByName.TryGetValue(name, out entry);
        }

        /// <summary>
        /// Looks up an entry by its global tag ID.
        /// </summary>
        /// <param name="id">The global tag ID to search for.</param>
        /// <param name="entry">The result variable.</param>
        /// <returns><c>true</c> if the entry was found.</returns>
        public bool FindEntryByGlobalTagId(int id, out ModuleEntry entry)
        {
            return _entriesByGlobalTagId.TryGetValue(id, out entry);
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
