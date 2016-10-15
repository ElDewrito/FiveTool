using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLib.Common
{
    public interface IBinaryWritable
    {
        void Write(BinaryWriter writer);
    }
}
