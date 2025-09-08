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
        int Id
    );

    
}