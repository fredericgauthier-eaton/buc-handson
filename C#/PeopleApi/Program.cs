using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PeopleApi.Middlewares;
using PeopleApi.Services;
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

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<AuthMiddleware>();

// Error handler
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
