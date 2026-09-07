using Byd.CoreExtensions.Interfaces;
using Byd.FileManagement.Entities.StoredFiles;

namespace Byd.FileManagement.Interfaces.StoredFiles;

/// <summary>
/// Dosya durumu lookup'i icin extended repository sozlesmesini tanimlar.
/// </summary>
// islevi: StoredFileStatus CRUD, filtreleme, sayim ve bulk yeteneklerini Domain katmanina soyutlar.
// sistemdeki gorevi: Durum seed akisi ile EF Core uygulamasi arasindaki veri erisim siniridir.
public interface IStoredFileStatusRepository : IExtendedRepository<StoredFileStatus>
{
}
