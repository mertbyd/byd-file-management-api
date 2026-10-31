using Byd.FileManagement.Entities.StoredFiles;
using Byd.FileManagement.Etos.StoredFiles;

namespace Byd.FileManagement.Extensions.StoredFiles;

/// <summary>
/// Tamamlanan dosya kaydini diger modullere giden olaya donusturur.
/// </summary>
// islevi: StoredFile alanlarini yeni bir StoredFileUploadCompletedEto'ya elle tasir.
// sistemdeki gorevi: Olayi kurmanin tek yeridir; dogrudan yukleme ve presigned yukleme ayni olayi buradan uretir.
public static class StoredFileExtensions
{
    // Kaydin kimlik, kapsam ve adres alanlarini tamamlanma olayina tasir.
    public static StoredFileUploadCompletedEto ToUploadCompletedEto(this StoredFile source)
    {
        return new StoredFileUploadCompletedEto
        {
            Id = source.Id,
            TenantId = source.TenantId,
            OrganizationId = source.OrganizationId,
            CategoryId = source.CategoryId,
            ExternalId = source.ExternalId,
            ModuleName = source.ModuleName,
            OriginalFileName = source.OriginalFileName
        };
    }
}
