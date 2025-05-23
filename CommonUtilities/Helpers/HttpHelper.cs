namespace CommonUtilities.Helpers;

/**
 * @author: LoveDoLove
 * @description: Get HTML content from URL
 */
public static class HttpHelper
{
    // Using a single static HttpClient instance is generally recommended for performance
    // to avoid socket exhaustion issues that can occur when creating many HttpClient instances.
    private static readonly HttpClient client = new();

    public static async Task<string> GetHtmlWithUrl(string url)
    {
        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode(); // Throws an exception if the HTTP response status is an error code.
            string receiptContent = await response.Content.ReadAsStringAsync();
            return receiptContent;
        }
        catch (HttpRequestException ex)
        {
            // Log the exception or handle it as needed
            // For example: Log.Error(ex, "Error fetching HTML from URL: {Url}", url);
            return string.Empty; // Or throw a custom exception, or rethrow ex
        }
    }
}