using Serilog;

namespace CommonUtilities.Utilities;

public static class FileUtilities
{
    public static bool DeleteFolder(string directoryPath)
    {
        try
        {
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");

            foreach (string filePath in Directory.GetFiles(directoryPath))
                File.SetAttributes(filePath, FileAttributes.Normal);

            foreach (string subDirectoryPath in Directory.GetDirectories(directoryPath)) DeleteFolder(subDirectoryPath);

            File.SetAttributes(directoryPath, FileAttributes.Normal);

            Directory.Delete(directoryPath, true);

            Log.Information($"Directory deleted: {directoryPath}");
            return true;
        }
        catch (DirectoryNotFoundException ex) // Specific exception
        {
            Log.Error(ex, "Directory not found during deletion: {DirectoryPath}", directoryPath);
            throw; // Rethrow original exception
        }
        catch (IOException ex) // Specific exception for I/O errors
        {
            Log.Error(ex, "IO error deleting directory: {DirectoryPath}", directoryPath);
            throw new IOException($"Error deleting directory: {directoryPath}. {ex.Message}", ex);
        }
        catch (UnauthorizedAccessException ex) // Specific exception for permission issues
        {
            Log.Error(ex, "Unauthorized access deleting directory: {DirectoryPath}", directoryPath);
            throw new UnauthorizedAccessException(
                $"Error deleting directory due to permissions: {directoryPath}. {ex.Message}", ex);
        }
        catch (Exception ex) // General fallback
        {
            Log.Error(ex, "Generic error deleting directory: {DirectoryPath}", directoryPath);
            throw new Exception($"Error deleting directory: {directoryPath}", ex);
        }
    }

    public static bool WriteFile(string path, string content)
    {
        try
        {
            File.WriteAllText(path, content);
            Log.Information($"Data stored in {path}");
            return true;
        }
        catch (IOException ex) // Specific exception for I/O errors
        {
            Log.Error(ex, "IO error writing file: {Path}", path);
            throw new IOException($"Error writing file: {path}. {ex.Message}", ex);
        }
        catch (UnauthorizedAccessException ex) // Specific exception for permission issues
        {
            Log.Error(ex, "Unauthorized access writing file: {Path}", path);
            throw new UnauthorizedAccessException($"Error writing file due to permissions: {path}. {ex.Message}", ex);
        }
        catch (Exception ex) // General fallback
        {
            Log.Error(ex, "Generic error writing file: {Path}", path);
            throw new Exception($"Error writing file: {path}", ex);
        }
    }
}