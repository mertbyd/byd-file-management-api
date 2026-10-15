using System;
using Byd.FileManagement.Consts;
using Volo.Abp.EventBus;

namespace Byd.FileManagement.Etos.StoredFiles;

/// <summary>
/// Dosya icerigi ve kaydi tamamlandiginda yayinlanan dagitik olay.
/// </summary>
// islevi: Tamamlanan dosyanin kimligini, kapsamini ve hangi is kaydina bagli oldugunu tasir.
// sistemdeki gorevi: Dosya modulu ile dinleyen moduller arasindaki tel sozlesmesidir; dogrudan ve presigned
// yuklemenin ikisi de kayit kesinlestiginde ayni olayi yayinlar.
[EventName(StoredFileEventNames.UploadCompleted)]
public class StoredFileUploadCompletedEto
{
    // Tamamlanan dosya kaydinin kimligi.
    public Guid Id { get; set; }

    // Kaydin tenant'i; host kayitlari icin bos.
    public Guid? TenantId { get; set; }

    // Kaydin organizasyonu.
    public Guid? OrganizationId { get; set; }

    // Dosyanin bagli oldugu kategori.
    public Guid CategoryId { get; set; }

    // Dosyayi talep eden is kaydinin dis kimligi.
    public Guid ExternalId { get; set; }

    // Kaydi yazan modulun adi.
    public string ModuleName { get; set; } = default!;

    // Kullanicinin yukledigi ozgun dosya adi.
    public string OriginalFileName { get; set; } = default!;
}
