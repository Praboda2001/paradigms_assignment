// Student Name: P.H. Thushan Prabodha
// Student ID: 10705662
// Date: 23/09/2026

namespace StudentRecordSystem.Services;

// This remembers who is currently logged in. It is registered as Scoped in
// Program.cs, so each visitor's browser connection gets its own instance
// and nobody sees another visitor's login.
public class SessionService
{
    // This is the signed-in Admin or Student, or null when nobody is signed in.
    public Models.User? CurrentUser { get; set; }
}
