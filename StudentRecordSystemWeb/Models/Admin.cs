// Student Name: P.H. Thushan Prabodha
// Student ID: 10705662
// Date: 23/09/2026

namespace StudentRecordSystem.Models;

// This is a staff account.
// Admins can add/ update/ remove student records through RecordManager.
public class Admin : User
{
    // This returns "Administrator" for this role.
    public override string GetRoleDescription() => "Administrator";
}
