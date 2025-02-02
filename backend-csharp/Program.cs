using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
// call in the DatabaseContext class
using backend_csharp.Data;
// Dapper is an "ORM" that allows you to map data from a database to a C# object
// An ORM is an Object-Relational Mapping tool that simplifies the process of mapping data between objects in code and tables in a database
using Dapper;
// call in the Todo model
using backend_csharp.Models;
using System.Data;




// Create a new WebApplication instance
// WebApplication is a class that is part of the ASP.NET Core framework (Microsoft.AspNetCore.Builder)
var builder = WebApplication.CreateBuilder(args);

var connectionString = "Host=localhost;Port=5432;Database=csharp_webapi_01;Username=ubuntu;Password=ubuntu;";
// A "singleton" is a class that is instantiated only once
builder.Services.AddSingleton(new DatabaseContext(connectionString));

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});

// If the app is not in development mode, use HTTPS
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Get all todos
// dbContext is a new instance of the class \
// the connection string is already in the app
app.MapGet("/todos", async (DatabaseContext dbContext) =>
{
    // CreateConnection is a method in the DatabaseContext class
    using var connection = dbContext.CreateConnection();
    // QueryAsync is a method in the Dapper library that is added to IDbConnection (set in the DatabaseContext class)
    // Dapper's QueryAsync<T> method is a generic method. It means that T can be any type. It is the return type: what is returned from the database
    // Dapper Query commands are just for getting data; Execute commands are for inserting, updating, or deleting data
    var todos = await connection.QueryAsync<Todo>("SELECT * FROM todos ORDER BY id;");
    // returns a 200 ok along with the todos
    return Results.Ok(todos);
});

// Post a new todo
// newTodo is what is passed in the body of the post request
app.MapPost("/todos", async (Todo newTodo, DatabaseContext dbContext) =>
{
    using var connection = dbContext.CreateConnection();
    var sql = "INSERT INTO todos (description, completed) VALUES (@Description, @Completed) RETURNING id;";
    // ExecuteScalarAsync's return type is int because it is returning the id of the new todo which will be added to the new todo and sent back to the client
    // new {} is an anonymous object that is used to pass parameters to the query. Dapper just requires that it be done this way.
    // Dapper Execute commands are for inserting, updating, or deleting data; Query commands are just for getting data
    var newId = await connection.ExecuteScalarAsync<int>(sql, new { newTodo.Description, newTodo.Completed });

    if (newId > 0)
    {
        newTodo.Id = newId;
        return Results.Ok(newTodo);
    }

    return Results.Problem("An error occurred while creating the todo.");
});

// Update a todo
// id and updatedTodo are passed in the body of the put request
app.MapPut("/todos/todo/{id}", async (int id, Todo updatedTodo, DatabaseContext dbContext) =>
{
    using var connection = dbContext.CreateConnection();
    var sqlUpdate = "UPDATE todos SET description = @Description, completed = @Completed WHERE id = @Id;";
    // Dapper Execute commands are for inserting, updating, or deleting data; Query commands are just for getting data
    // not sure why a type isn't needed here. it has something to do with types in this context only being needed for "mapping", but I'm not sure what that means. 
    var result = await connection.ExecuteAsync(sqlUpdate, new { updatedTodo.Description, updatedTodo.Completed, Id = id });

    if (result > 0)
    {
        var sqlSelect = "SELECT * FROM todos WHERE id = @Id;";
        var todo = await connection.QuerySingleOrDefaultAsync<Todo>(sqlSelect, new { Id = id });
        return todo != null ? Results.Ok(todo) : Results.NotFound();
    }

    return Results.NotFound();
});

// Delete a todo
app.MapDelete("/todos/todo/{id}", async (int id, DatabaseContext dbContext) =>
{
    using var connection = dbContext.CreateConnection();
    var result = await connection.ExecuteAsync("DELETE FROM todos WHERE Id = @Id;", new { Id = id });
    return result > 0 ? Results.Ok() : Results.NotFound();
});

app.Run();
