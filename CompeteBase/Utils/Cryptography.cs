using Compete.Extensions;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Compete.Utils
{
    public static class Cryptography
    {
        public static byte[]? PublicKey { get; set; }

        private static readonly byte[] key = [241, 251, 197, 239, 193, 229, 227, 241, 199, 211, 223, 233, 241, 229, 223, 199, 193, 233, 229, 229, 199, 223, 193, 233, 197, 193, 197, 211, 241, 197, 233, 229];

        private static byte[] Decrypt(byte[] ciphertext)
        {
            var ciphertextBytes = ciphertext.Skip(16).ToArray(); // Skip the first 16 bytes which are the IV

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = ciphertext.Split(0, 16);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                    return decryptor.TransformFinalBlock(ciphertextBytes, 0, ciphertextBytes.Length);
            }
        }

        public static byte[] SymmetricEncrypt(byte[] ciphertext)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.GenerateIV();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                    return aes.IV.Concat(encryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length)).ToArray(); // Prepend IV to the encrypted data
            }
        }

        public static byte[] Encryption(byte[] ciphertext)
        {
            //RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            //provider.ImportFromEncryptedPem(Encoding.UTF8.GetString(Decrypt(PublicKey!)));
            //provider.Encrypt(ciphertext, RSAEncryptionPadding.OaepSHA3_512);

            var rsa = RSA.Create();
            rsa.ImportFromPem(Encoding.UTF8.GetString(Decrypt(PublicKey!)));
            return rsa.Encrypt(ciphertext, RSAEncryptionPadding.OaepSHA3_512);//OaepSHA256  OaepSHA3_512
        }
    }
}
