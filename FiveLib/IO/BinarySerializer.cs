// Inspired by KSoft.IO.EndianStream
// <https://bitbucket.org/KornnerStudios/ksoft/src/bbcac637a5f2/KSoft/IO/EndianStreams/EndianStream.cs>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveLib.Common;
using FiveLib.Memory;

namespace FiveLib.IO
{
    public class BinarySerializer : IDisposable
    {
        private readonly BinaryReader _reader;
        private readonly BinaryWriter _writer;

        public BinarySerializer(BinaryReader reader)
        {
            _reader = reader;
        }

        public BinarySerializer(BinaryWriter writer)
        {
            _writer = writer;
        }

        public static void Serialize<T>(Stream stream, T obj)
            where T : IBinarySerializable
        {
            var writer = new BinaryWriter(stream, Encoding.UTF8, /* leaveOpen */ true);
            Serialize(writer, obj);
        }

        public static void Serialize<T>(BinaryWriter writer, T obj)
            where T : IBinarySerializable
        {
            obj.Serialize(new BinarySerializer(writer));
        }

        public static T Deserialize<T>(Stream stream)
            where T : IBinarySerializable, new()
        {
            var reader = new BinaryReader(stream, Encoding.UTF8, /* leaveOpen */ true);
            return Deserialize<T>(reader);
        }

        public static T Deserialize<T>(BinaryReader reader)
            where T : IBinarySerializable, new()
        {
            var obj = new T();
            obj.Serialize(new BinarySerializer(reader));
            return obj;
        }

        public bool IsReading => _reader != null;

        public bool IsWriting => _writer != null;

        public Stream BaseStream => IsReading ? _reader.BaseStream : _writer.BaseStream;

        public bool Value(ref bool f)
        {
            if (IsReading)
                f = _reader.ReadByte() != 0;
            else
                _writer.Write(f ? (byte)1 : (byte)0);
            return f;
        }

        public sbyte Value(ref sbyte f)
        {
            if (IsReading)
                f = _reader.ReadSByte();
            else
                _writer.Write(f);
            return f;
        }

        public byte Value(ref byte f)
        {
            if (IsReading)
                f = _reader.ReadByte();
            else
                _writer.Write(f);
            return f;
        }

        public short Value(ref short f)
        {
            if (IsReading)
                f = _reader.ReadInt16();
            else
                _writer.Write(f);
            return f;
        }

        public ushort Value(ref ushort f)
        {
            if (IsReading)
                f = _reader.ReadUInt16();
            else
                _writer.Write(f);
            return f;
        }

        public int Value(ref int f)
        {
            if (IsReading)
                f = _reader.ReadInt32();
            else
                _writer.Write(f);
            return f;
        }

        public uint Value(ref uint f)
        {
            if (IsReading)
                f = _reader.ReadUInt32();
            else
                _writer.Write(f);
            return f;
        }

        public long Value(ref long f)
        {
            if (IsReading)
                f = _reader.ReadInt64();
            else
                _writer.Write(f);
            return f;
        }

        public ulong Value(ref ulong f)
        {
            if (IsReading)
                f = _reader.ReadUInt64();
            else
                _writer.Write(f);
            return f;
        }

        public float Value(ref float f)
        {
            if (IsReading)
                f = _reader.ReadSingle();
            else
                _writer.Write(f);
            return f;
        }

        public double Value(ref double f)
        {
            if (IsReading)
                f = _reader.ReadDouble();
            else
                _writer.Write(f);
            return f;
        }

        public MagicNumber Value(ref MagicNumber f)
        {
            if (IsReading)
                f = new MagicNumber(_reader.ReadInt32());
            else
                _writer.Write(f.Value);
            return f;
        }

        public Pointer64<T> Value<T>(ref Pointer64<T> f)
            where T: IBinarySerializable, IFixedSize, new()
        {
            if (IsReading)
                f = new Pointer64<T>(_reader.ReadUInt64());
            else
                _writer.Write(f.Address);
            return f;
        }

        public T Object<T>(ref T f)
            where T : IBinarySerializable, new()
        {
            if (IsReading)
                f = new T();
            f.Serialize(this);
            return f;
        }

        public string AsciiString(ref string f, int bytes)
        {
            if (IsReading)
                f = ReadString(Encoding.ASCII, bytes);
            else
                WriteString(f, Encoding.ASCII, bytes);
            return f;
        }

        public string Utf8String(ref string f, int bytes)
        {
            if (IsReading)
                f = ReadString(Encoding.UTF8, bytes);
            else
                WriteString(f, Encoding.UTF8, bytes);
            return f;
        }

        public string Utf16String(ref string f, int bytes)
        {
            if (IsReading)
                f = ReadString(Encoding.Unicode, bytes);
            else
                WriteString(f, Encoding.Unicode, bytes);
            return f;
        }

        public T Enum<T>(ref T f)
            where T : struct
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new InvalidOperationException($"{type} is not an enum type");
            var underlyingType = type.GetEnumUnderlyingType();
            var typeCode = Type.GetTypeCode(underlyingType);
            if (IsReading)
                f = (T)ReadEnum(typeCode);
            else
                WriteEnum(f, typeCode);
            return f;
        }

        private object ReadEnum(TypeCode type)
        {
            switch (type)
            {
                case TypeCode.SByte: return _reader.ReadSByte();
                case TypeCode.Byte: return _reader.ReadByte();
                case TypeCode.Int16: return _reader.ReadInt16();
                case TypeCode.UInt16: return _reader.ReadUInt16();
                case TypeCode.Int32: return _reader.ReadInt32();
                case TypeCode.UInt32: return _reader.ReadUInt32();
                case TypeCode.Int64: return _reader.ReadInt64();
                case TypeCode.UInt64: return _reader.ReadUInt64();
                default:
                    throw new InvalidOperationException($"Unsupported underlying enum type {type}");
            }
        }

        private void WriteEnum(object val, TypeCode type)
        {
            switch (type)
            {
                case TypeCode.SByte: _writer.Write((sbyte)val); break;
                case TypeCode.Byte: _writer.Write((byte)val); break;
                case TypeCode.Int16: _writer.Write((short)val); break;
                case TypeCode.UInt16: _writer.Write((ushort)val); break;
                case TypeCode.Int32: _writer.Write((int)val); break;
                case TypeCode.UInt32: _writer.Write((uint)val); break;
                case TypeCode.Int64: _writer.Write((long)val); break;
                case TypeCode.UInt64: _writer.Write((ulong)val); break;
                default:
                    throw new InvalidOperationException($"Unsupported underlying enum type {type}");
            }
        }

        public byte[] Array(ref byte[] f, int count)
        {
            if (IsReading)
            {
                f = new byte[count];
                _reader.BaseStream.Read(f, 0, f.Length);
            }
            else
            {
                CheckArrayLength(f, count);
                _writer.Write(f);
            }
            return f;
        }

        public T[] Array<T>(ref T[] f, int count)
            where T : IBinarySerializable, new()
        {
            if (IsReading)
                f = new T[count];
            CheckArrayLength(f, count);
            for (var i = 0; i < f.Length; i++)
                Object(ref f[i]);
            return f;
        }

        public T[] Array<T>(ref T[] f, int count, Func<BinarySerializer, int, T, T> serializeFunc)
        {
            if (IsReading)
                f = new T[count];
            CheckArrayLength(f, count);
            for (var i = 0; i < f.Length; i++)
            {
                var v = serializeFunc(this, i, f[i]);
                if (IsReading)
                    f[i] = v;
            }
            return f;
        }

        public StringBlob StringTable(ref StringBlob f, Encoding encoding, int size)
        {
            if (IsReading)
            {
                f = new StringBlob(encoding, _reader.ReadBytes(size));
            }
            else
            {
                if (f.Length != size)
                    throw new ArgumentException($"String blob must be {size} bytes large, got {f.Length}");
            }
            return f;
        }

        public T Custom<T>(ref T f, Func<BinaryReader, T> readFunc, Action<BinaryWriter, T> writeFunc)
        {
            if (IsReading)
                f = readFunc(_reader);
            else
                writeFunc(_writer, f);
            return f;
        }

        public void Expect<T>(Func<BinarySerializer, T> serializeFunc, string name, T expectedValue)
        {
            var actualValue = serializeFunc(this);
            if (!actualValue.Equals(expectedValue))
                throw new InvalidDataException($"Unexpected {name}: got \"{actualValue}\", but expected \"{expectedValue}\"");
        }

        public void Expect<T>(Func<BinarySerializer, T> serializeFunc, string name, Predicate<T> predicate)
        {
            var actualValue = serializeFunc(this);
            if (!predicate(actualValue))
                throw new InvalidDataException($"Unexpected {name}: \"{actualValue}\"");
        }

        public void Padding(int count)
        {
            if (IsReading)
                BaseStream.Seek(count, SeekOrigin.Current);
            else
                StreamUtil.Fill(_writer.BaseStream, 0, count);
        }

        private string ReadString(Encoding encoding, int size)
        {
            var bytes = _reader.ReadBytes(size);
            var chars = encoding.GetChars(bytes);
            for (var i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '\0')
                {
                    var terminatedChars = new char[i];
                    Buffer.BlockCopy(chars, 0, terminatedChars, 0, i * sizeof(char));
                    return new string(terminatedChars);
                }
            }
            return new string(chars);
        }

        private void WriteString(string str, Encoding encoding, int size)
        {
            var bytes = encoding.GetBytes(str);
            _writer.Write(bytes, 0, Math.Min(size - 1, bytes.Length));
            _writer.Write(encoding.GetBytes("\0"));
            if (size > bytes.Length + 1)
                Padding(size - bytes.Length - 1);
        }

        private static void CheckArrayLength(Array array, int expectedLength)
        {
            if (array.Length != expectedLength)
                throw new ArgumentException($"Array must contain {expectedLength} elements, got {array.Length}");
        }

        public void Dispose()
        {
            _reader?.Dispose();
            _writer?.Dispose();
        }
    }
}
