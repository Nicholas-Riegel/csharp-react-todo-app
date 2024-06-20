using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using backend_csharp.Data;
using Dapper;
using backend_csharp.Models;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Host=localhost;Port=5432;Database=csharp_webapi_01;Username=ubuntu;Password=ubuntu;";
builder.Services.AddSingleton(new DatabaseContext(connectionString));

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Get all todos
app.MapGet("/todos", async (DatabaseContext dbContext) =>
{
    using var connection = dbContext.CreateConnection();
    var todos = await connection.QueryAsync<Todo>("SELECT * FROM todos ORDER BY id;");
    return Results.Ok(todos);
});

// Post a new todo
app.MapPost("/todos", async (Todo newTodo, DatabaseContext dbContext) =>
{
    using var connection = dbContext.CreateConnection();
    var sql = "INSERT INTO todos (description, completed) VALUES (@Description, @Completed) RETURNING id;";
    var newId = await connection.ExecuteScalarAsync<int>(sql, new { newTodo.Description, newTodo.Completed });

    if (newId > 0)
    {
        newTodo.Id = newId;
        return Results.Ok(newTodo);
    }

    return Results.Problem("An error occurred while creating the todo.");
});

// Get a single todo
app.MapGet("/todos/todo/{id}", async (int id, DatabaseContext dbContext) =>
{
    using var connection = dbContext.CreateConnection();
    var todo = await connection.QueryFirstOrDefaultAsync<Todo>("SELECT * FROM todos WHERE Id = @Id;", new { Id = id });
    return todo is not null ? Results.Ok(todo) : Results.NotFound();
});

// Update a todo
app.MapPut("/todos/todo/{id}", async (int id, Todo updatedTodo, DatabaseContext dbContext) =>
{
    using var connection = dbContext.CreateConnection();
    var sqlUpdate = "UPDATE todos SET description = @Description, completed = @Completed WHERE id = @Id;";
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
