//Привет мир!
using System;
using MySql.Data.MySqlClient;
using NGM_Server_rebuild;
using NGM_Server_rebuild.Controllers;
using NGM_Server_rebuild.Hubs;
using ZstdSharp.Unsafe;

//Проверка на коннект, чтобы я не подключился к несуществующей дб
/*try
{
    var c = new MySqlConnection(gb.dbconn);
    c.Open();
    c.Close();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}*/

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDeveloperExceptionPage();
app.UseHttpsRedirection();

app.MapControllers();
app.UseDefaultFiles();
app.UseStaticFiles();
 
app.UseRouting();
app.MapHub<ChatHub>("/api/hub");
app.Run();