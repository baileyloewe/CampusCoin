using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using CampusCoin.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusCoin.Services;

public class SaltHashService
{

    /// <summary> Generates a salt for hashing a password </summary>
    /// <param></param>
    /// <returns> A salt value in base64 string format</returns>
    public static string GenerateSalt()
    {
        byte[] salt = new byte[32];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return Convert.ToBase64String(salt);
    }

    /// <summary> Hashes a password with the salt </summary>
    /// <param name="password">The password to be hashed</param>
    /// <param name="salt">The salt to use for hashing the password</param>
    /// <returns> The hashed password in base64 string format</returns>
    public static string HashPassword(string password, string salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), 10000, HashAlgorithmName.SHA256);
        return Convert.ToBase64String(pbkdf2.GetBytes(32));
    }
}

