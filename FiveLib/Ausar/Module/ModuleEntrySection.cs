using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLib.Ausar.Module
{
    public enum ModuleEntrySection
    {
        Header,
        TagData,
        ResourceData,

        /// <summary>
        /// Includes every section in the entry.
        /// For raw files, this is the only valid section that can be used.
        /// For everything else, using this section will open the entry as read-only.
        /// </summary>
        All,
    }
}
