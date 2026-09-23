// Student Name: P.H. Thushan Prabodha
// Student ID: 10705662
// Date: 23/09/2026

namespace StudentRecordSystem.Exceptions;

// We use this when a user attempts to log in with incorrect credentials.
public class InvalidLoginException : Exception
{
    // We use this to send a fixed, generic message.
    // But doesn't say whether the username or password was wrong,
    // since that would help an attacker guess.
    public InvalidLoginException()
        : base("Incorrect username or password. Please try again.")
    {
    }
}