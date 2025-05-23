using System.Text.RegularExpressions;
using CommonUtilities.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using File = System.IO.File;

namespace CommonUtilities.Services;

public class ImageService : IImageService
{
    private const string ImageType = @"^image\/(jpeg|png)$";
    private const string ImageName = @"^.+\.(jpeg|jpg|png)$";
    private const int ImageSize = 5 * 1024 * 1024;

    private static readonly Regex ImageTypeRegex = new(ImageType, RegexOptions.IgnoreCase);
    private static readonly Regex ImageNameRegex = new(ImageName, RegexOptions.IgnoreCase);

    private readonly IWebHostEnvironment _environment;

    public ImageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public string ValidatePhoto(IFormFile f)
    {
        if (!ImageTypeRegex.IsMatch(f.ContentType) || !ImageNameRegex.IsMatch(f.FileName))
            return "Only JPG and PNG photo is allowed.";

        return f.Length > ImageSize ? "Photo size cannot more than 5MB." : "";
    }

    public string SavePhoto(IFormFile f, string folder)
    {
        string originalExtension = Path.GetExtension(f.FileName).ToLowerInvariant();
        string targetExtension = ".jpg"; // Default to jpg

        // Allow specific extensions or preserve original if it's one of the allowed types
        if (originalExtension == ".png" || originalExtension == ".jpeg" || originalExtension == ".jpg")
            targetExtension = originalExtension;
        // Ensure the target extension is one that SixLabors.ImageSharp can save easily by default.
        // Forcing to .jpg if unsure is a safe bet for broad compatibility if original format isn't critical.
        // If original format (like PNG transparency) is critical, more sophisticated handling is needed.
        // For this example, we'll use .jpg if it's not png or jpeg/jpg.
        if (targetExtension != ".png" && targetExtension != ".jpeg" && targetExtension != ".jpg")
            targetExtension = ".jpg";


        string fileName = Guid.NewGuid().ToString("n") + targetExtension;
        string path = Path.Combine(_environment.WebRootPath, folder, fileName);

        ResizeOptions options = new()
        {
            Size = new Size(200, 200),
            Mode = ResizeMode.Crop
        };

        using Stream stream = f.OpenReadStream();
        using Image img = Image.Load(stream);
        img.Mutate(x => x.Resize(options));
        img.Save(path); // ImageSharp will attempt to save in the format indicated by the path's extension

        return fileName;
    }

    public void DeletePhoto(string file, string folder)
    {
        file = Path.GetFileName(file);
        string path = Path.Combine(_environment.WebRootPath, folder, file);
        File.Delete(path);
    }
}