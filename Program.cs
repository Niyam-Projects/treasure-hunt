using TreasureHunt;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Prevent port collision on Azure App Service: only configure Kestrel in code if Oryx hasn't already injected Kestrel:Endpoints
var kestrelEndpoints = builder.Configuration.GetSection("Kestrel:Endpoints").GetChildren();
if (!kestrelEndpoints.Any())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        // HTTP/1.1 & HTTP/2 on port 8080 (for local testing & gRPC-Web)
        options.ListenAnyIP(8080, o => o.Protocols = HttpProtocols.Http1AndHttp2);
        // Explicit HTTP/2 port on 8586 for direct gRPC clients
        options.ListenAnyIP(8586, o => o.Protocols = HttpProtocols.Http2);
    });
}

builder.Services
    .AddGraphQLServer()
    .AddQueryType<HuntQuery>();

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

var app = builder.Build();

// ── Step 1 ── GET /
app.MapGet("/", async (HttpContext ctx) =>
{
    ctx.Response.Headers.Append("X-Next-Step", "/api/v1/integrity-check");
    ctx.Response.Headers.Append("X-Step-Token", HuntConstants.TokenStep1);
    ctx.Response.ContentType = "application/json";
    await ctx.Response.WriteAsync(
        """{"message":"Welcome to the challenge! The path forward is hidden in plain sight..."}"""
    );
});

// ── Step 2 ── ANY /api/v1/integrity-check
app.Map("/api/v1/integrity-check", (HttpContext ctx) =>
{
    if (!HasValidToken(ctx, HuntConstants.TokenStep1))
        return Unauthorized();

    if (ctx.Request.Method == HttpMethods.Post)
        return Results.Json(
            new { key = HuntConstants.TokenStep2, instruction = "Reconcile the sequence at /api/v1/reconcile" },
            statusCode: 418);

    return Results.Json(new { hint = "A GET request is too passive. You need to push for answers." });
});

// ── Step 3 ── GET /api/v1/reconcile
app.MapGet("/api/v1/reconcile", (HttpContext ctx) =>
{
    if (!HasValidToken(ctx, HuntConstants.TokenStep2))
        return Unauthorized();

    return Results.Json(new
    {
        sequence = new[] { 101, 102, 103, 105, 106 },
        task     = "Find the missing integer 'x'. The final endpoint is /api/v1/finish/{x}",
        key      = HuntConstants.TokenStep3
    });
});

// ── Step 4 ── GET /api/v1/finish/{id}
app.MapGet("/api/v1/finish/{id:int}", (HttpContext ctx, int id) =>
{
    if (!HasValidToken(ctx, HuntConstants.TokenStep3))
        return Unauthorized();

    if (id != 104)
        return Results.NotFound(new { error = "That is not the missing number." });

    return Results.Json(new
    {
        success      = true,
        instructions = $"You hunt very well! Email your resume to hr@niyamit.com with the subject: " +
                       $"'Challenge completed: {HuntConstants.TokenStep1}-{HuntConstants.TokenStep2}-" +
                       $"{HuntConstants.TokenStep3}-104-{HuntConstants.TokenStep4}",
        extraCredit  = new
        {
            hint    = "Two more steps remain for those who really know their protocols.",
            graphql = "POST /graphql — run an introspection query to discover the schema.",
            token   = HuntConstants.TokenStep4
        }
    });
});

// ── Bonus Step 5 ── GraphQL
app.MapGraphQL().WithOptions(new HotChocolate.AspNetCore.GraphQLServerOptions
{
    Tool = { Enable = false }
});

// ── Bonus Step 6 ── gRPC
app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
app.MapGrpcService<HuntGrpcService>();
app.MapGrpcReflectionService();

app.Run();

static bool HasValidToken(HttpContext ctx, string expected) =>
    ctx.Request.Headers.TryGetValue("X-Step-Token", out var val) && val == expected;

static IResult Unauthorized() =>
    Results.Json(new { error = "Missing or invalid X-Step-Token header." }, statusCode: 401);