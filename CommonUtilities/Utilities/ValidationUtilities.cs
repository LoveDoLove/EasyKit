namespace CommonUtilities.Utilities;

public static class ValidationUtilities
{
    public static bool IsValidHex(string szText)
    {
        if (string.IsNullOrEmpty(szText))
            return false;

        // Hex strings often have an even length (each byte is two hex chars).
        // This check can be added if strictness is required:
        // if (szText.Length % 2 != 0)
        // return false;

        foreach (char c in szText)
        {
            bool isHexChar = (c >= '0' && c <= '9') ||
                             (c >= 'a' && c <= 'f') ||
                             (c >= 'A' && c <= 'F');
            if (!isHexChar)
                return false;
        }

        return true;
    }
}