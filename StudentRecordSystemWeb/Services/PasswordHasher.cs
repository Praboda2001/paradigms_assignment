using System.Security.Cryptography;
using System.Text;

namespace StudentRecordSystem.Services;

// Small helper for hashing passwords so we're never storing them as
// plain text in the database.
public static class PasswordHasher
{
	// Turns a plain-text password into a SHA-256 hash.
	public static string Hash(string plainTextPassword)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(plainTextPassword);
		byte[] hash = SHA256.HashData(bytes);
		return Convert.ToHexString(hash);
	}

	// Checks a plain-text password against a stored hash by hashing it
	// again and comparing the two.
	public static bool Verify(string plainTextPassword, string storedHash) =>
		Hash(plainTextPassword) == storedHash;
}