using Microsoft.AspNetCore.Http;

namespace CommonUtilities.Interfaces;

public interface IImageService
{
    string ValidatePhoto(IFormFile f);
    string SavePhoto(IFormFile f, string folder);
    void DeletePhoto(string file, string folder);
}