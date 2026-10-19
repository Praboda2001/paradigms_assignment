// Student Name: P.H. Thushan Prabodha
// Student ID: 10705662
// Date: 23/09/2026

using Microsoft.EntityFrameworkCore;
using StudentRecordSystem.Data;
using StudentRecordSystem.Services;
using StudentRecordSystemWeb.Components;

// This is the entry point of the web application. It registers the services
// the pages need, prepares the database and then starts the web server.
var builder = WebApplication.CreateBuilder(args);

// Add the Razor component system and let components run on the server so
// that button clicks are handled in C# instead of JavaScript.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ---- Database and application services ----------------------------------

// The SQLite database is one file stored next to the compiled application.
string dbPath = Path.Combine(AppContext.BaseDirectory, "studentrecords.db3");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Filename={dbPath}"));

// The services below are Scoped, which means one instance per visitor's
// browser connection. A Singleton would make every visitor share the same
// login session, which would be wrong for a multi-user site.
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<RecordManager>();

var app = builder.Build();

// ---- Start-up work: create the database and add the starting data -------

// A scope is needed because the services above are Scoped and there is no
// visitor yet at start-up.
using (var scope = app.Services.CreateScope())
{
    // Creates the database file and the default admin account if they do not exist.
    var auth = scope.ServiceProvider.GetRequiredService<AuthService>();
    await auth.InitialiseAsync();

    // Adds a few demo students the first time the app runs with an empty database.
    var records = scope.ServiceProvider.GetRequiredService<RecordManager>();
    await records.SeedDemoDataAsync();
}

// ---- HTTP request pipeline ----------------------------------------------

if (!app.Environment.IsDevelopment())
{
    // Show a friendly error page instead of technical details in production.
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // HSTS tells browsers to use HTTPS only. See https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

// Protects form posts against cross-site request forgery.
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();