using Microsoft.EntityFrameworkCore;
using Server.Data;
using Scalar.AspNetCore;
using Server.Events;

// Create the web application builder
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Add CORS for frontend communication
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add OpenAPI services for Scalar
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    // Map OpenAPI endpoint
    app.MapOpenApi();
    
    // Add Scalar API documentation
    app.MapScalarApiReference(options =>
    {
        options.Title = "Local Events API";
        options.Theme = ScalarTheme.BluePlanet;
        options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseCors();
app.UseHttpsRedirection();

// API Routes
app.MapGet("/", () => "Local Events API is running!")
   .WithName("GetRoot")
   .WithSummary("API Status")
   .WithDescription("Returns a simple status message to confirm the API is running")
   .WithTags("Status");

app.MapGet("/api/health", () => new { 
       status = "healthy", 
       timestamp = DateTime.Today,
       version = "1.0.0",
       environment = app.Environment.EnvironmentName
   })
   .WithName("GetHealthCheck")
   .WithSummary("Health Check")
   .WithDescription("Returns the current health status of the API")
   .WithTags("Health")
   .Produces<object>(200);

// Register CRUD endpoints
CreateEventEndpoints.MapEndpoint(app);
DeleteEventEndpoints.MapEndpoint(app);
UpdateEventEndpoints.MapEndpoint(app);
GetEventEndpoints.MapEndpoint(app);

CreateCategoryEndpoints.MapEndpoint(app);
DeleteCategoryEndpoints.MapEndpoint(app);
UpdateCategoryEndpoints.MapEndpoint(app);
GetCategoryEndpoints.MapEndpoint(app);
//look at making a endpointmaker!!!


// Start the server
app.Run();
