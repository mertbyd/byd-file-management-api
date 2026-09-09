using System;

namespace Byd.FileManagement.Dtos.StoredFiles;

/// <summary>
/// Saklanan dosya kaydini gercek byte icerigiyle birlikte tasir.
/// </summary>
// islevi: StoredFile metadata alanlariyla blob depodan okunan byte'lari tek public cevapta birlestirir.
// sistemdeki gorevi: Kategori tabanli toplu dosya okuma endpointlerinin response sozlesmesidir.
public class StoredFileWithContentDto
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? OrganizationId { get; set; }
    public Guid StatusId { get; set; }
    public string StatusName { get; set; } = default!;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
    public Guid ExternalId { get; set; }
    public string ModuleName { get; set; } = default!;
    public string BlobName { get; set; } = default!;
    public string OriginalFileName { get; set; } = default!;
    public DateTime CreationTime { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public byte[] Data { get; set; } = default!;
}
