// Student Name: P.H. Thushan Prabodha
// Student ID: 10705662
// Date: 23/09/2026

namespace StudentRecordSystem.Models;

// We use abstract base class for anyone who can log in.
// This demonstrates encapsulation (private filed + validated property) and
// also this is the shared base that Admin and Student inherit from.)
public abstract class User
{
    // This is the Backing field for the validated Username property.
    private string _username = string.Empty;

    // This is the account's database ID, set once when the user object is created.
    public int AccountId { get; init; }

    // This is the login username, which must be unique and cannot be empty. 
    public string Username
    {
        // This returns the stored username.
        get => _username;
        // This rejects an empty/whitespace-only username before storing it.
        set => _username = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("Username cannot be empty.", nameof(value))
            : value;
    }

    // Abstract method - each subclass provides its own version (polymorphism).
    public abstract string GetRoleDescription();

}