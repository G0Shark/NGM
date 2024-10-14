using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
using NGM_Server.Models;

var builder = WebApplication.CreateSlimBuilder(args);
//var secrets = new ConfigurationBuilder().AddJsonFile("Secrets/settings.json").Build();  
var app = builder.Build();

//builder.Services.AddEntityFrameworkMySQL()
    //.AddDbContext<MyDBContext>(options =>
    //{
        //options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection"));
    //});

app.MapGet(NGM_Server.Path.MainAPIPath + NGM_Server.Path.Test, () =>
{
    return "Pong! QQ, мой друг";
});

app.Run();