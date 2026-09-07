using System;
using Byd.CoreExtensions.Interfaces;
using Volo.Abp.Domain.Entities.Auditing;

namespace Byd.FileManagement.Entities.FileCategories;

/// <summary>
/// Dosyalarin siniflandirildigi kategori kaydini tasir.
/// </summary>
// islevi: Kategorinin adini ve istege bagli is kodunu kalici olarak tutar.
// sistemdeki gorevi: Kurallari FileCategoryManager'a birakan ince lookup persistence kabugudur.
// Bir is kaydinin hangi slotuna ait oldugunu belirler; StoredFile buna baglanir.
public class FileCategory : CreationAuditedEntity<Guid>, ILookup
{
    // Kategorinin kullaniciya gosterilen adidir.
    public string Name { get; internal set; } = default!;

    // Is tarafinin istege bagli kategori kodudur.
    public int? Code { get; internal set; }

    // EF Core materializasyonu icin bos kurucudur.
    protected FileCategory()
    {
    }

    // Manager tarafindan uretilen kimlikle bos veri kabugunu kurar.
    internal FileCategory(Guid id)
        : base(id)
    {
    }

    // ILookup yazma yuzeyini Domain disina public entity mutasyonu acmadan uygular.
    string ILookup.Name
    {
        get => Name;
        set => Name = value;
    }

    // ILookup kod yazma yuzeyini Domain disina public entity mutasyonu acmadan uygular.
    int? ILookup.Code
    {
        get => Code;
        set => Code = value;
    }
}
