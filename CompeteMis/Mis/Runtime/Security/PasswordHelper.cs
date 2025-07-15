using Compete.Extensions;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Compete.Mis.Runtime.Security
{
    internal static class PasswordHelper
    {
        private static string Encrypt(string password, byte[] salt)
            => Convert.ToBase64String(salt.Merge(/*SHA3_512.HashData*/MD5.HashData(Encoding.UTF8.GetBytes(password).Merge(salt))));
        //{
        //    var plaintext = Encoding.UTF8.GetBytes(password);
        //    for (int i = 0; i < plaintext.Length && i < salt.Length; i++)
        //        plaintext[i] += salt[i];
        //    return Convert.ToBase64String(salt.Merge(SHA512.HashData(plaintext)));
        //}

        public static string Encrypt(string password) => Encrypt(password, Guid.NewGuid().ToByteArray());

        public static bool Verify(string password, string ciphertext) => Encrypt(password, Convert.FromBase64String(ciphertext).Split(0L, 16L)) == ciphertext;
    }
}
