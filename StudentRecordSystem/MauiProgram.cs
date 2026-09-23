using Microsoft.EntityFrameworkCore;
using StudentRecordSystem.Data;
using StudentRecordSystem.Services;
using StudentRecordSystem.Views;

namespace StudentRecordSystem;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder.UseMauiApp<App>();

		// SQLite database, stored in the app's private data folder.
		string dbPath = Path.Combine(FileSystem.AppDataDirectory, "studentrecords.db3");
		builder.Services.AddDbContext<AppDbContext>(
			options => options.UseSqlite($"Filename={dbPath}"),
			contextLifetime: ServiceLifetime.Singleton,
			optionsLifetime: ServiceLifetime.Singleton);

		// Shared, app-lifetime services.
		builder.Services.AddSingleton<SessionService>();
		builder.Services.AddSingleton<AuthService>();
		builder.Services.AddSingleton<RecordManager>();

		// Pages — new instance each time Shell navigates to one.
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<AdminDashboardPage>();
		builder.Services.AddTransient<StudentEditPage>();
		builder.Services.AddTransient<StudentDashboardPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}