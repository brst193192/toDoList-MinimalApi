using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using TodoApi;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("ToDoDB"),
        new MySqlServerVersion(new Version(8, 0, 23))
    )
);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();


var connectionString = builder.Configuration.GetConnectionString("ToDoDB");

app.MapGet("", async () =>
{
    using var connection = new MySqlConnection(connectionString);
    var query = "SELECT Id, Name, IsComplete FROM Items";
    var items = await connection.QueryAsync<Item>(query);
    return Results.Ok(items);
});

app.MapPost("", async (Item todo) =>
{
    try
    {
        using var connection = new MySqlConnection(connectionString);
        var query = "INSERT INTO Items (Name, IsComplete) VALUES (@Name, @IsComplete); SELECT LAST_INSERT_ID();";
        var parameters = new { Name = todo.Name, IsComplete = todo.IsComplete };
        var id = await connection.ExecuteScalarAsync<int>(query, parameters);

        if (id > 0)
        {
            todo.Id = id;
            return Results.Created($"/{todo.Id}", todo);
        }
        return Results.BadRequest("Failed to insert the item.");
    }
    catch (Exception ex)
    {
        return Results.Problem($"An error occurred: {ex.Message}");
    }
});

app.MapPut("/{id}", async (int id, Item updatedTodo) =>
{
    using var connection = new MySqlConnection(connectionString);
    var query = "UPDATE Items SET Name = @Name, IsComplete = @IsComplete WHERE Id = @Id";
    var parameters = new { Id = id, Name = updatedTodo.Name, IsComplete = updatedTodo.IsComplete };
    var result = await connection.ExecuteAsync(query, parameters);

    if (result > 0)
    {
        return Results.Ok($"Todo with id '{id}' updated successfully.");
    }
    return Results.NotFound($"Todo with id '{id}' not found.");
});

app.MapPut("/{id}/{isComplete}", async (int id, bool isComplete) =>
{
    using var connection = new MySqlConnection(connectionString);
    var query = "UPDATE Items SET IsComplete = @IsComplete WHERE Id = @Id";
    var parameters = new { IsComplete = isComplete, Id = id };
    var result = await connection.ExecuteAsync(query, parameters);
    if (result > 0)
    {
        return Results.Ok($"Todo with id '{id}' updated successfully as {isComplete}.");
    }
    return Results.NotFound($"Todo with id '{id}' not found.");
});

app.MapDelete("/{id}", async (int id) =>
{
    using var connection = new MySqlConnection(connectionString);

    // שליפת הפריט לפני המחיקה
    var selectQuery = "SELECT Id, Name, IsComplete FROM Items WHERE Id = @Id";
    var itemToDelete = await connection.QueryFirstOrDefaultAsync<Item>(selectQuery, new { Id = id });

    if (itemToDelete == null)
    {
        return Results.NotFound($"Todo with id '{id}' not found.");
    }

    var deleteQuery = "DELETE FROM Items WHERE Id = @Id";
    var result = await connection.ExecuteAsync(deleteQuery, new { Id = id });

    if (result > 0)
    {
        return Results.Ok(itemToDelete); // החזרת הפריט שנמחק
    }

    return Results.BadRequest("Failed to delete the item.");
});

app.MapPost("/register", async (User user) =>
{
    using var connection = new MySqlConnection(connectionString);

    var query = "SELECT * FROM Users WHERE Name = @Username AND Password = @Password";
    var parameters = new { Username = user.Name, Password = user.Password };
    var existingUser = await connection.QueryFirstOrDefaultAsync<User>(query, parameters);
    if (existingUser != null)
        return Results.BadRequest("User already exists.");

    // הוספת המשתמש החדש
    query = "INSERT INTO Users (Name, Password) VALUES (@Username, @Password); SELECT LAST_INSERT_ID();";
    var id = await connection.ExecuteScalarAsync<int>(query, parameters);
    if (id > 0)
    {
        user.Id = id;
        return Results.Created($"/{user.Id}", user);
    }
    return Results.BadRequest("Failed to register the user.");
});

app.MapPost("/login", async (User user) =>
{
    using var connection = new MySqlConnection(connectionString);

    var query = "SELECT * FROM Users WHERE Name = @Username AND Password = @Password";
    var parameters = new { Username = user.Name, Password = user.Password };
    var existingUser = await connection.QueryFirstOrDefaultAsync<User>(query, parameters);
    if (existingUser == null)
        return Results.BadRequest("Invalid username or password.");
    return Results.Ok("Login successful!");
});


app.Run();