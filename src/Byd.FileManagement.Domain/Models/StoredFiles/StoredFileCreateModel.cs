using System;

namespace Byd.FileManagement.Models.StoredFiles;

/// <summary>
/// Yeni saklanan dosya kaydinin domain create girdisini tanimlar.
/// </summary>
// islevi: Yukleme akisinin cagirandan aldigi uc alani Manager'in entity kurma akimina tasir.
// sistemdeki gorevi: Yukleyen taraf ile StoredFileManager arasindaki transporttan bagimsiz girdi sozlesmesidir.
// Burada olmayanlar Manager'in isi: ModuleName host beyanindan, TenantId ortam baglamindan,
// BlobName uretilir, ContentType dosya adindan cozulur, Provider ve durum modulun kararidir.
public sealed class StoredFileCreateModel
{
    public Guid ExternalId { get; set; }
    public Guid CategoryId { get; set; }
    public string FileName { get; set; } = default!;
}
