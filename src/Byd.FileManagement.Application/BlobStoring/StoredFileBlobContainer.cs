using Byd.FileManagement.Consts;
using Volo.Abp.BlobStoring;

namespace Byd.FileManagement.BlobStoring;

/// <summary>
/// Modulun byte'lari yazdigi blob container'ini isaretler.
/// </summary>
// islevi: ABP BlobStoring'e bu module ait tipli container'i tanitir.
// sistemdeki gorevi: Varsayilan container'a dokunmadan modulun kendi depolama alanini ayirir;
// tuketen host kendi varsayilan container'ini serbestce yapilandirabilir.
[BlobContainerName(StoredFileConsts.BlobContainerName)]
public class StoredFileBlobContainer
{
}
