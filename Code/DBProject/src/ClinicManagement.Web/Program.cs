using ClinicManagement.Application.Extensions;
using ClinicManagement.Infrastructure.Extensions;

// Configure Npgsql to use legacy timestamp behavior for DateTime compatibility
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages();

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Register Infrastructure services (DbContext + Repositories)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Port=5432;Database=dbproject;Username=postgres;Password=postgres";
builder.Services.AddInfrastructureServices(connectionString);

// Register Application services
builder.Services.AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapRazorPages();

// Default redirect to login
app.MapGet("/", () => Results.Redirect("/Account/Login"));

app.Run();
