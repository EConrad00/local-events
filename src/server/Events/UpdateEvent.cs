using Server.Data;
using Server._internal;
using Microsoft.EntityFrameworkCore;

namespace Server.Events;

public class UpdateEventEndpoints : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder app) => app
        .MapPut("/api/events/{id:int}", Handler)
        .WithName("UpdateEvent")
        .WithSummary("Update a Event")
        .WithTags("Events")
        .Produces<Event>(201)
        .Produces(204)
        .Produces(404);

    public record UpdateEventRequest(
        string Name,
        string Description,
        DateTime DateTime,
        string Location,
        ICollection<Category> EventCategories
    );

    public record UpdateEventResponse(
        int Id,
        string Name,
        string Description,
        DateTime DateTime,
        string Location,
        ICollection<Category> EventCategories
    );

    private static async Task<IResult> Handler(int id, UpdateEventRequest request, AppDbContext dbContext)
    {
        var UpdatedEvent = await dbContext.Events
            .Include(e => e.Categories)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (UpdatedEvent == null)
        {
            return Results.NotFound(new { Message = $"Category with ID {id} not found." });
        }
        UpdatedEvent.Name = request.Name;
        UpdatedEvent.Location = request.Location;
        dbContext.Events.Update(UpdatedEvent);
        await dbContext.SaveChangesAsync();

        var response = new UpdateEventResponse(
            UpdatedEvent.Id,
            UpdatedEvent.Name,
            UpdatedEvent.Description,
            UpdatedEvent.DateTime,
            UpdatedEvent.Location,
            UpdatedEvent.Categories
        );
        
        return Results.Ok(response);
    }
}