// Student Name: P.H. Thushan Prabodha
// Student ID: 10705662
// Date: 23/09/2026

using System.Security.Cryptography;
using System.Text;

namespace StudentRecordSystem.Services;

// This is a small helper for hashing passwords so they are never stored as
// plain text in the database. It is a static class because it holds no
// state, so there is nothing to create an instance of.
// Limitation: SHA-256 without a salt is fast to attack. A salted slow hash
// such as Argon2id is the better choice for a real system.
public static class PasswordHasher
{
    // This turns a plain-text password into a SHA-256 hash written as hex text.
    public static string Hash(string plainTextPassword)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(plainTextPassword);
        byte[] hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }

    // This hashes the typed password again and compares it with the stored
    // hash. The method body is a single expression so it uses =>.
    public static bool Verify(string plainTextPassword, string storedHash) =>
        Hash(plainTextPassword) == storedHash;
}
