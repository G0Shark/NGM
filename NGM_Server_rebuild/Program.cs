//Привет мир!
using System;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;
using Mysqlx;
using NGM_Server_rebuild;
using NGM_Server_rebuild.Controllers;
using NGM_Server_rebuild.Hubs;
using ZstdSharp.Unsafe;
using Path = NGM_Server_rebuild.Path;

//Проверка на коннект, чтобы я не подключился к несуществующей дб
try
{
    //Если коннект нормальный, то всё ок.
    var c = new MySqlConnection(gb.dbconn);
    c.Open();
    c.Close();
    c = new MySqlConnection(gb.admdbconn);
    c.Open();
    c.Close();
}
catch (Exception ex)
{
    //Даже если ДБ не существует или она не запущена, это не помешает приложению
    //После дева удалить, ошибка должна отключать сервер
    Console.Error.WriteLine("MYSQL connection FAILURE!\nMore info below:\n"+ex.Message);
}

//Конфигуратор
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddControllers();

//Создание приложения на основе конфигуратора
var app = builder.Build();

//После дева удалить
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseDeveloperExceptionPage();
app.UseHttpsRedirection();

//Присоединение всех моих контроллеров и файлов
app.MapControllers();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

app.MapGet("/api/ping", () =>
{
    return "Pong!";
});

//Маппинг SignalR хабов
app.MapHub<ChatHub>(Path.ChatHub); //Все текстовые сообщения
app.MapHub<VoiceHub>(Path.VoiceHub); //Все голосовые сообщения

//Запуск приложения!
app.Run();