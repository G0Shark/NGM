using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using MySql.EntityFrameworkCore.Extensions;

//todo: почистить всё это, тут моного лишнего и непонятного

var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.TypeInfoResolver = MyJsonContext.Default; // Set the TypeInfoResolver
});

var app = builder.Build();

app.MapGet(NGM_Server.Path.MainAPIPath + NGM_Server.Path.Test, () =>
{
    return "Pong! QQ, мой друг";
});

app.MapPost(NGM_Server.Path.MainAPIPath + NGM_Server.Path.Test, (string returned) =>
{
    return "Pong! Ты написал " + returned;
});

app.UseAuthorization();
app.MapControllers();

NGM_Server.DBConnection.connection = new MySqlConnection(new ConfigurationBuilder().AddJsonFile("Secrets/settings.json").Build().GetConnectionString("DefaultConnection"));

try
{
    NGM_Server.DBConnection.connection.Open();
    Console.WriteLine("SQL Connection success!");
    
    app.Run();
    
    
    
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
finally
{
    NGM_Server.DBConnection.connection.Close();
}
