using System;
using System.Threading;
using System.Threading.Tasks;
using Byd.CoreExtensions.Managers;
using Byd.FileManagement.Entities.FileCategories;
using Byd.FileManagement.Interfaces.FileCategories;
using Byd.FileManagement.Models.FileCategories;

namespace Byd.FileManagement.Managers.FileCategories;

/// <summary>
/// Dosya kategorisi normalizasyon, varlik ve mutasyon kurallarini yonetir.
/// </summary>
// islevi: Kategori adini kanoniklestirir, ad benzersizligini dogrular, entity kurar ve mevcut
// entity alanlarini degistirir.
// sistemdeki gorevi: AppService'i is kararindan ve data-shell entity'yi davranistan uzak tutan domain sahibidir.
public class FileCategoryManager : BaseManager<FileCategory, Guid>
{
    // FileCategory extended repository'sini ortak guard tabanina devreder.
    public FileCategoryManager(IFileCategoryRepository repository)
        : base(repository)
    {
    }

    // Kimligi verilen kategoriyi getirir; yoksa BaseManager lokalize NotFound firlatir.
    public virtual async Task<FileCategory> EnsureExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return (await TryGetByIdAsync(id, throwIfNotExists: true, cancellationToken))!;
    }

    // Create modelini normalize edip yeni FileCategory entity'sine donusturur.
    public virtual async Task<FileCategory> CreateAsync(
        FileCategoryCreateModel model,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(model);
        var entity = new FileCategory(GuidGenerator.Create());
        await ApplyAsync(entity, model, cancellationToken);

        return entity;
    }

    // Update modelini normalize edip mevcut FileCategory alanlarina uygular.
    public virtual async Task<FileCategory> UpdateAsync(
        FileCategory entity,
        FileCategoryUpdateModel model,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentNullException.ThrowIfNull(model);
        await ApplyAsync(entity, model, cancellationToken);

        return entity;
    }

    // Dogrulanmis input alanlarini FileCategory veri kabuguna uygular.
    // Ad, tuketicinin dosyalarini adresledigi anahtardir: ayni ad ikinci kez acilirsa dosyalar
    // fark edilmeden iki kategoriye bolunur, bu yuzden benzersizlik yazma aninda zorlanir.
    private async Task ApplyAsync(
        FileCategory entity,
        FileCategoryInputModel model,
        CancellationToken cancellationToken = default)
    {
        var name = NormalizeRequiredText(model.Name);
        await CheckAlreadyExistsAsync(other => other.Name == name, entity.Id, cancellationToken);
        entity.Name = name;
        entity.Code = model.Code;
        entity.MaxFileSize = model.MaxFileSize;
    }
}
