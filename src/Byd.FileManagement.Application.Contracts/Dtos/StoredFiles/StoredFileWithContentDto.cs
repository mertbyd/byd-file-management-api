namespace Byd.FileManagement.Dtos.StoredFiles;

/// <summary>
/// Saklanan dosya kaydini gercek byte icerigiyle birlikte tasir.
/// </summary>
// islevi: StoredFileDto'nun metadata alanlarina blob depodan okunan byte'lari ekler.
// sistemdeki gorevi: Kategori tabanli toplu dosya okuma endpointlerinin response sozlesmesidir.
// mirastan gelen: StoredFileDto -> Id, TenantId, OrganizationId, CategoryId, CategoryName,
//                 ExternalId, ModuleName, OriginalFileName, CreationTime, LastModificationTime.
// burada yazilan: yalnizca icerik.
public class StoredFileWithContentDto : StoredFileDto
{
    // Blob depodan okunan dosya icerigi; JSON cevabinda Base64 metin olarak gorunur.
    public byte[] Data { get; set; } = default!;
}
