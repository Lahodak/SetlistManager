namespace SetlistManager.App.Services;

public interface IQRService
{
    string GenerateQrCode(string roomUrl);
}