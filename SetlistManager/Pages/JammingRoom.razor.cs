namespace SetlistManager.Pages;
using Microsoft.AspNetCore.Components;
using SetlistManager.Services;
using SetlistManager.Common.Models;
using MudBlazor;
using System.Runtime.CompilerServices;

public partial class JammingRoom
{
    [Inject]
    public required IDialogService DialogService { get; set; }

    private async Task OpenDialogAsync()
    {
        var options = new DialogOptions { BackgroundClass = "BackgroundBlur" };
        var dialog = DialogService.ShowAsync<JammingRoom>("Join or Create a Room", options);
        var result = dialog.Result;

        //if (dialog.Result.Dismiss(result.Result.Result))
        //{
        //    var dialogResult = (bool)result.Result.Result.Data;
        //    if (dialogResult)
        //    {
        //        // Handle 'Join' button click logic here
        //        // You can pass the room code data here if needed.
        //    }
        //    else
        //    {
        //        // Handle 'Create' button click logic here.
        //    }
        //}
    }

    private bool showRoomCodeInput = false;
    private string roomCode;

    private void OnJoinClick()
    {
        showRoomCodeInput = true;
    }

    private void OnCreateClick()
    {
        showRoomCodeInput = false;
    }

    private void SubmitRoomCode()
    {
        if (!string.IsNullOrEmpty(roomCode))
        {
        }
    }
}
