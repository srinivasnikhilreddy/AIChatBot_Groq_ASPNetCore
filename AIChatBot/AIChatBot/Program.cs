using AIChatBot;
using AIChatBot.Contexts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register MyDbContext with the connection string from appsettings.json
builder.Services.AddDbContext<ChatDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("sqlconstr")));

// Add MVC with Views (Razor)
builder.Services.AddControllersWithViews();

// Bind GroqSettings from appsettings.json
builder.Services.Configure<GroqSettings>(
    builder.Configuration.GetSection("GroqSettings"));

// Add HttpClient (needed for Groq API calls)
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// MVC route (default)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Groq}/{action=Index}/{id?}");

app.Run();
