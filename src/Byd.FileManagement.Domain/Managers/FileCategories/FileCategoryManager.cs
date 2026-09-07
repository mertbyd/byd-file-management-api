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
// islevi: Kategori adini kanoniklestirir, entity kurar ve mevcut entity alanlarini degistirir.
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
        CancellationToken cancellationToken)
    {
        return (await TryGetByIdAsync(id, throwIfNotExists: true, cancellationToken))!;
    }

    // Create modelini normalize edip yeni FileCategory entity'sine donusturur.
    public virtual FileCategory Create(FileCategoryCreateModel model, Guid? id = null)
    {
        ArgumentNullException.ThrowIfNull(model);
        var entity = new FileCategory(id ?? GuidGenerator.Create());
        Apply(entity, model);
        return entity;
    }

    // Update modelini normalize edip mevcut FileCategory alanlarina uygular.
    public virtual FileCategory Update(
        FileCategory entity,
        FileCategoryUpdateModel model)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentNullException.ThrowIfNull(model);
        Apply(entity, model);
        return entity;
    }

    // Dogrulanmis input alanlarini FileCategory veri kabuguna uygular.
    private void Apply(FileCategory entity, FileCategoryInputModel model)
    {
        entity.Name = NormalizeRequiredText(model.Name);
        entity.Code = model.Code;
    }
}
