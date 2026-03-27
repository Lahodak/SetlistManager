using QRCoder;

namespace SetlistManager.App.Services.Implementations;

public class QRService : IQRService
{
    private const int _pixelsPerModule = 20;

    public string GenerateQrCode(string roomUrl)
    {
        using QRCodeGenerator generator = new();
        using QRCodeData qrCodeData = generator.CreateQrCode(roomUrl, QRCodeGenerator.ECCLevel.Q);
        using PngByteQRCode qrCode = new(qrCodeData);

        byte[] qrCodeImage = qrCode.GetGraphic(_pixelsPerModule);
        string base64String = Convert.ToBase64String(qrCodeImage);

        return $"data:image/png;base64,{base64String}";
    }    
}