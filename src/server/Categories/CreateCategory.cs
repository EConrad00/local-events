using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server._internal;

namespace Server.Events;

public class CreateCategoryEndpoints : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder app) => app
        .MapPost("/api/categories", Handle)
        .WithName("CreateCategory")
        .WithSummary("Create a new Category")
        .WithTags("Categories")
        .Produces<Category>(201);
    
    public record CreateCategoryRequest(
        string Name
    );

    public record CreateCategoryResponse(
        int Id,
        string Name
    );

    private static async Task<IResult> Handle(CreateCategoryRequest request, AppDbContext dbContext)
    {
        
        var newCategory = new Category
        {
            Name = request.Name
        };

        dbContext.Categories.Add(newCategory);
        await dbContext.SaveChangesAsync();

        var response = new CreateCategoryResponse(
            newCategory.Id,
            newCategory.Name
        );

        return Results.Created($"/api/categories/{newCategory.Id}", response);
    }
    
}