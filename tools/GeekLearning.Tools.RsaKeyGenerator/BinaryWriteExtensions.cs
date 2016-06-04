using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GeekLearning.Tools.RsaKeyGenerator
{
    public static class BinaryWriterExtensions
    {
        public static void WriteLengthPrefixedBuffer(this BinaryWriter writer, byte[] buffer)
        {
            writer.Write(buffer.Length);
            writer.Write(buffer);
        }
    }
}
