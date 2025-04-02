using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.SERVICES;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Authorization;
using ELIXIRETD.API.Authentication;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;



// Add services to the container.


//builder.Services.AddMediatR(x =>
//{
//    x.RegisterServicesFromAssemblies(typeof(Program).Assembly);
//    //x.RegisterServicesFromAssemblies(typeof(ConsolidateFinanceExport.ConsolidateFinanceExportCommand).Assembly);
//});

var connectionString = builder.Configuration.GetConnectionString("DevConnection");
builder.Services.AddDbContext<StoreContext>(x => x.UseSqlServer(connectionString, sqlOptions => sqlOptions.CommandTimeout(320)));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();

    options.Filters.Add(new AuthorizeFilter(policy));

}).AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Elixir ETD", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

});

builder.Services.AddAuthentication(authOptions =>
{
    authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(jwtOptions =>
    {
        var key = builder.Configuration.GetValue<string>("JwtConfig:Key");
        var keyBytes = Encoding.ASCII.GetBytes(key);
        jwtOptions.SaveToken = true;
        jwtOptions.RequireHttpsMetadata = false;
        jwtOptions.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuer = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
            ValidAudience = builder.Configuration["JwtConfig:Audience"]
        };

    });



//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("ClientPermission", policy =>
//    {
//        policy.AllowAnyOrigin()
//              .AllowAnyHeader()
//              .AllowAnyMethod();
//    });
//});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


//kestrell
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5000); // HTTP
    serverOptions.ListenAnyIP(5001, listenOptions => listenOptions.UseHttps()); // HTTPS
});




builder.Services.AddTransient(typeof(IUserService), typeof(UserService));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
//app.UseCors("ClientPermission");
app.UseCors("AllowAll");
//app.UseMiddleware<ApiKeyAuthMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
