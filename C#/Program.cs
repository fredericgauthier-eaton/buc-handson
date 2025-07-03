using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Collections.Concurrent;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("ApiKeyAuth", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "username",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Description = "Username header (use 'calaca')"
    });
    options.AddSecurityDefinition("ApiPassword", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "password",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Description = "Password header (use '12345')"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "ApiKeyAuth"
                }
            }, new string[] {}
        },
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "ApiPassword"
                }
            }, new string[] {}
        }
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        policy => policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});
builder.Services.AddRateLimiter(_ =>
    _.AddFixedWindowLimiter(policyName: "fixed", options =>
    {
        options.Window = TimeSpan.FromMinutes(1);
        options.PermitLimit = 50;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 0;
    }));
builder.Services.AddSingleton<PeopleStore>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowLocalhost");
app.UseRateLimiter();
app.Use(async (context, next) =>
{
    // Logging (like morgan)
    Console.WriteLine($"{context.Request.Method} {context.Request.Path}");
    await next();
});
app.Use(async (context, next) =>
{
    // Security headers (like helmet)
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    await next();
});

// Auth middleware
app.Use(async (context, next) =>
{
    var username = context.Request.Headers["username"].FirstOrDefault();
    var password = context.Request.Headers["password"].FirstOrDefault();
    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsJsonAsync(new { message = "Username or password can't be blank!" });
        return;
    }
    if (username != "calaca" || password != "12345")
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsJsonAsync(new { message = "Invalid credentials!" });
        return;
    }
    await next();
});

// Error handling middleware
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { message = error?.Message });
    });
});

// In-memory stateless store (per instance)
var peopleStore = app.Services.GetRequiredService<PeopleStore>();

app.MapGet("/people", ([FromServices] PeopleStore store) => Results.Ok(store.GetAll()));

app.MapGet("/people/{id:int}", ([FromServices] PeopleStore store, int id) =>
{
    var person = store.GetById(id);
    if (person == null)
        return Results.NotFound(new { message = "Person not found" });
    return Results.Ok(person);
});

app.MapPost("/people", ([FromServices] PeopleStore store, PersonDto dto) =>
{
    var person = store.Add(dto.Name, dto.Age);
    return Results.Created($"/people/{person.Id}", person);
});

app.MapDelete("/people", ([FromServices] PeopleStore store) =>
{
    store.Clear();
    return Results.Ok(Array.Empty<Person>());
});

app.MapGet("/people/search", ([FromServices] PeopleStore store, string name) =>
{
    var results = store.Search(name);
    return Results.Ok(results);
});

app.Run("http://localhost:3000");

public record Person(int Id, string Name, int Age);
public record PersonDto(string Name, int Age);

public class PeopleStore
{
    private List<Person> _people = new();
    public IEnumerable<Person> GetAll() => _people;
    public Person? GetById(int id) => _people.FirstOrDefault(p => p.Id == id);
    public Person Add(string name, int age)
    {
        var person = new Person(_people.Count, name, age);
        _people.Add(person);
        return person;
    }
    public void Clear() => _people.Clear();
    public IEnumerable<Person> Search(string name) => _people.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
}
