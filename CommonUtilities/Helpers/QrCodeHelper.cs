using QRCoder;

namespace CommonUtilities.Helpers;

/*
 * @author: LoveDoLove
 * @description:
 * This service class generates QR codes.
 * It provides a method to create a QR code from a given text and return it as a Base64 string.
 */
public static class QrCodeHelper
{
    public static string GenerateQrCode(string url)
    {
        QRCodeGenerator qrCodeGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
        Base64QRCode qrCode = new Base64QRCode(qrCodeData);
        string qrCodeImageAsBase64 = qrCode.GetGraphic(20);
        return qrCodeImageAsBase64;
    }
}