using System;
using System.Threading;
using System.Threading.Tasks;
using Byd.FileManagement.Entities.FileCategories;
using Byd.FileManagement.Interfaces.FileCategories;
using Byd.FileManagement.Managers.FileCategories;
using Byd.FileManagement.Models.FileCategories;

namespace Byd.FileManagement.EntityFrameworkCore.FileCategories;

/// <summary>
/// Testlerin dosya baglayabilecegi kalici bir kategori kurar.
/// </summary>
// islevi: Benzersiz adli bir kategori uretip veritabanina yazar ve kaydi geri verir.
// sistemdeki gorevi: Modul kategori seed etmedigi icin her testin kendi slotunu acmasi gerekir;
// bu da tuketicinin gercek davranisidir. Tekrarlanan kurulumu tek yerde tutar.
public static class FileCategoryTestDataBuilder
{
    // Yeni bir kategori kurar ve kalici hale getirir.
    public static async Task<FileCategory> InsertAsync(
        FileCategoryManager manager,
        IFileCategoryRepository repository,
        CancellationToken cancellationToken = default)
    {
        var category = manager.Create(new FileCategoryCreateModel
        {
            Name = $"Test kategorisi {Guid.NewGuid():N}"
        });
        await repository.InsertAsync(category, autoSave: true, cancellationToken: cancellationToken);

        return category;
    }
}
