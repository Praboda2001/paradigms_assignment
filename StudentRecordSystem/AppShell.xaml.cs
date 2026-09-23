namespace StudentRecordSystem;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		// Register every page reachable by navigating away from login.
		Routing.RegisterRoute(nameof(Views.AdminDashboardPage), typeof(Views.AdminDashboardPage));
		Routing.RegisterRoute(nameof(Views.StudentEditPage), typeof(Views.StudentEditPage));
		Routing.RegisterRoute(nameof(Views.StudentDashboardPage), typeof(Views.StudentDashboardPage));
	}
}