using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server._internal;

namespace Server.Events;

public class CreateEventEndpoints : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder app) => app
        .MapPost("/api/events", Handle)
        .WithName("CreateEvent")
        .WithSummary("Create a new event")
        .WithTags("Events")
        .Produces<Event>(201);

    public record CreateEventRequest(
        string Name,
        string Description,
        DateTime DateTime,
        string Location,
        int[]? CategoryId
    );

    public record CreateEventResponse(
        int Id,
        string Name,
        string Description,
        DateTime DateTime,
        string Location
       

    );

    private static async Task<IResult> Handle(CreateEventRequest request, AppDbContext dbContext)
    {
        var newEvent = new Event
        {
            Name = request.Name,
            Description = request.Description,
            DateTime = request.DateTime,
            Location = request.Location
        };

        // Connect to categories if provided
        if (request.CategoryId != null && request.CategoryId.Length > 0)
        {
            var categories = await dbContext.Categories
                .Where(c => request.CategoryId.Contains(c.Id))
                .ToListAsync();

            // Add each category to the event's Categories collection
            foreach (var category in categories)
            {
                newEvent.Categories.Add(category);
            }
        }

        dbContext.Events.Add(newEvent);
        await dbContext.SaveChangesAsync();

        var response = new CreateEventResponse(
            newEvent.Id,
            newEvent.Name,
            newEvent.Description,
            newEvent.DateTime,
            newEvent.Location
            
        );
    
        return Results.Created($"/api/events/{newEvent.Id}", response);
    }
}