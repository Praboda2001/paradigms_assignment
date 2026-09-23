// Student Name: P.H. Thushan Prabodha
// Student ID: 10705662
// Date: 23/09/2026

namespace StudentRecordSystem.Models;

// This is a staff acount.
// Admins can add/ update/ remove student recoeds through recordManager.
public class Admin : User
{
    public override string GetRoleDescription() => "Administrator";
}