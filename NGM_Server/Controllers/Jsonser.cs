using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using NGM_Server.Controllers;

//todo: мне страшно это удалять
[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ProblemDetails))] // Add ProblemDetails here
[JsonSerializable(typeof(ValidationProblemDetails))]
[JsonSerializable(typeof(Test))] // Add your Test class here
[JsonSerializable(typeof(List<Test>))] // If you are using lists of Test
[JsonSerializable(typeof(UserForm))] // Add your Test class here
[JsonSerializable(typeof(List<UserForm>))] // If you are using lists of Test
internal partial class MyJsonContext : JsonSerializerContext { }