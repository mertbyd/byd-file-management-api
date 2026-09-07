using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Byd.FileManagement.Entities.StoredFiles;

/// <summary>
/// Saklanan bir dosyanin metadata kaydini tasir.
/// </summary>
// islevi: Dosyanin hangi modul ve slot icin, hangi saglayicida, hangi blob adiyla durdugunu kalici tutar.
// sistemdeki gorevi: Kurallari StoredFileManager'a birakan veri kabugudur; dosya icerigi burada degil,
// saglayicida durur.
//
// Sahiplik alanlari (TenantId, OrganizationId) yalnizca tasinan veridir; global sorgu filtresine
// baglanmaz. Kardes core entity'leri ApiKey ve ModuleSetting de TenantId'yi ayni sekilde duz
// kapsam kolonu olarak tutar.
public class StoredFile : AuditedAggregateRoot<Guid>
{
    // Kaydi yazan modulun adi; DTO'dan degil, CurrentModuleNameProvider'dan gelir.
    public string ModuleName { get; internal set; } = default!;

    // Dosyayi talep eden is kaydinin dis kimligi.
    public Guid ExternalId { get; internal set; }

    // Dosyanin bagli oldugu FileCategory slotunun kimligi.
    public Guid CategoryId { get; internal set; }


    // Icerigin saglayicidaki blob adi.
    public string BlobName { get; internal set; } = default!;

    // Kullanicinin yukledigi ozgun dosya adi.
    public string OriginalFileName { get; internal set; } = default!;


    // Kaydin ait oldugu tenant; kapsam bilgisi olarak tasinir.
    public Guid? TenantId { get; internal set; }

    // Kaydin ait oldugu organizasyon; kapsam bilgisi olarak tasinir.
    public Guid? OrganizationId { get; internal set; }

    // TODO: SubOrganizationId - alt organizasyon kirilimi gerektiginde eklenecek.
    //       Su an hicbir tuketici bu seviyede filtrelemiyor; kolon acilmadi.

    // EF Core materializasyonu icin bos kurucudur.
    protected StoredFile()
    {
    }

    // Manager tarafindan uretilen kimlikle bos veri kabugunu kurar.
    internal StoredFile(Guid id)
        : base(id)
    {
    }
}
