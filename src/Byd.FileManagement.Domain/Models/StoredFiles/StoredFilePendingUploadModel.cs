using System;
using Volo.Abp.MultiTenancy;

namespace Byd.FileManagement.Models.StoredFiles;

/// <summary>
/// Yuklemesi baslatilmis ama henuz tamamlanmamis dosyanin bilgisini tasir.
/// </summary>
// islevi: Presigned istek aninda Manager'in kurdugu kaydin alanlarini, kayit acilmadan saklanabilir hale getirir.
// sistemdeki gorevi: Onbellekte bekleyen yukleme ile MinIO'nun "yazildi" olayi arasindaki tek sozlesmedir;
// olay gelince Manager satiri bu modelden kurar. Alanlar public set'tir cunku onbellek JSON ile yazar ve okur.
//
// NEDEN IgnoreMultiTenancy. Anahtar blob adidir ve blob adi kayit kimliginden uretildigi icin tum
// tenant'larda tekildir. Olay isleyicisi onbellegi tenant'a gecmeden okuyabilsin diye anahtar tenant'siz tutulur;
// tenant bilgisi modelin kendi icinde tasinir.
[IgnoreMultiTenancy]
public sealed class StoredFilePendingUploadModel
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? OrganizationId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid ExternalId { get; set; }
    public string ModuleName { get; set; } = default!;
    public string BlobName { get; set; } = default!;
    public string OriginalFileName { get; set; } = default!;
}
