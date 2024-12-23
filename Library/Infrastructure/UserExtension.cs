﻿using System.Security.Cryptography;
using System.Text;

namespace Library.Infrastructure
{
    public static class UserExtension
    {
        public static String HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                string hashedPassword = Convert.ToBase64String(hashBytes);

                return hashedPassword;
            }
        }
    }
}
