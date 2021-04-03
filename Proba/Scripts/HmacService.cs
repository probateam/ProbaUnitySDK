using System;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;

namespace Proba.Scripts
{
    internal class HmacService
    {
        internal string GenerateHmacSignature(string secretKey, string message)
        {
            var secretBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            using (var hmacsha256 = new HMACSHA256(secretBytes))
            {
                var hashedMessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashedMessage);
            }
        }
    }
}
