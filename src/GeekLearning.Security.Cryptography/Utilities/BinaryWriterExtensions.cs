namespace GeekLearning.Security.Cryptography.Utilities
{
    using System.IO;

    public static class BinaryWriterExtensions
    {
        public static void WriteLengthPrefixedBuffer(this BinaryWriter writer, byte[] buffer)
        {
            writer.Write(buffer.Length);
            writer.Write(buffer);
        }
    }
}
