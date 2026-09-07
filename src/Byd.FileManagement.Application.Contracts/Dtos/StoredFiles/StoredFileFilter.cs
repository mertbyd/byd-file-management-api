using Byd.CoreExtensions.Attributes;
using Byd.CoreExtensions.Querying;
using Byd.FileManagement.Consts;

namespace Byd.FileManagement.Dtos.StoredFiles;

/// <summary>
/// Saklanan dosya okuma uclarinin filtre alanlarini tasir.
/// </summary>
// islevi: Her filtre propertysini kararli tel adi ve entity property yoluyla DataTable motoruna tanitir.
// sistemdeki gorevi: Dosya metadata liste ve sayim uclarinin tipli filtre izin listesidir.
public class StoredFileFilter
{
    [DataTable(StoredFileFilterFields.ModuleName, StoredFileFilterFields.PropertyPaths.ModuleName)]
    public DataTableFilterItem? ModuleName { get; set; }

    [DataTable(StoredFileFilterFields.ExternalId, StoredFileFilterFields.PropertyPaths.ExternalId)]
    public DataTableFilterItem? ExternalId { get; set; }

    [DataTable(StoredFileFilterFields.CategoryId, StoredFileFilterFields.PropertyPaths.CategoryId)]
    public DataTableFilterItem? CategoryId { get; set; }


    [DataTable(StoredFileFilterFields.TenantId, StoredFileFilterFields.PropertyPaths.TenantId)]
    public DataTableFilterItem? TenantId { get; set; }

    [DataTable(StoredFileFilterFields.CreationTime, StoredFileFilterFields.PropertyPaths.CreationTime)]
    public DataTableFilterItem? CreationTime { get; set; }
}
