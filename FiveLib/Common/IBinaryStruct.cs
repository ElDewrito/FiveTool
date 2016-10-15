using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLib.Common
{
    /// <summary>
    /// An object that has a guaranteed size and expected alignment when serialized into binary form.
    /// </summary>
    public interface IBinaryStruct
    {
        ulong GetStructSize();

        ulong GetStructAlignment();
    }
}
