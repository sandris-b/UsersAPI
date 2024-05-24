using UsersAPI.Models;
using Newtonsoft.Json;
using UsersAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<UsersDBContext>(options => 
{
    options.UseInMemoryDatabase(builder.Configuration.GetConnectionString("Database"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.MapGet("/getremoteusers", async () =>
{
    try
    {
        var result = await UsersAPI.Extensions.Client.GetStringAsync(app.Configuration["endpoint"]);
        var users = JsonConvert.DeserializeObject<List<User>>(result);
        return Results.Ok(users);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex.StackTrace);
        return Results.Json(new { error = ex.Message }, statusCode: 500);
    }
}).WithName("GetRemoteUsers")
.WithOpenApi();

app.MapGet("/getlocalusers", (UsersDBContext db) =>
{
    try
    {
        return Results.Ok(db.Users.ToList());
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex.StackTrace);
        return Results.Json(new { error = ex.Message }, statusCode: 500);
    }
}).WithName("GetLocalUsers")
.WithOpenApi();

app.MapPost("/synctolocal", async (UsersDBContext db) =>
{
    try
    {
        var result = await UsersAPI.Extensions.Client.GetStringAsync(app.Configuration["endpoint"]);
        var userList = JsonConvert.DeserializeObject<List<User>>(result);
        if (userList != null && userList.Count > 0)
        {
            UsersAPI.Extensions.AddOrUpdateRange(db.Users, userList);
            db.SaveChanges();
            return Results.Json(new { status = "Users Updated." }, statusCode: 202);
        }
        return Results.Json(new { status = "No users updated." }, statusCode: 200);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex.StackTrace);
        return Results.Json(new { error = ex.Message }, statusCode: 500);
    }
}).WithName("SyncToLocal")
.WithOpenApi();

app.MapPost("/synctoremote", async (UsersDBContext db) =>
{
    try
    {
        var result = await UsersAPI.Extensions.Client.GetStringAsync(app.Configuration["endpoint"]);
        var remoteUserList = JsonConvert.DeserializeObject<List<User>>(result);
        if (remoteUserList != null && remoteUserList.Count > 0)
        {
            foreach(var localUser in db.Users.ToList())
            {
                var existing = remoteUserList.FirstOrDefault(x => x.id == localUser.id);
                if (existing == null)
                {
                    // (without actually executing them against API)
                    //await UsersAPI.Extensions.Client.PutAsJsonAsync<User>(app.Configuration["endpoint"], localUser);
                }
                else
                {
                    // (without actually executing them against API)
                    //await UsersAPI.Extensions.Client.PostAsJsonAsync<User>(app.Configuration["endpoint"], localUser);
                }
            }
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex.ToString());
        return Results.Json(new { error = ex.Message }, statusCode: 500);
    }
    return Results.Json(new { status = "Ok" }, statusCode: 202);
}).WithName("SyncToRemote")
.WithOpenApi();

app.MapPost("/addorupdateuser", async (UsersDBContext db, HttpRequest request, User user) =>
{
    try
    {
        var body = new StreamReader(request.Body);
        string postData = await body.ReadToEndAsync();
        var postedUser = user;
        List<User> list = [postedUser];
        UsersAPI.Extensions.AddOrUpdateRange(db.Users, list);
        db.SaveChanges();
        return Results.Json(new { status = "Ok" }, statusCode: 202);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex.ToString());
        return Results.Json(new { error = ex.Message }, statusCode: 500);
    }
}).WithName("AddOrUpdateUser")
.WithOpenApi();

app.MapDelete("/deleteuser/{id}", (UsersDBContext db, int id) =>
{
    try
    {
        var user = db.Users.FirstOrDefault(x => x.id == id);
        if (user != null)
        {
            db.Users.Remove(user);
            db.SaveChanges();
            return Results.Json(new { status = "Deleted." }, statusCode: 200);
        }
        return Results.Json(new { status = "User Not Found" }, statusCode: 404);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex.ToString());
        return Results.Json(new { error = ex.Message }, statusCode: 500);
    }
}).WithName("DeleteUser")
.WithOpenApi();

app.Run();
