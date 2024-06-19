using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using backend_csharp.Data;
using Dapper; // Dapper is an Object-Relational Mapping (ORM) library for .NET that extends the IDbConnection interface. It provides methods to map database query results directly to objects, making database interaction more straightforward and efficient compared to traditional ADO.NET methods.
using backend_csharp.Models;
using System.Data;

// from the Microsoft.AspNetCore.Builder namespace
var builder = WebApplication.CreateBuilder(args);

var connectionString = "Server=13.58.34.6;Database=csharp_webapi_01;User=root;Password='';Port=3306;";

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
    var sql = "INSERT INTO todos (description) VALUES (@Description);";
    var result = await connection.ExecuteAsync(sql, new { newTodo.Description });
    if (result > 0)
    {
        // Get the ID of the newly inserted todo
        var newId = await connection.ExecuteScalarAsync<int>("SELECT LAST_INSERT_ID();");
        newTodo.Id = newId; // Set the ID of the new todo

        return Results.Ok(newTodo); // Return the new todo
    }

    return Results.Problem("An error occurred while creating the todo.");
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
    var sqlUpdate = "UPDATE todos SET description = @Description, completed = @Completed WHERE id = @Id;";
    var result = await connection.ExecuteAsync(sqlUpdate, new { updatedTodo.Description, updatedTodo.Completed, Id = id });

    if (result > 0)
    {
        // Fetch the updated todo
        var sqlSelect = "SELECT * FROM todos WHERE id = @Id;";
        var todo = await connection.QuerySingleOrDefaultAsync<Todo>(sqlSelect, new { Id = id });

        return todo != null ? Results.Ok(todo) : Results.NotFound();
    }

    return Results.NotFound();
});


// delete a todo
app.MapDelete("/todos/todo/{id}", async (int id, DatabaseContext dbContext) =>
{
    using var connection = dbContext.CreateConnection();
    var result = await connection.ExecuteAsync("DELETE FROM todos WHERE Id = @Id;", new { Id = id });
    return result > 0 ? Results.Ok() : Results.NotFound();
});

app.Run();
