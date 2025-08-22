using LuftbornTask.Data;
using LuftbornTask.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=students.db"));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}


app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Students API running");

// ---- CRUD ----
app.MapGet("/api/students", async (AppDbContext db) =>
    await db.Students.AsNoTracking().ToListAsync());

app.MapGet("/api/students/{id:int}", async (int id, AppDbContext db) =>
    await db.Students.FindAsync(id) is Student s ? Results.Ok(s) : Results.NotFound());

app.MapPost("/api/students", async (Student s, AppDbContext db) =>
{
    db.Students.Add(s);
    await db.SaveChangesAsync();
    return Results.Created($"/api/students/{s.Id}", s);
});

app.MapPut("/api/students/{id:int}", async (int id, Student input, AppDbContext db) =>
{
    var s = await db.Students.FindAsync(id);
    if (s is null) return Results.NotFound();

    s.FirstName = input.FirstName;
    s.LastName  = input.LastName;
    s.Age       = input.Age;
    await db.SaveChangesAsync();
    return Results.Ok(s);
});

app.MapDelete("/api/students/{id:int}", async (int id, AppDbContext db) =>
{
    var s = await db.Students.FindAsync(id);
    if (s is null) return Results.NotFound();

    db.Remove(s);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();