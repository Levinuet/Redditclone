using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Model;
using Data;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);

// Sætter CORS så API'en kan bruges fra andre domæner
var AllowSomeStuff = "_AllowSomeStuff";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowSomeStuff, builder => {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// Configure EF and DbContext
builder.Services.AddDbContext<RedditContext>(options => options.UseSqlite("Data Source=reddit.db"));

var app = builder.Build();

app.UseCors(AllowSomeStuff);

app.Use(async (context, next) =>
{
    context.Response.ContentType = "application/json; charset=utf-8";
    await next(context);
});

//app.UseHttpsRedirection();

// Routes for Posts
app.MapGet("/api/posts", async (RedditContext db) =>
    await db.Posts.Include(p => p.Comments).ToListAsync());

app.MapGet("/api/posts/{id}", async (int id, RedditContext db) =>
    await db.Posts.Include(p => p.Comments).FirstOrDefaultAsync(p => p.Id == id));

app.MapPost("/api/posts", async (Post post, RedditContext db) =>
{
    post.CreatedAt = DateTime.UtcNow;
    db.Posts.Add(post);
    await db.SaveChangesAsync();
    return Results.Created($"/api/posts/{post.Id}", post);
});

app.MapPut("/api/posts/{id}/upvote", async (int id, RedditContext db) =>
{
    var post = await db.Posts.FindAsync(id);
    if (post == null) return Results.NotFound();
    post.Upvotes++;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPut("/api/posts/{id}/downvote", async (int id, RedditContext db) =>
{
    var post = await db.Posts.FindAsync(id);
    if (post == null) return Results.NotFound();
    post.Downvotes++;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/api/posts/{id}", async (int id, RedditContext db) =>
{
    var post = await db.Posts.FindAsync(id);
    if (post == null) return Results.NotFound();

    db.Posts.Remove(post);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Routes for Comments
app.MapPost("/api/posts/{id}/comments", async (int id, Comment comment, RedditContext db) =>
{
    var post = await db.Posts.FindAsync(id);
    if (post == null) return Results.NotFound();
    comment.CreatedAt = DateTime.UtcNow;
    comment.PostId = id;
    db.Comments.Add(comment);
    await db.SaveChangesAsync();
    return Results.Created($"/api/posts/{id}/comments/{comment.Id}", comment);
});

app.MapPut("/api/posts/{postId}/comments/{commentId}/upvote", async (int postId, int commentId, RedditContext db) =>
{
    var comment = await db.Comments.FindAsync(commentId);
    if (comment == null) return Results.NotFound();
    comment.Upvotes++;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPut("/api/posts/{postId}/comments/{commentId}/downvote", async (int postId, int commentId, RedditContext db) =>
{
    var comment = await db.Comments.FindAsync(commentId);
    if (comment == null) return Results.NotFound();
    comment.Downvotes++;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/api/posts/{postId}/comments/{commentId}", async (int postId, int commentId, RedditContext db) =>
{
    var comment = await db.Comments.FindAsync(commentId);
    if (comment == null || comment.PostId != postId) return Results.NotFound();

    db.Comments.Remove(comment);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
