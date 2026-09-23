namespace StudentRecordSystem.Services;

// Keeps track of who's currently logged in. Registered as Scoped, so
// each visitor's browser connection gets its own instance — nobody
// sees another visitor's login.
public class SessionService
{
	public Models.User? CurrentUser { get; set; }
}