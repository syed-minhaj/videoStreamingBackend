using Microsoft.AspNetCore.Http.Features;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 5000000000; // 5GB
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 5L * 1024 * 1024 * 1024; // 5 GB
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

}


app.UseHttpsRedirection();



app.MapPost("/api/video/upload", async (HttpRequest request) =>
{
    if (!request.HasFormContentType)
        return Results.BadRequest("Missing form content type");

    var form = await request.ReadFormAsync();
    var file = form.Files["file"];

    if (file == null || file.Length == 0)
        return Results.BadRequest("No file found");
    
    if (!file.ContentType.Contains("mp4"))
        return Results.BadRequest("Invalid file type");
    
    if (file.Length > 5_000_000_000)
        return Results.BadRequest("File too large");

    string videoId = await saveFile.SaveFileAsync(file);

    return Results.Ok( new
        {
            nessage = "Success",
            videoId = videoId
        }
    );
});

app.Run();
