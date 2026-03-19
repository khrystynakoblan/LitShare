using LitShare.BLL.Services;
using LitShare.BLL.Services.Interfaces;
using LitShare.DAL.Context;
using LitShare.DAL.Models;
using LitShare.DAL.Repositories;
using LitShare.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting LitShare application...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
    dataSourceBuilder.MapEnum<DealType>("deal_type_t");
    dataSourceBuilder.MapEnum<RoleType>("role_t");
    var dataSource = dataSourceBuilder.Build();

    builder.Services.AddDbContext<LitShareDbContext>(options =>
        options.UseNpgsql(dataSource));

    builder.Services.AddScoped<IPasswordHasher<Users>, PasswordHasher<Users>>();

    builder.Services.AddScoped<IUserRepository, UserRepository>();

    builder.Services.AddScoped<IRegisterService, RegisterService>();

    builder.Services.AddScoped<ProfileService>();

    builder.Services.AddControllersWithViews();

    builder.Services.AddScoped<IProfileService, ProfileService>();

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
    });

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}