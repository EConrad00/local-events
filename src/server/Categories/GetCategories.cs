using Server.Data;
using Server._internal;
using Microsoft.EntityFrameworkCore;

namespace Server.Events;

public class GetCategoryEndpoints : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        // Get all categories
        app.MapGet("/api/categories", GetAllHandler)
            .WithName("GetAllCategories")
            .WithSummary("Get all categories")
            .WithTags("Categories")
            .Produces<IEnumerable<GetCategoryResponse>>(200);
        
        // Get single category by ID
        // app.MapGet("/api/categories/{id:int}", Handler)
        //     .WithName("GetCategory")
        //     .WithSummary("Get a Category")
        //     .WithTags("Categories")
        //     .Produces<Category>(201)
        //     .Produces(204)
        //     .Produces(404);
    }

    public record GetCategoryResponse(
        int Id,
        string Name,
        IEnumerable<EventDto> Events
    );
    
    public record EventDto(
        int Id,
        string Name,
        string Description,
        DateTime DateTime,
        string Location
    );

    private static async Task<IResult> GetAllHandler(AppDbContext dbContext)
    {
        var categories = await dbContext.Categories
            .Include(c => c.Events)
            .ToListAsync();

        var response = categories.Select(categoryItem => new GetCategoryResponse(
            categoryItem.Id,
            categoryItem.Name,
            categoryItem.Events.Select(eventGet => new EventDto(
                eventGet.Id,
                eventGet.Name,
                eventGet.Description,
                eventGet.DateTime,
                eventGet.Location
            ))
        ));

        return Results.Ok(response);
    }

    // // private static async Task<IResult> Handler(int id, AppDbContext dbContext)
    // // {
    // //     var GetCategory = await dbContext.Categories
    // //         .Include(c => c.Events)
    // //         .FirstOrDefaultAsync(c => c.Id == id);

    // //     if (GetCategory == null)
    // //     {
    // //         return Results.NotFound(new { Message = $"Category with ID {id} not found." });
    // //     }

    // //     var response = new GetCategoryResponse(
    // //         GetCategory.Id,
    // //         GetCategory.Name,
    // //         GetCategory.Events.Select(eventGet => new EventDto(
    // //             eventGet.Id,
    // //             eventGet.Name,
    // //             eventGet.Description,
    // //             eventGet.DateTime,
    // //             eventGet.Location
    // //         ))
    // //     );

    // //     return Results.Ok(response);
    // // }
}