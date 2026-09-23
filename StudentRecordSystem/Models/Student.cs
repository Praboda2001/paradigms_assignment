// Student Name: P.H. Thushan Prabodha
// Student ID: 10705662
// Date: 23/09/2026

namespace StudentRecordSystem.Models;

// This is a student account, we are going to linked to one StundentRecord it can view.
public class Student : User
{
    public int StudentRecordId { get; init; }
    
    public override string GetRoleDescription() => "Student";
}