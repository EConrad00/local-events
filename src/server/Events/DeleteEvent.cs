using Server.Data;
using Server._internal;
using Microsoft.EntityFrameworkCore;

namespace Server.Events;

public class DeleteEventEndpoints : IEndpoint
{ 
    public static void MapEndpoint(IEndpointRouteBuilder app) => app
        .MapDelete("/api/events/{id:int}", Handler)
        .WithName("DeleteEvent")
        .WithSummary("Delete a Event")
        .WithTags("Events")
        .Produces<Category>(201)
        .Produces(204)
        .Produces(404);

    public record DeleteEventResponse(
        int Id
    );

    private static async Task<IResult> Handler(int id, AppDbContext dbContext)
    {

        var deleteEvent = await dbContext.Events
            .FindAsync(id);

        if (deleteEvent == null)
        {
            return Results.NotFound(new { Message = $"Event with ID {id} not found." });
        }
        dbContext.Events.Remove(deleteEvent);
        await dbContext.SaveChangesAsync();

        return Results.Ok();
    }
}