using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.CommandLine;
using System.IO;

namespace GeekLearning.Tools.RsaKeyGenerator
{
    public class Program
    {
        private static RandomNumberGenerator rng = RandomNumberGenerator.Create();

        public static void Main(string[] args)
        {
            var outputPath = System.IO.Directory.GetCurrentDirectory();
            byte[] password = null;
            byte[] salt = new byte[32];
            string kId = Guid.NewGuid().ToString("N");
            rng.GetBytes(salt);

            ArgumentSyntax.Parse(args, syntax =>
            {
                syntax.DefineOption("o|outputPath", ref outputPath, "The output path where to write new keys");
                syntax.DefineOption("p|password", ref password, str => GetPasswordDerivedBytes(str, salt), "The password to protect the private key");
            });

            if (password == null)
            {
                var passwordBytes = new byte[32];
                rng.GetBytes(passwordBytes);
                var passwordString = Convert.ToBase64String(passwordBytes);
                password = GetPasswordDerivedBytes(passwordString, salt);
                Console.WriteLine($"Generated Password : {passwordString}");
            }

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
            ExportPrivateKey(rsa, kId, outputPath, password, salt);
            ExportPublicKey(rsa, kId, outputPath);
            Console.ReadLine();
        }

        private static byte[] GetPasswordDerivedBytes(string password, byte[] salt)
        {
            var passwordDeriver = new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt);
            return passwordDeriver.GetBytes(32);
        }

        public static void ExportPublicKey(RSACryptoServiceProvider rsa, string kId, string outputPath)
        {
            var rsaParams = rsa.ExportParameters(false);
            var ms = new System.IO.MemoryStream();
            var noBytes = new byte[0];
            System.IO.BinaryWriter w = new System.IO.BinaryWriter(ms);
            w.WriteLengthPrefixedBuffer(rsaParams.D ?? noBytes);
            w.WriteLengthPrefixedBuffer(rsaParams.DP ?? noBytes);
            w.WriteLengthPrefixedBuffer(rsaParams.DQ ?? noBytes);
            w.WriteLengthPrefixedBuffer(rsaParams.Exponent ?? noBytes);
            w.WriteLengthPrefixedBuffer(rsaParams.InverseQ ?? noBytes);
            w.WriteLengthPrefixedBuffer(rsaParams.Modulus ?? noBytes);
            w.WriteLengthPrefixedBuffer(rsaParams.P ?? noBytes);
            w.WriteLengthPrefixedBuffer(rsaParams.Q ?? noBytes);
            w.Flush();
            System.IO.File.WriteAllText(System.IO.Path.Combine(outputPath, kId + ".key.public"), Convert.ToBase64String(ms.ToArray()));
        }

        public static void ExportPrivateKey(RSACryptoServiceProvider rsa, string kId, string outputPath, byte[] password, byte[] salt)
        {
            var rsaParams = rsa.ExportParameters(true);
            var ms = new System.IO.MemoryStream();
            System.IO.BinaryWriter w = new System.IO.BinaryWriter(ms);
            w.WriteLengthPrefixedBuffer(rsaParams.D);
            w.WriteLengthPrefixedBuffer(rsaParams.DP);
            w.WriteLengthPrefixedBuffer(rsaParams.DQ);
            w.WriteLengthPrefixedBuffer(rsaParams.Exponent);
            w.WriteLengthPrefixedBuffer(rsaParams.InverseQ);
            w.WriteLengthPrefixedBuffer(rsaParams.Modulus);
            w.WriteLengthPrefixedBuffer(rsaParams.P);
            w.WriteLengthPrefixedBuffer(rsaParams.Q);
            w.Flush();

            var aes = Aes.Create();
            aes.Key = password;
            aes.GenerateIV();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var transform = aes.CreateEncryptor();
            var encrypted = new MemoryStream();
            encrypted.Write(aes.IV, 0, (int)aes.IV.Length);
            using (CryptoStream stream = new CryptoStream(encrypted, transform, CryptoStreamMode.Write))
            {
                stream.Write(ms.ToArray(), 0, (int)ms.Length);
                stream.FlushFinalBlock();
            }

            System.IO.File.WriteAllText(System.IO.Path.Combine(outputPath, kId + ".key.private"), 
                Convert.ToBase64String(salt) + "." + Convert.ToBase64String(encrypted.ToArray()));
        }
    }
}
