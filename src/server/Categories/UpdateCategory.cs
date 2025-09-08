using Server.Data;
using Server._internal;
using Microsoft.EntityFrameworkCore;

namespace Server.Events;

public class UpdateCategoryEndpoints : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder app) => app
        .MapPut("/api/categories/{id:int}", Handler)
        .WithName("UpdateCategory")
        .WithSummary("Update a Category")
        .WithTags("Categories")
        .Produces<Category>(201)
        .Produces(204)
        .Produces(404);

    public record UpdateCategoryRequest(
        string Name
    );

    public record UpdateCategoryResponse(
        int Id,
        string Name
    );

    private static async Task<IResult> Handler(int id, UpdateCategoryRequest request, AppDbContext dbContext)
    {
        var Updatedcategory = await dbContext.Categories
            .FindAsync(id);

        if (Updatedcategory == null)
        {
            return Results.NotFound(new { Message = $"Category with ID {id} not found." });
        }
        Updatedcategory.Name = request.Name;
        dbContext.Categories.Update(Updatedcategory);
        await dbContext.SaveChangesAsync();

        var response = new UpdateCategoryResponse(
            Updatedcategory.Id,
            Updatedcategory.Name
        );
        
        return Results.Ok(response);
    }
}