

class saveFile
{
    public static async Task<string> SaveFileAsync(IFormFile  file)
    {
        var videoId = Guid.NewGuid().ToString();

        var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "storage" , "uploads");
        Directory.CreateDirectory(uploadDir);
        
        var filePath = Path.Combine(uploadDir, $"{videoId}.mp4");
        

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return videoId;
    }
}