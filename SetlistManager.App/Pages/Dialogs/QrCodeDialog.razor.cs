using Microsoft.AspNetCore.Components;
using MudBlazor;
using SetlistManager.App.Services;

namespace SetlistManager.App.Pages.Dialogs
{
    public partial class QrCodeDialog
    {
        [CascadingParameter]
        public required IMudDialogInstance MudDialog { get; set; }
        [Parameter]
        public string RoomUrl { get; set; } = string.Empty;
        [Inject]
        public required IQRService QRService { get; set; }

        private string _qrCodeImageSrc = string.Empty;

        protected override void OnInitialized()
        {
            
            if (!string.IsNullOrWhiteSpace(RoomUrl))
            {
                _qrCodeImageSrc = QRService.GenerateQrCode(RoomUrl);
            }            
        }

        private void Close()
        {
            MudDialog.Close();
        }
    }
}