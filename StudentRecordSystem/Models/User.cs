// Student Name: P.H. Thushan Prabodha
// Student ID: 10705662
// Date: 23/09/2026

namespace StudentRecordSystem.Models;

// We use abstract base class for anyone who can log in.
// This demonstrates encapsulation (private filed + validated property) and
// also this is the shared base that Admin and Student inherit from.)
public abstract class User
{
    private string _username = string.Empty;

    public int AccountId { get; init; }

    public string _username
    {
        get => _username;
        set => _username = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("Username cannot be empty.", nameof(value))
            : value;
    }

    // Abstract method - each subclass provides its own version (polymorphism).
    public abstract string GetRoleDescription();

}