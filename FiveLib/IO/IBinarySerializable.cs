using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLib.IO
{
    public interface IBinarySerializable
    {
        void Serialize(BinarySerializer s);
    }
}
