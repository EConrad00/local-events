using Server.Data;
using Server._internal;
using Microsoft.EntityFrameworkCore;

namespace Server.Events;

public class GetEventEndpoints : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder app) => app
        .MapGet("/api/events/{id:int}", Handler)
        .WithName("GetEvent")
        .WithSummary("Get a event")
        .WithTags("Events")
        .Produces<Event>(201)
        .Produces(204)
        .Produces(404);

    public record GetEventResponse(
        int Id,
        string Name,
        ICollection<Category> EventCategories
    );

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
            GetEvent.Categories
        );
        
        return Results.Ok(response);
    }
}