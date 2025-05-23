using System.Security.Cryptography;

namespace CommonUtilities.Utilities;

public static class SHA256Utilities
{
    public static string ComputeSHA256Hash(string key)
    {
        try
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                byte[] hashBuffer = sha256.ComputeHash(keyBytes);
                return WriteHex(hashBuffer).ToLower();
            }
        }
        catch (Exception ex)
        {
            // Rethrow the original exception to preserve stack trace and type
            throw new CryptographicException($"Error computing SHA256 hash: {ex.Message}", ex);
        }
    }

    private static string WriteHex(byte[] array)
    {
        if (array == null)
            return string.Empty;

        StringBuilder hex = new StringBuilder(array.Length * 2);
        foreach (byte b in array) hex.AppendFormat("{0:X2}", b);
        return hex.ToString();
    }
}