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
        private readonly byte[] _nullTerminator;

        /// <summary>
        /// Constructs an empty <see cref="StringBlob"/>.
        /// </summary>
        /// <param name="encoding">The encoding to be used for strings in the blob.</param>
        public StringBlob(Encoding encoding)
        {
            _stream = new MemoryStream();
            _encoding = encoding;
            _nullTerminator = _encoding.GetBytes("\0");
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
            _stream.Write(_nullTerminator, 0, _nullTerminator.Length);
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

            // Decode characters until a null terminator is reached
            // We have to decode up to 2 chars at a time in order to handle large code points correctly
            var result = new StringBuilder();
            var buffer = _stream.GetBuffer();
            var decoder = _encoding.GetDecoder();
            var chars = new char[2];
            var maxByteCount = _encoding.GetMaxByteCount(chars.Length);
            while (offset < _stream.Length)
            {
                int bytesUsed, charsUsed;
                bool completed;
                var byteCount = Math.Min((int)(_stream.Length - offset), maxByteCount);
                decoder.Convert(buffer, offset, byteCount, chars, 0, chars.Length, false, out bytesUsed, out charsUsed, out completed);
                for (var i = 0; i < charsUsed; i++)
                {
                    if (chars[i] == '\0')
                        return result.ToString();
                    result.Append(chars[i]);
                }
                offset += bytesUsed;
            }
            return result.ToString();
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
