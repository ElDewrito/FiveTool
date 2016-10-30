using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveLib.Common
{
    /// <summary>
    /// A blob of binary data containing null-terminated strings.
    /// </summary>
    public class StringBlob
    {
        private readonly MemoryStream _stream;
        private readonly Encoding _encoding;
        private readonly byte[] _terminator;

        /// <summary>
        /// Constructs an empty <see cref="StringBlob"/>.
        /// </summary>
        /// <param name="encoding">The encoding to be used for strings in the blob.</param>
        public StringBlob(Encoding encoding)
        {
            _stream = new MemoryStream();
            _encoding = encoding;
            _terminator = _encoding.GetBytes("\0");
            if (_terminator.Length != 4 && _terminator.Length != 2 && _terminator.Length != 1)
                throw new ArgumentException("Unsupported encoding");
        }

        /// <summary>
        /// Constructs a <see cref="StringBlob"/> initialized with a blob of binary data.
        /// </summary>
        /// <param name="encoding">The encoding used for strings in the blob.</param>
        /// <param name="data">The data to initialize the blob with.</param>
        public StringBlob(Encoding encoding, byte[] data)
            : this(encoding)
        {
            _stream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Gets the current length of the blob in bytes.
        /// </summary>
        public int Length => (int)_stream.Length;

        /// <summary>
        /// Adds a string followed by a null terminator to the end of the blob.
        /// </summary>
        /// <param name="str">The string to add.</param>
        /// <returns>The offset of the first character of the string.</returns>
        public int AddString(string str)
        {
            var offset = (int)_stream.Position;
            var bytes = _encoding.GetBytes(str);
            _stream.Write(bytes, 0, bytes.Length);
            _stream.Write(_terminator, 0, _terminator.Length);
            return offset;
        }

        /// <summary>
        /// Gets the null-terminated string at an offset.
        /// </summary>
        /// <param name="offset">The offset of the string within the blob.</param>
        /// <returns>The string.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the offset is outside of the blob.</exception>
        public string GetStringAtOffset(int offset)
        {
            if (offset < 0 || offset >= Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            var buffer = _stream.GetBuffer();
            switch (_terminator.Length)
            {
                case 1: return GetStringAtOffset8(buffer, offset, (int)_stream.Length, _encoding);
                case 2: return GetStringAtOffset16(buffer, offset, (int)_stream.Length & ~2, _encoding);
                case 4: return GetStringAtOffset32(buffer, offset, (int)_stream.Length & ~4, _encoding);
                default: throw new InvalidOperationException("Unsupported encoding");
            }
        }

        private static string GetStringAtOffset8(byte[] buffer, int offset, int maxLength, Encoding encoding)
        {
            int i;
            for (i = offset; buffer[i] != 0 && i < maxLength; i++) ;
            return encoding.GetString(buffer, offset, i - offset);
        }

        private static string GetStringAtOffset16(byte[] buffer, int offset, int maxLength, Encoding encoding)
        {
            int i;
            for (i = offset; BitConverter.ToInt16(buffer, i) != 0 && i < maxLength; i += 2) ;
            return encoding.GetString(buffer, offset, i - offset);
        }

        private static string GetStringAtOffset32(byte[] buffer, int offset, int maxLength, Encoding encoding)
        {
            int i;
            for (i = offset; BitConverter.ToInt32(buffer, i) != 0 && i < maxLength; i += 4) ;
            return encoding.GetString(buffer, offset, i - offset);
        }

        /// <summary>
        /// Gets a copy of the bytes in the blob.
        /// </summary>
        public byte[] GetBytes()
        {
            var result = new byte[Length];
            Buffer.BlockCopy(_stream.GetBuffer(), 0, result, 0, Length);
            return result;
        }

        /// <summary>
        /// Writes the contents of the blob to a stream.
        /// </summary>
        /// <param name="outStream">The stream to write to.</param>
        public void WriteTo(Stream outStream)
        {
            outStream.Write(_stream.GetBuffer(), 0, Length);
        }
    }
}
