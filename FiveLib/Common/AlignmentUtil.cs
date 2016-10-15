using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLib.Common
{
    public static class AlignmentUtil
    {
        public static ulong Align(ulong val, ulong alignment)
        {
            return (val + alignment - 1) & ~(alignment - 1);
        }
    }
}
