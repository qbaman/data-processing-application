using System.Threading;
using System.Threading.Tasks;

namespace FBZSystemMvc.Services.DatasetUpdates;

public interface IDatasetUpdateService
{
    DatasetUpdateStatus GetStatus();
    Task<(bool Updated, string Message)> CheckAndUpdateAsync(bool force, CancellationToken ct = default);
}