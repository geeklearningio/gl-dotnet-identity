namespace GeekLearning.Security.Cryptography.Utilities
{
    using System.IO;

    public static class BinaryReaderExtensions
    {
        public static byte[] ReadLengthPrefixedBuffer(this BinaryReader reader)
        {
            return reader.ReadBytes(reader.ReadInt32());
        }

        public static byte[] ReadLengthPrefixedBufferNullIfEmpty(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(reader.ReadInt32());
            return bytes.Length > 0 ? bytes : null;
        }
    }
}
