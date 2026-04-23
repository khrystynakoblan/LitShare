using LitShare.BLL.Common;
using LitShare.BLL.Services;
using LitShare.BLL.Services.Interfaces;
using LitShare.DAL.Context;
using LitShare.DAL.Models;
using LitShare.DAL.Repositories;
using LitShare.DAL.Repositories.Interfaces;
using LitShare.Web.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
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
    _ = dataSourceBuilder.MapEnum<RoleType>("role_t");
    dataSourceBuilder.MapEnum<RequestStatus>("request_status_t");
    var dataSource = dataSourceBuilder.Build();

    builder.Services.AddDbContext<LitShareDbContext>(options =>
        options.UseNpgsql(dataSource));

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();
    builder.Services.AddMemoryCache();

    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(AppSettings.SectionName));

    builder.Services.AddScoped<IPasswordHasher<Users>, PasswordHasher<Users>>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IRegisterService, RegisterService>();
    builder.Services.AddScoped<ILoginService, LoginService>();

    builder.Services.AddScoped<IPostRepository, PostRepository>();
    builder.Services.AddScoped<ICreatePostService, CreatePostService>();
    builder.Services.AddScoped<IGenreRepository, GenreRepository>();
    builder.Services.AddScoped<IGenreService, GenreService>();
    builder.Services.AddScoped<IEditPostService, EditPostService>();
    builder.Services.AddScoped<IHomeService, HomeService>();
    builder.Services.AddScoped<IPostService, PostService>();
    builder.Services.AddScoped<IComplaintRepository, ComplaintRepository>();
    builder.Services.AddScoped<IComplaintService, ComplaintService>();
    builder.Services.AddScoped<IAdminService, AdminService>();
    builder.Services.AddControllersWithViews();
    builder.Services.AddScoped<IProfileService, ProfileService>();
    builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();

    builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
    builder.Services.AddScoped<IReviewService, ReviewService>();
    builder.Services.AddScoped<IFavoriteService, FavoriteService>();

    builder.Services.AddScoped<IExchangeRepository, ExchangeRepository>();
    builder.Services.AddScoped<IExchangeService, ExchangeService>();

    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Home/Error";
            options.ExpireTimeSpan = TimeSpan.FromDays(7);
        });

    var app = builder.Build();

    app.UseExceptionHandler();

    if (!app.Environment.IsDevelopment())
    {
        app.UseHsts();
    }

    app.UseMiddleware<RequestTimingMiddleware>();

    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
    });

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseMiddleware<RequestLoggingMiddleware>();

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