using Server.Data;
using Server._internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Server.Events;

public class UpdateEventEndpoints : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder app) => app
        .MapPatch("/api/events/{id:int}", Handler)
        .WithName("UpdateEvent")
        .WithSummary("Update a Event")
        .WithTags("Events")
        .Produces<Event>(201)
        .Produces(204)
        .Produces(404);

    public record UpdateEventRequest(
        string Name,
        string Description,
        DateTime? DateTime,
        string Location,
        int[]? CategoryId
    );

    public record UpdateEventResponse(
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

    private static async Task<IResult> Handler(int id, UpdateEventRequest request, AppDbContext dbContext)
    {
        var ExistingEvent = await dbContext.Events
            .Include(e => e.Categories)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (ExistingEvent == null)
        {
            return Results.NotFound(new { Message = $"Event with ID {id} not found." });
        }

        var UpdatedEvent = new Event
        {
            Id = id,
            Name = request.Name ?? ExistingEvent.Name,
            Description = request.Description ?? ExistingEvent.Description,
            DateTime = request.DateTime ?? ExistingEvent.DateTime,
            Location = request.Location ?? ExistingEvent.Location
            //Categories = request.EventCategories ?? ExistingEvent.Categories
        };

        if (request.CategoryId != null)
        {
            ExistingEvent.Categories.Clear();
            var newCategories = await dbContext.Categories
                .Where(c => request.CategoryId.Contains(c.Id))
                .ToListAsync();

            foreach (var category in newCategories)
            {
                ExistingEvent.Categories.Add(category);
            }
        }

        dbContext.Entry(ExistingEvent).CurrentValues.SetValues(UpdatedEvent);
        await dbContext.SaveChangesAsync();

        var response = new UpdateEventResponse(
            ExistingEvent.Id,
            ExistingEvent.Name,
            ExistingEvent.Description,
            ExistingEvent.DateTime,
            ExistingEvent.Location,
            ExistingEvent.Categories.Select(category => new CategoryDto(
                category.Id,
                category.Name
            ))
        );

        return Results.Ok(response);
    }
}