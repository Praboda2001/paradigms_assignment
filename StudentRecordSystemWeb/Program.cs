using Microsoft.EntityFrameworkCore;
using StudentRecordSystem.Data;
using StudentRecordSystem.Services;
using StudentRecordSystemWeb.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// --- Added: database + app services ---------------------------------
// SQLite file stored in the project's own folder (a web app has no
// MAUI-style "app data directory" — this is just a plain local path).
string dbPath = Path.Combine(AppContext.BaseDirectory, "studentrecords.db3");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Filename={dbPath}"));

// Scoped, not Singleton: in a web app, Scoped means one instance per
// visitor's browser connection. Singleton here would mean every visitor
// shares the same login session — clearly wrong for a multi-user site.
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<RecordManager>();
// ----------------------------------------------------------------------

var app = builder.Build();

// --- Added: create the database and seed the admin account on startup ---
using (var scope = app.Services.CreateScope())
{
    var auth = scope.ServiceProvider.GetRequiredService<AuthService>();
    await auth.InitialiseAsync();
}
// --------------------------------------------------------------------------

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();