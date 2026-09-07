using Byd.CoreExtensions.Interfaces;
using Byd.FileManagement.Entities.FileCategories;

namespace Byd.FileManagement.Interfaces.FileCategories;

/// <summary>
/// Dosya kategorisi aggregate'i icin extended repository sozlesmesini tanimlar.
/// </summary>
// islevi: FileCategory CRUD, filtreleme, sayim ve bulk yeteneklerini Domain katmanina soyutlar.
// sistemdeki gorevi: Kategori kullanim senaryolari ile EF Core uygulamasi arasindaki veri erisim siniridir.
public interface IFileCategoryRepository : IExtendedRepository<FileCategory>
{
}
