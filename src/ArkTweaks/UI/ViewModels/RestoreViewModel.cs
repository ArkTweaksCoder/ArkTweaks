using System;
using Microsoft.Extensions.Logging;
using ArkTweaks.Services;

namespace ArkTweaks.UI.ViewModels;

public class RestoreViewModel : BaseViewModel
{
    private readonly RestorePointService _restorePointService;

    public RestoreViewModel(ILogger<RestoreViewModel> logger, RestorePointService restorePointService) 
        : base(logger)
    {
        _restorePointService = restorePointService;
    }

    public void CreateRestorePoint()
    {
        try
        {
            _restorePointService.CreateRestorePoint();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to create restore point");
        }
    }
}
