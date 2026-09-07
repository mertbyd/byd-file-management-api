using System;

namespace Byd.FileManagement.Dtos.StoredFiles;

/// <summary>
/// Kalici saklanan dosya kaydinin public cevap alanlarini tasir.
/// </summary>
// islevi: Dosya kimligini, kapsamini, saglayici yerlesimini ve durumunu HTTP ciktisina acar.
// sistemdeki gorevi: Entity veya EF tiplerini disariya sizdirmayan response sozlesmesidir.
public class StoredFileDto
{
    public Guid Id { get; set; }
    public Guid ExternalId { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? OrganizationId { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
    public string ModuleName { get; set; } = default!;
    public string BlobName { get; set; } = default!;
    public string OriginalFileName { get; set; } = default!;
    public DateTime CreationTime { get; set; }
    public DateTime? LastModificationTime { get; set; }
}
