// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using System.Text.Json;

namespace JPSoftworks.ChangeCaseExtension.Helpers;

/// <summary>
/// Abstract base class for settings managers that provides async saving, backup management, 
/// and atomic file operations.
/// </summary>
/// <typeparam name="TModel">The model type to serialize/deserialize</typeparam>
internal abstract class SettingsManagerBase<TModel> : IDisposable where TModel : class, new()
{
    private readonly SemaphoreSlim _saveSemaphore = new(1, 1);
    private CancellationTokenSource? _currentSaveOperation;
    private volatile bool _disposed;

    protected string FilePath { get; }
    protected string BackupFilePath { get; }
    protected string TempFilePath { get; }

    protected abstract JsonSerializerOptions JsonOptions { get; }

    protected SettingsManagerBase(string filePath)
    {
        this.FilePath = filePath;
        this.BackupFilePath = filePath + ".backup";
        this.TempFilePath = filePath + ".tmp";
    }

    /// <summary>
    /// Creates the model from current state. Must be thread-safe.
    /// </summary>
    protected abstract TModel CreateModel();

    /// <summary>
    /// Applies the loaded model to current state. Must be thread-safe.
    /// </summary>
    protected abstract void ApplyModel(TModel model);

    /// <summary>
    /// Clears the current state. Must be thread-safe.
    /// </summary>
    protected abstract void ClearState();

    /// <summary>
    /// Called when settings are successfully loaded.
    /// </summary>
    protected virtual void OnSettingsLoaded() { }

    /// <summary>
    /// Called when settings fail to load and state is cleared.
    /// </summary>
    protected virtual void OnSettingsLoadFailed() { }

    /// <summary>
    /// Called before saving settings. Return false to cancel the save.
    /// </summary>
    protected virtual bool OnBeforeSave() => true;

    /// <summary>
    /// Called after settings are successfully saved.
    /// </summary>
    protected virtual void OnAfterSave() { }

    public void LoadSettings()
    {
        try
        {
            // Try to load from main file first
            if (this.TryLoadFromFile(this.FilePath))
            {
                this.OnSettingsLoaded();
                return;
            }

            // If main file fails, try backup
            if (this.TryLoadFromFile(this.BackupFilePath))
            {
                Logger.LogWarning("Loaded settings from backup file");
                this.OnSettingsLoaded();
                return;
            }

            // If both fail, clear state
            this.ClearState();
            this.OnSettingsLoadFailed();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            this.ClearState();
            this.OnSettingsLoadFailed();
        }
        finally
        {
            // Clean up any leftover temp files
            this.CleanupTempFiles();
        }
    }

    private bool TryLoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            return false;

        try
        {
            var json = File.ReadAllText(filePath);
            var model = JsonSerializer.Deserialize<TModel>(json, this.JsonOptions);
            
            if (model != null)
            {
                this.ApplyModel(model);
                return true;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to load from {filePath}: {ex.Message}");
        }

        return false;
    }

    public async Task SaveSettingsAsync(CancellationToken cancellationToken = default)
    {
        if (this._disposed)
            return;

        if (!this.OnBeforeSave())
            return;

        // Cancel any existing save operation
        var cancellation = this._currentSaveOperation?.CancelAsync();
        if (cancellation != null)
        {
            await cancellation;
        }
        
        using var newSaveOperation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        this._currentSaveOperation = newSaveOperation;

        try
        {
            // Wait for exclusive access to save operation
            await this._saveSemaphore.WaitAsync(newSaveOperation.Token);
            
            try
            {
                // Check if this operation was cancelled while waiting
                newSaveOperation.Token.ThrowIfCancellationRequested();

                var model = this.CreateModel();
                var json = JsonSerializer.Serialize(model, this.JsonOptions);

                // Atomic save operation
                await this.SaveAtomicallyAsync(json, newSaveOperation.Token);

                this.OnAfterSave();
            }
            finally
            {
                this._saveSemaphore.Release();
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when a newer save operation cancels this one
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
        finally
        {
            if (this._currentSaveOperation == newSaveOperation)
            {
                this._currentSaveOperation = null;
            }
        }
    }

    private async Task SaveAtomicallyAsync(string json, CancellationToken cancellationToken)
    {
        // Step 1: Write to temporary file
        await File.WriteAllTextAsync(this.TempFilePath, json, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();

        // Step 2: Create backup if main file exists
        if (File.Exists(this.FilePath))
        {
            File.Copy(this.FilePath, this.BackupFilePath, overwrite: true);
        }
        cancellationToken.ThrowIfCancellationRequested();

        // Step 3: Atomically replace main file with temp file
        File.Move(this.TempFilePath, this.FilePath, overwrite: true);
        
        // Step 4: Clean up old backup files (keep only the most recent)
        this.CleanupOldBackups();
    }

    public void SaveSettings()
    {
        // Synchronous wrapper that blocks
        try
        {
            this.SaveSettingsAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
    }

    protected async Task SaveSettingsAsyncInternal()
    {
        // Protected method for derived classes to trigger saves
        await this.SaveSettingsAsync();
    }

    private void CleanupTempFiles()
    {
        try
        {
            if (File.Exists(this.TempFilePath))
            {
                File.Delete(this.TempFilePath);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to cleanup temp file: {ex.Message}");
        }
    }

    private void CleanupOldBackups()
    {
        try
        {
            var directory = Path.GetDirectoryName(this.FilePath);
            if (directory == null) return;

            var backupPattern = Path.GetFileName(this.BackupFilePath);
            var backupFiles = Directory.GetFiles(directory, backupPattern + "*")
                .OrderByDescending(File.GetCreationTime)
                .Skip(1);

            foreach (var oldBackup in backupFiles)
            {
                File.Delete(oldBackup);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to cleanup old backups: {ex.Message}");
        }
    }

    public async Task CreateBackupAsync(CancellationToken cancellationToken = default)
    {
        if (File.Exists(this.FilePath))
        {
            var timestampedBackup = $"{this.BackupFilePath}.{DateTime.UtcNow:yyyyMMdd_HHmmss}";
            await Task.Run(() => File.Copy(this.FilePath, timestampedBackup, overwrite: false), cancellationToken);
        }
    }

    public async Task RestoreFromBackupAsync(CancellationToken cancellationToken = default)
    {
        if (File.Exists(this.BackupFilePath))
        {
            await Task.Run(() => File.Copy(this.BackupFilePath, this.FilePath, overwrite: true), cancellationToken);
            this.LoadSettings();
        }
    }

    public virtual void Dispose()
    {
        if (this._disposed)
            return;

        this._disposed = true;

        // Cancel any pending save operations
        this._currentSaveOperation?.Cancel();

        // Perform final synchronous save
        try
        {
            this.SaveSettings();
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to save settings during disposal: {ex.Message}");
        }

        // Dispose resources
        this._currentSaveOperation?.Dispose();
        this._saveSemaphore?.Dispose();
    }
}
