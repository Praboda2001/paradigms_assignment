namespace StudentRecordSystem;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	// Creates the app's one window, showing AppShell (which starts on LoginPage).
	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell()) { Title = "Student Record System" };
	}
}