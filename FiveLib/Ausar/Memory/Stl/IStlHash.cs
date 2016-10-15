using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLib.Ausar.Memory.Stl
{
    public interface IStlHash<in TKey>
    {
        ulong Hash(TKey key);
    }
}
