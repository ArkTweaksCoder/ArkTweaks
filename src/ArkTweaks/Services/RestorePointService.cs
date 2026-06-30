using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace ArkTweaks.Services;

public class RestorePointService
{
    private readonly ILogger<RestorePointService> _logger;

    public RestorePointService(ILogger<RestorePointService> logger)
    {
        _logger = logger;
    }

    public class RestorePointResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public void CreateRestorePoint()
    {
        try
        {
            // For MVP, use a simplified approach
            // In production, this would use SRSetRestorePoint API
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = "-Command \"Checkpoint-Computer -Description 'ArkTweaks Restore Point'\"",
                Verb = "runas",
                UseShellExecute = true,
                CreateNoWindow = true
            };

            System.Diagnostics.Process.Start(startInfo);
            _logger.LogInformation("Restore point creation initiated");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create restore point");
            throw;
        }
    }

    public async Task<RestorePointResult> CreateRestorePointAsync(string description)
    {
        return await Task.Run(() =>
        {
            try
            {
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command \"Checkpoint-Computer -Description '{description}'\"",
                    Verb = "runas",
                    UseShellExecute = true,
                    CreateNoWindow = true
                };

                var process = System.Diagnostics.Process.Start(startInfo);
                process?.WaitForExit();
                
                return new RestorePointResult
                {
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create restore point");
                return new RestorePointResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        });
    }
}
