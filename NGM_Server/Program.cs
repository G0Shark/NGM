using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using MySql.EntityFrameworkCore.Extensions;

//todo: почистить всё это, тут моного лишнего

var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.TypeInfoResolver = MyJsonContext.Default; // Set the TypeInfoResolver
});

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

var b = new ConfigurationBuilder().AddJsonFile("Secrets/settings.json").Build();

NGM_Server.Conf.DBConnection = b.GetConnectionString("DefaultConnection");
MySqlConnection connection = new MySqlConnection(NGM_Server.Conf.DBConnection);

try
{
    connection.Open();
    Console.WriteLine("SQL Connection success!");
    connection.Close();
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
finally
{
    connection.Close();
}
