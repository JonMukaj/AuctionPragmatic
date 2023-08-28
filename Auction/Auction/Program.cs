using Auction.Extensions;
using Hangfire;
using Hangfire.SqlServer;
using NLog;
using Service.Contracts;
using Service;
using Shared.Utility;

var builder = WebApplication.CreateBuilder(args);
LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

builder.Services.AddControllersWithViews();

// Add services to the container.
builder.Services.AddAuthentication("MyCookieAuthenticationScheme")
    .AddCookie("MyCookieAuthenticationScheme", options =>
    {
        options.LoginPath = new PathString("/Authentication/Login");
        options.LogoutPath = new PathString("/Authentication/Logout");
    });

builder.Services.ConfigureCors(builder.Configuration);
builder.Services.ConfigureIISIntegration();
builder.Services.ConfigureLoggerService();
builder.Services.AddHttpClient();
//builder.Services.ConfigureCookiePath();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureIdentity();
builder.Services.ConfigureResponseCaching();
builder.Services.ConfigureTokenLifetime();
builder.Services.AddCryptographyUtils();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ConfigureSignalR();
builder.Services.AddSignalR();
builder.Services.ConfigureSession();


if (builder.Configuration.GetSection("DefaultConfiguration:UseHangfire").Get<bool>() == true)
{
    builder.Services.AddTransient<IHangfireService, HangfireService>();

    var connectionString = builder.Configuration.GetConnectionString("sqlConnection");

    builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true
        }));

    builder.Services.AddHangfireServer();

}


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/Error");
}
else
{
    app.UseDeveloperExceptionPage();
}
//app.UseExceptionHandler("/Error/Error");

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

if (builder.Configuration.GetSection("DefaultConfiguration:UseHangfire").Get<bool>() == true)
{
    app.UseHangfireDashboard("/hangfire");

    //    RecurringJob.AddOrUpdate<IHangfireService>("updateMarketData", service => service.UpdateDataMarket(), Cron.Minutely(), TimeZoneInfo.Local);
    RecurringJob.AddOrUpdate<IHangfireService>("updateMarketData", service => service.UpdateDataMarket(), Cron.Minutely(), TimeZoneInfo.Local);

}

app.UseSession();

app.Run();