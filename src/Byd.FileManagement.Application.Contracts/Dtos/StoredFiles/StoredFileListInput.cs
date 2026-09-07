using Byd.CoreExtensions.Querying;

namespace Byd.FileManagement.Dtos.StoredFiles;

/// <summary>
/// Saklanan dosya filtre, siralama ve sayfalama istegini tasir.
/// </summary>
// islevi: Tipli filtre modelini DataTablePaging ile tek public inputta birlestirir.
// sistemdeki gorevi: Filtreli liste ve sayim endpointlerinin dogrulanan request sozlesmesidir.
public class StoredFileListInput
    : DataTablePaging, IDataTablePagingProperty<StoredFileFilter>
{
    public StoredFileFilter? Filters { get; set; }
}
