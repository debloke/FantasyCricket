using System;
using System.Security.Cryptography;

namespace FantasyCricket.Namak
{
    public class NamakGenerator
    {

        public static string Get(string pwd)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(pwd, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] namak = new byte[36];

            Array.Copy(salt, 0, namak, 0, 16);
            Array.Copy(hash, 0, namak, 16, 20);

            return Convert.ToBase64String(namak);
        }

        public static bool Verify(string namak, string pwd)
        {
            byte[] namakByteArr = Convert.FromBase64String(namak);
            byte[] salt = new byte[16];

            Array.Copy(namakByteArr, 0, salt, 0, 16);

            var pbkdf2 = new Rfc2898DeriveBytes(pwd, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            for (int i = 0; i < 20; i++)
            {
                if (namakByteArr[i + 16] != hash[i])
                {
                    return false;
                }
            }
            return true;


        }
    }
}
