using Server.Data;
using Server._internal;
using Microsoft.EntityFrameworkCore;

namespace Server.Events;

public class GetCategoryEndpoints : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder app) => app
        .MapGet("/api/categories/{id:int}", Handler)
        .WithName("GetCategory")
        .WithSummary("Get a Category")
        .WithTags("Categories")
        .Produces<Category>(201)
        .Produces(204)
        .Produces(404);

    public record GetCategoryResponse(
        int Id,
        string Name,
        ICollection<Event> EventCategories
    );

    private static async Task<IResult> Handler(int id, AppDbContext dbContext)
    {
        var GetCategory = await dbContext.Categories
            .Include(c => c.Events)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (GetCategory == null)
        {
            return Results.NotFound(new { Message = $"Category with ID {id} not found." });
        }

        var response = new GetCategoryResponse(
            GetCategory.Id,
            GetCategory.Name,
            GetCategory.Events
        );
        
        return Results.Ok(response);
    }
}