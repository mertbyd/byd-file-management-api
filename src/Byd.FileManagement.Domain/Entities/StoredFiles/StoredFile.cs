using System;
using Byd.FileManagement.Entities.FileCategories;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Byd.FileManagement.Entities.StoredFiles;

/// <summary>
/// Saklanan bir dosyanin metadata kaydini tasir.
/// </summary>
// islevi: Dosyanin hangi modul ve kategori icin, hangi blob adiyla durdugunu kalici olarak tutar.
// sistemdeki gorevi: Kurallari StoredFileManager'a birakan veri kabugudur; dosya icerigi burada degil, saglayicida durur.
//
// IMultiTenant: dosya, son kullanicinin okudugu is verisidir; core'daki ApiKey ve ModuleSetting
// gibi host kapsamli bir ayar kaydi degildir. Arayuz ABP'nin global tenant filtresini acar;
// filtre olmadan (ExternalId, CategoryId) adresini bilen bir tenant baska tenant'in dosyasini okurdu.
public class StoredFile : AuditedAggregateRoot<Guid>, IMultiTenant
{
    // Kaydin ait oldugu tenant; ABP global tenant filtresi bu kolon uzerinden calisir.
    public Guid? TenantId { get; internal set; }
    // Kaydin ait oldugu organizasyon; kapsam bilgisi olarak tasinir.
    public Guid? OrganizationId { get; internal set; }
    // Kaydin onay durumu; okuma uclari yalniz Onayli kayitlari dondurur.
    public Guid StatusId { get; internal set; }
    // DTO'daki StatusId-StatusName ciftinin guncel ad kaynagidir.
    public StoredFileStatus Status { get; internal set; } = default!;
    // Dosyanin bagli oldugu FileCategory kaydinin kimligi.
    public Guid CategoryId { get; internal set; }
    // DTO'daki CategoryId-CategoryName ciftinin guncel ad kaynagidir.
    public FileCategory Category { get; internal set; } = default!;
    // Dosyayi talep eden is kaydinin dis kimligi.
    public Guid ExternalId { get; internal set; }
    // Kaydi yazan modulun adi; DTO'dan degil, CurrentModuleNameProvider'dan gelir.
    public string ModuleName { get; internal set; } = default!;
    // Icerigin saglayicidaki blob adi. Bugun Id'den turetilir ama saklanir: adlandirma semasi
    // degisirse eski kayitlar kendi adlariyla cozulmeye devam eder.
    public string BlobName { get; internal set; } = default!;
    // Kullanicinin yukledigi ozgun dosya adi.
    public string OriginalFileName { get; internal set; } = default!;
    // TODO: SubOrganizationId - alt organizasyon kirilimi gerektiginde eklenecek.
    //       Su an hicbir tuketici bu seviyede filtrelemiyor; kolon acilmadi.

    // EF Core materializasyonu icin parametresiz ctor.
    protected StoredFile()
    {
    }

    // Manager tarafindan uretilen kimlikle bos veri kabugunu kurar.
    internal StoredFile(Guid id)
        : base(id)
    {
    }
}
