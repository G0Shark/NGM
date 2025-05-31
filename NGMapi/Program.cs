//Привет мир!
using System.Net;
using System.Text;
using LiveStreamingServerNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NGM_Server_rebuild;
using NGM_Server_rebuild.API.API_v1.MIddlewares;
using NGMapi.API.API_v1.Hubs;
using NGMapi.Application.Interfaces;
using NGMapi.Application.Services;
using NGMapi.Domain.Interfaces;
using NGMapi.Infrastructure.Persistence;
using NGMapi.Infrastructure.Repositories;
using NGMapi.Infrastructure.Services;
using Prometheus;
using StackExchange.Redis;

//Конфигуратор
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "NGM API v1", Version = "v1" });
    // some other configs
    options.AddSignalRSwaggerGen();
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
builder.Services.AddSignalR(hubOptions =>{    
    hubOptions.MaximumReceiveMessageSize = null;
});
builder.Services.AddDbContext<NGMDbContext>(options =>
        options.UseMySql(gb.db,
            new MySqlServerVersion(new Version(8, 0, 21))),
    ServiceLifetime.Scoped);
var configuration = ConfigurationOptions.Parse("redis:6379");
configuration.AbortOnConnectFail = false;
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => 
    ConnectionMultiplexer.Connect(configuration));
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();
builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();
builder.Services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IChatsRepository, ChatsRepository>();
builder.Services.AddScoped<IFriendshipRepository, FriendshipRepository>();
builder.Services.AddScoped<IMembersRepository, MembersRepository>();
builder.Services.AddScoped<IMessagingRepository, MessagingRepository>();
builder.Services.AddScoped<IRolesRepository, RolesRepository>();
builder.Services.AddScoped<IServersRepository, ServersRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Dev",
        policy =>
        {
            policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()
                .SetIsOriginAllowed((host) => true);
        });
});
builder.Services.AddControllers();
builder.Services.AddRateLimiter(options => {
    /*options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext => RateLimitPartition.GetFixedWindowLimiter(partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(), factory: partition => new FixedWindowRateLimiterOptions
    {
        AutoReplenishment = true,
        PermitLimit = 50,
        QueueLimit = 0,
        Window = TimeSpan.FromMinutes(1)
    }));*/
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "NGMApiV1",
            
            ValidateAudience = true,
            ValidAudience = "NGMClient",
            ValidateLifetime = true,
            
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII
                    .GetBytes("secret_ultra_key_mega3957239587weasjrgfhnolrv3uh")),
            ValidateIssuerSigningKey = true
        };
    });


//Создание приложения на основе конфигуратора
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

//После дева удалить
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseDeveloperExceptionPage();
app.UseHttpsRedirection();
app.UseCors("Dev");
//Присоединение всех моих контроллеров и файлов
app.MapControllers();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseRateLimiter();
app.UseMetricServer();
app.UseHttpMetrics();

app.MapGet("/api/ping", () =>
{
    return "Pong!";
});

//Маппинг SignalR хабов
app.MapHub<ChatHub>("api/hubs/text"); //Все текстовые сообщения
app.MapHub<VoiceHub>("api/hubs/voice"); //Все голосовые сообщения

using var server = LiveStreamingServerBuilder.Create()
    .Build();
server.RunAsync(new IPEndPoint(IPAddress.Any, 5280));

//Запуск приложения!
app.Run();