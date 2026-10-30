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
// sistemdeki gorevi: Modul kategori seed etmedigi icin her testin kendi kategorisini acmasi
// gerekir; bu da tuketicinin gercek davranisidir. Tekrarlanan kurulumu tek yerde tutar.
public static class FileCategoryTestDataBuilder
{
    // Yeni bir kategori kurar ve kalici hale getirir; azami boyut verilmezse modulun varsayilani gecer.
    public static async Task<FileCategory> InsertAsync(
        FileCategoryManager manager,
        IFileCategoryRepository repository,
        long? maxFileSize = null,
        CancellationToken cancellationToken = default)
    {
        var category = await manager.CreateAsync(
            new FileCategoryCreateModel { Name = $"Test kategorisi {Guid.NewGuid():N}", MaxFileSize = maxFileSize },
            cancellationToken);
        await repository.InsertAsync(category, autoSave: true, cancellationToken: cancellationToken);

        return category;
    }
}
