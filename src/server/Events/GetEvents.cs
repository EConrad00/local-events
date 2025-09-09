using Server.Data;
using Server._internal;
using Microsoft.EntityFrameworkCore;

namespace Server.Events;

public class GetEventEndpoints : IEndpoint
{
   public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        // Get all events
        app.MapGet("/api/events", GetAllHandler)
            .WithName("GetAllEvents")
            .WithSummary("Get all events")
            .WithTags("Events")
            .Produces<IEnumerable<GetEventResponse>>(200);
        
        // Get single event by ID
        app.MapGet("/api/events/{id:int}", Handler)
            .WithName("GetEvent")
            .WithSummary("Get a event")
            .WithTags("Events")
            .Produces<Event>(201)
            .Produces(204)
            .Produces(404);
    }
    public record GetEventResponse(
       int Id,
        string Name,
        string Description,
        DateTime DateTime,
        string Location,
        IEnumerable<CategoryDto> Categories
    );
    
    public record CategoryDto(
        int Id,
        string Name
    );

    private static async Task<IResult> GetAllHandler(AppDbContext dbContext)
    {
        var events = await dbContext.Events
            .Include(e => e.Categories)
            .ToListAsync();

        var response = events.Select(eventItem => new GetEventResponse(
            eventItem.Id,
            eventItem.Name,
            eventItem.Description,
            eventItem.DateTime,
            eventItem.Location,
            eventItem.Categories.Select(category => new CategoryDto(
                category.Id,
                category.Name
            ))
        ));

        return Results.Ok(response);
    }

    private static async Task<IResult> Handler(int id, AppDbContext dbContext)
    {
        var GetEvent = await dbContext.Events
            .Include(e => e.Categories)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (GetEvent == null)
        {
            return Results.NotFound(new { Message = $"Event with ID {id} not found." });
        }

        var response = new GetEventResponse(
            GetEvent.Id,
            GetEvent.Name,
            GetEvent.Description,
            GetEvent.DateTime,
            GetEvent.Location,
            GetEvent.Categories.Select(category => new CategoryDto(
                category.Id,
                category.Name
            ))
        );

        return Results.Ok(response);
    }
}