//Привет мир!
using System;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.RateLimiting;
using MySql.Data.MySqlClient;
using Mysqlx;

//Проверка на коннект, чтобы я не подключился к несуществующей дб
try
{
    //Если коннект нормальный, то всё ок.
    var c = new MySqlConnection("server=localhost;user=root;database=ngm;");
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
builder.Services.AddRateLimiter(options => {
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext => RateLimitPartition.GetFixedWindowLimiter(partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(), factory: partition => new FixedWindowRateLimiterOptions
    {
        AutoReplenishment = true,
        PermitLimit = 5,
        QueueLimit = 0,
        Window = TimeSpan.FromMinutes(1)
    }));
});


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
app.UseRateLimiter();

app.MapGet("/api/ping", () =>
{
    return "Pong!";
});

//Маппинг SignalR хабов
//app.MapHub<ChatHub>(Path.ChatHub); //Все текстовые сообщения
//app.MapHub<VoiceHub>(Path.VoiceHub); //Все голосовые сообщения

//Запуск приложения!
app.Run();