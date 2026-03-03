using System;

namespace FBZSystemMvc.Services.DatasetUpdates;

public interface IDatasetReloadable
{
    string DataFolderPath { get; }
    DateTime LoadedUtc { get; }
    void Reload();
}