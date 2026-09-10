using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace test.Models
{
    public static class MakePassword
    {
        public static void createPasswordHash(string password,out string passwordHash,out string passwordSalt)
        {
            using (var x =  new HMACSHA512())
            {
                passwordSalt = Convert.ToBase64String(x.Key);
                byte[] hashBytes = x.ComputeHash(Encoding.UTF8.GetBytes(password));
                passwordHash = Convert.ToBase64String(hashBytes);
            }
        }

        public static bool VerifyPasswordHash(string password,string storedHash, string storedSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);
            byte[] storedHashBytes = Convert.FromBase64String(storedHash);

            using( var x = new HMACSHA512(saltBytes))
            {
                byte[] computedHashBytes = x.ComputeHash(Encoding.UTF8.GetBytes(password));
                for(int i = 0;i < computedHashBytes.Length; i++)
                {
                    if (computedHashBytes[i] != storedHashBytes[i])
                        return false;
                }
            }
            return true;
        }
    }
}