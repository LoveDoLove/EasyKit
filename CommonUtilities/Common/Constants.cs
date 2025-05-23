namespace CommonUtilities.Common;

public class Constants
{
    // WARNING: Using a fixed IV like this for cryptographic operations (e.g., AES-CBC, TripleDES-CBC)
    // is insecure if used for multiple messages with the same key. 
    // IVs should ideally be unique and randomly generated for each encryption operation.
    // This default might be suitable for specific, controlled scenarios or testing.
    public const string DefaultIv = "0000000000000000";

    public const int MaxUrlLength = 100;

    public const string ConsoleSplitLine =
        "------------------------------------------------------------------------------------";
}