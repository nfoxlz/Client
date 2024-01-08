using Compete.Extensions;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Compete.Mis.Runtime.Security
{
    internal static class PasswordHelper
    {
        private static string Encrypt(string password, byte[] salt) => Convert.ToBase64String(salt.Merge(MD5.HashData(Encoding.UTF8.GetBytes(password).Merge(salt))));

        public static string Encrypt(string password) => Encrypt(password, Guid.NewGuid().ToByteArray());

        public static bool Verify(string password, string ciphertext) => Encrypt(password, Convert.FromBase64String(ciphertext).Split(0L, 16L)) == ciphertext;
    }
}
