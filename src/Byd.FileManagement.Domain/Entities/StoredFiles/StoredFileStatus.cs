using System;
using Byd.CoreExtensions.Interfaces;
using Volo.Abp.Domain.Entities.Auditing;

namespace Byd.FileManagement.Entities.StoredFiles;

/// <summary>
/// Saklanan dosyanin onay durumunu tanimlayan lookup kaydini tasir.
/// </summary>
// islevi: Durumun adini ve is kodunu kalici olarak tutar.
// sistemdeki gorevi: Kurallari StoredFileStatusManager'a birakan ince lookup kabugudur; FileCategory
// ile ayni lookup seklini kullanir. Iki sistem durumu seed ile sabit kimliklerle yazilir.
public class StoredFileStatus : CreationAuditedEntity<Guid>, ILookup
{
    // Durumun kullaniciya gosterilen adidir.
    public string Name { get; internal set; } = default!;

    // Is tarafinin istege bagli durum kodudur.
    public int? Code { get; internal set; }

    // EF Core materializasyonu icin bos kurucudur.
    protected StoredFileStatus()
    {
    }

    // Manager tarafindan uretilen kimlikle bos veri kabugunu kurar.
    internal StoredFileStatus(Guid id)
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
