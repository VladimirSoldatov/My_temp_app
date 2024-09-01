using System;

var builder = WebApplication.CreateBuilder();
var app = builder.Build();

app.Run(async (context) =>
{
    var path = context.Request.Path;
    var fullPath = $"html/{path}";
    var response = context.Response;
    response.ContentType = "text/html; charset=utf-8";
    var request = context.Request;
    if (File.Exists(fullPath))
    {
        await response.SendFileAsync(fullPath);
    }
    else if (context.Request.Path == "/postuser")
    {

        var form = context.Request.Form;
        string name = form["name"];
        string age = form["age"];
        string langList = String.Empty;
        foreach (var item in form["languages"])
        {
            langList += item + ", ";
        }
        langList = langList.Substring(0, langList.Length - 2);
        await context.Response.WriteAsync($"<div><p>Name: {name}</p><p>Age: {age}</p><p>Languages: {langList} </p></div>");
    }
    else if (request.Path == "/api/user")
    {
        var message = "Некорректные данные";   // содержание сообщения по умолчанию
        try
        {
            // пытаемся получить данные json
            var person = await request.ReadFromJsonAsync<Person>();
            if (person != null) // если данные сконвертированы в Person
                message = $"Name: {person.Name}  Age: {person.Age}";
        }
        catch { }
        // отправляем пользователю данные
        await response.WriteAsJsonAsync(new { text = message });
    }


    else if (path == "/")
    {
        await response.SendFileAsync("html/index.html");
    }
    else if (context.Request.Path == "/old")
    {
        context.Response.Redirect("/new");
    }
    else if (context.Request.Path == "/new")
    {
        await context.Response.WriteAsync("New Page");
    }
    else if (path == "/person")
    {
        var persona = new {name = "Игорь", age = 32.1 };
        await context.Response.WriteAsJsonAsync(persona);
    }
    else
    {
        response.StatusCode = 404;
        await response.WriteAsync("<h2>Not Found</h2>");
        response.ContentType = "text/html; charset=utf-8";
        await response.SendFileAsync("html/index.html");
    }
}
);
app.Run();
public record Person(string Name, int Age);