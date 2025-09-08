using Server.Data;
using Server._internal;
using Microsoft.EntityFrameworkCore;

namespace Server.Events;

public class DeleteCategoryEndpoints : IEndpoint
{ 
    public static void MapEndpoint(IEndpointRouteBuilder app) => app
        .MapDelete("/api/categories/{id:int}", Handler)
        .WithName("DeleteCategory")
        .WithSummary("Delete a Category")
        .WithTags("Categories")
        .Produces<Category>(201)
        .Produces(204)
        .Produces(404);

    public record DeleteCategoryResponse(
        int Id
    );

    private static async Task<IResult> Handler(int id, AppDbContext dbContext)
    {

        var category = await dbContext.Categories
            .FindAsync(id);

        if (category == null)
        {
            return Results.NotFound(new { Message = $"Category with ID {id} not found." });
        }
        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync();

        return Results.Ok();
    }
}