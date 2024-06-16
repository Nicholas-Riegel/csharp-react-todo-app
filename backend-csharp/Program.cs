using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using backend_csharp.Data;
using Dapper; // Dapper is an Object-Relational Mapping (ORM) library for .NET that extends the IDbConnection interface. It provides methods to map database query results directly to objects, making database interaction more straightforward and efficient compared to traditional ADO.NET methods.
using backend_csharp.Models;
using System.Data;

// from the Microsoft.AspNetCore.Builder namespace
var builder = WebApplication.CreateBuilder(args);

var connectionString = "Server=localhost;Database=csharp_webapi_01;User=root;Password='';Port=3306;";

// DatabaseContext from Data/DatabaseContext.cs
// (A singleton is a design pattern that restricts the instantiation of a class to one single instance. In the context of ASP.NET Core, when a service is registered as a singleton, the same instance of the service is used throughout the application's lifetime. This means that the DI container will create the instance the first time it is requested, and then it will return that same instance for all subsequent requests.)
builder.Services.AddSingleton(new DatabaseContext(connectionString));

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});

// For Https redirection outside of development environment
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// get all todos
app.MapGet("/todos", async (DatabaseContext dbContext) =>
{
    // 'using' is used to declare a scope within which a resource (like a file stream, database connection, etc.) is used, and ensures that the resource is properly disposed of when the execution leaves the scope.
    using var connection = dbContext.CreateConnection();
    var todos = await connection.QueryAsync<Todo>("SELECT * FROM todos;"); //Type Parameter (<Todo>): Specifies the type of object that each row of the query result will be mapped to. In this example, Todo is a class with properties (Id, Name, Age) that correspond to the columns in the Todo table.
    return Results.Ok(todos);
});

// post a new todo
app.MapPost("/todos", async (Todo newTodo, DatabaseContext dbContext) =>
{
    using var connection = dbContext.CreateConnection();
    var sql = "INSERT INTO todos (todo_text) VALUES (@Todo_text);";
    var result = await connection.ExecuteAsync(sql, new { newTodo.Todo_text });
    return Results.Ok(result);
});

// get a one todo
app.MapGet("/todos/todo/{id}", async (int id, DatabaseContext dbContext) =>
{
    using var connection = dbContext.CreateConnection();
    var todo = await connection.QueryFirstOrDefaultAsync<Todo>("SELECT * FROM todos WHERE Id = @Id;", new { Id = id });
    return todo is not null ? Results.Ok(todo) : Results.NotFound();
});

// update a todo
app.MapPut("/todos/todo/{id}", async (int id, Todo updatedTodo, DatabaseContext dbContext) =>
{
    using var connection = dbContext.CreateConnection();
    var sql = "UPDATE todos SET todo_text = @Todo_text WHERE Id = @Id;";
    var result = await connection.ExecuteAsync(sql, new { updatedTodo.Todo_text, Id = id });
    return result > 0 ? Results.Ok() : Results.NotFound();
});

// delete a todo
app.MapDelete("/todos/todo/{id}", async (int id, DatabaseContext dbContext) =>
{
    using var connection = dbContext.CreateConnection();
    var result = await connection.ExecuteAsync("DELETE FROM todos WHERE Id = @Id;", new { Id = id });
    return result > 0 ? Results.Ok() : Results.NotFound();
});

app.Run();
