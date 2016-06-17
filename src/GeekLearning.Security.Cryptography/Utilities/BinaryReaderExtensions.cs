namespace GeekLearning.Security.Cryptography.Utilities
{
    using System.IO;

    public static class BinaryReaderExtensions
    {
        public static byte[] ReadLengthPrefixedBuffer(this BinaryReader reader)
        {
            return reader.ReadBytes(reader.ReadInt32());
        }
    }
}
