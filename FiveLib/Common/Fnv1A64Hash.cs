using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FiveLib.Common
{
    /// <summary>
    /// HashAlgorithm implementation for 64-bit FNV-1a.
    /// </summary>
    /// <seealso cref="System.Security.Cryptography.HashAlgorithm" />
    public class Fnv1A64Hash : HashAlgorithm
    {
        private const ulong FnvPrime = 1099511628211;
        private const ulong FnvOffsetBasis = 14695981039346656037;

        private ulong _hash = FnvOffsetBasis;

        public static ulong ComputeValue(byte[] buffer)
        {
            return ComputeValue(buffer, 0, buffer.Length);
        }

        public static ulong ComputeValue(byte[] buffer, int offset, int count)
        {
            var hash = new Fnv1A64Hash();
            hash.TransformBlock(buffer, offset, count, buffer, offset);
            return hash._hash;
        }

        public override void Initialize()
        {
            _hash = FnvOffsetBasis;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            for (var i = 0; i < cbSize; i++)
            {
                _hash ^= array[ibStart + i];
                _hash *= FnvPrime;
            }
        }

        protected override byte[] HashFinal()
        {
            // Always produce a big-endian hash
            var hashValue = _hash;
            var result = new byte[8];
            for (var i = 7; i >= 0; i--)
            {
                result[i] = (byte)(hashValue & 0xFF);
                hashValue >>= 8;
            }
            return result;
        }
    }
}
