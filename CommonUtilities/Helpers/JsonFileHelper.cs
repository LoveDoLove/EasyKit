using System.Text.Json;
using CommonUtilities.Utilities;

namespace CommonUtilities.Helpers;

public static class JsonFileHelper
{
    private const string SupportedFileType = ".json";

    public static string ReadJsonFile(string filePath, string? aesKey = null, string? iv = null)
    {
        try
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException($"File not found: {filePath}", filePath);

            string fileType = Path.GetExtension(filePath);
            if (!string.Equals(fileType, SupportedFileType, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"Invalid file type: {fileType}. Only {SupportedFileType} is supported.",
                    nameof(filePath));

            string fileData = File.ReadAllText(filePath);

            if (aesKey != null && iv != null) // Decryption is requested
            {
                if (string.IsNullOrEmpty(fileData))
                    // Cannot decrypt an empty file/content
                    throw new JsonException($"File content is empty and cannot be decrypted: {filePath}");
                return AesUtilities.Aes256CbcDecrypt(fileData, aesKey, iv);
            }

            // No decryption requested, return fileData as is (could be empty)
            return fileData;
        }
        catch (JsonException) // Rethrow JsonException as it's specific
        {
            throw;
        }
        catch (Exception e)
        {
            // Wrap the original exception to preserve stack trace and provide context
            throw new IOException($"Error reading or decrypting file: {filePath}. Details: {e.Message}", e);
        }
    }

    public static bool SaveFileAsJson(string filePath, object data, string? aesKey = null, string? iv = null)
    {
        try
        {
            string serializedData = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });

            if (aesKey == null || iv == null)
            {
                FileUtilities.WriteFile(filePath, serializedData);
                return true;
            }

            // Use the provided aesKey and iv, not Common.Common.AesKey
            string encryptedData = AesUtilities.Aes256CbcEncrypt(serializedData, aesKey, iv);

            FileUtilities.WriteFile(filePath, encryptedData);

            return true;
        }
        catch (Exception e)
        {
            throw new IOException($"Error serializing, encrypting, or saving file: {filePath}. Details: {e.Message}",
                e);
        }
    }
}