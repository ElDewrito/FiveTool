using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Common;
using Ionic.Zlib;

namespace FiveLib.Ausar.Module
{
    public class ModuleBlockCompressor
    {
        private readonly Stream _stream;
        private readonly long _dataAreaOffset;

        public ModuleBlockCompressor(Stream moduleStream, long dataAreaOffset)
        {
            if (!moduleStream.CanRead)
                throw new ArgumentException("Stream must support reading", nameof(moduleStream));
            _stream = moduleStream;
            _dataAreaOffset = dataAreaOffset;
        }

        public void ReadBlock(long entryOffset, ModuleDataBlock block, Stream outStream)
        {
            _stream.Position = _dataAreaOffset + entryOffset + block.CompressedOffset;
            if (block.IsCompressed)
                Inflate(_stream, block.CompressedSize, block.UncompressedSize, outStream);
            else
                StreamUtil.Copy(_stream, outStream, block.UncompressedSize);
        }

        private static void Inflate(Stream inStream, long compressedSize, long uncompressedSize, Stream outStream)
        {
            var compressedData = new byte[compressedSize];
            inStream.Read(compressedData, 0, (int)compressedSize);
            using (var inflate = new ZlibStream(new MemoryStream(compressedData), CompressionMode.Decompress))
                StreamUtil.Copy(inflate, outStream, uncompressedSize);
        }
    }
}
