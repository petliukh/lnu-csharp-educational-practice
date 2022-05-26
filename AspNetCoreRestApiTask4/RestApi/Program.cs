using Microsoft.EntityFrameworkCore;
using BCryptNet = BCrypt.Net.BCrypt;


using RestApi.DataAccess.Contracts;
using RestApi.DataAccess.Users;
using RestApi.DataAccess.BlackListedTokens;

using RestApi.Authorization;
using RestApi.Helpers.Users;
using RestApi.Models.Users;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
{
    builder.Services.AddCors();
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddAutoMapper(typeof(Program));

    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

    builder.Services.AddDbContext<ContractPostgreSqlContext>(
        options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddScoped<IContractDataAccessProvider, ContractDataAccessProvider>();

    builder.Services.AddDbContext<UserPostgreSqlContext>(
        options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped<IJwtUtils, JwtUtils>();
    builder.Services.AddScoped<IUserDataAccessProvider, UserDataAccessProvider>();

    builder.Services.AddDbContext<BlackListedTokenPostgreSqlContext>(
        options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddScoped<IBlackListedTokenService, BlackListedTokenService>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// migrate any database changes on startup (includes initial db creation)
using (var scope = app.Services.CreateScope())
{
    var userDataContext = scope.ServiceProvider.GetRequiredService<UserPostgreSqlContext>();    
    userDataContext.Database.Migrate();

    var contractDataContext = scope.ServiceProvider.GetRequiredService<ContractPostgreSqlContext>();    
    contractDataContext.Database.Migrate();
}

// configure HTTP request pipeline
{
    // global cors policy
    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    // global error handler
    app.UseMiddleware<ErrorHandlerMiddleware>();

    // custom jwt auth middleware
    app.UseMiddleware<JwtMiddleware>();

    app.MapControllers();
}

// create hardcoded test users in db on startup
// {
//     var testUsers = new List<User>
//     {
//         new User { Id = 1, FirstName = "Admin", LastName = "User", Username = "admin", PasswordHash = BCryptNet.HashPassword("1111"), Role = Role.Admin },
//         new User { Id = 2, FirstName = "Normal", LastName = "User", Username = "user", PasswordHash = BCryptNet.HashPassword("1111"), Role = Role.User }
//     };
// 
//     using var scope = app.Services.CreateScope();
//     var dataContext = scope.ServiceProvider.GetRequiredService<UserPostgreSqlContext>();
//     dataContext.Users.AddRange(testUsers);
//     dataContext.SaveChanges();
// }

app.Run();
