using Server.Data;
using Server._internal;
using Microsoft.EntityFrameworkCore;

namespace Server.Events;

public class GetCategoryEndpoints : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder app) => app
        .MapPut("/api/categories/{id:int}", Handler)
        .WithName("GetCategory")
        .WithSummary("Get a Category")
        .WithTags("Categories")
        .Produces<Category>(201)
        .Produces(204)
        .Produces(404);

    public record GetCategoryResponse(
        int Id
    );

    private static async Task<IResult> Handler(int id, AppDbContext dbContext)
    { 
        
    }
}