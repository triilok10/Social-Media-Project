using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.DependencyInjection;
using Social_Media_Project;
using Social_Media_Project.AppCode;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ISessionService, SessionService>();

// Register EncryptionHelper as a singleton
builder.Services.AddSingleton<EncryptionHelper>();

// Configure session management
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure Google Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.CallbackPath = "/signin-google";
});


var firebaseConfig = builder.Configuration.GetSection("Firebase");
var serviceAccountPath = firebaseConfig["ServiceAccountPath"];

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(serviceAccountPath)
});



var app = builder.Build();
var configuration = builder.Configuration;
string connectionString = "data source=ACER\\CYNOSUREDBS; Initial Catalog = SocialMediaProject; Integrated Security = true; TrustServerCertificate=true";
configuration["ConnectionStrings:CustomConnection"] = connectionString;

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
