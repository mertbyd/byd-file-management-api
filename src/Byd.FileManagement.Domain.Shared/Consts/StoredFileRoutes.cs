namespace Byd.FileManagement.Consts;

/// <summary>
/// Saklanan dosya HTTP rotalarini tanimlar.
/// </summary>
// islevi: Dosya metadata okuma endpointlerinin kararli rota sozlesmesini tek yerde tutar.
// sistemdeki gorevi: Controller ile istemcilerin ayni derleme guvenli route degerlerini kullanmasini saglar.
public static class StoredFileRoutes
{
    public const string Root = "api/file-management/stored-files";
    public const string ById = "{id:guid}";
    public const string ByExternalIds = "by-external-ids";
    public const string ByCategory = "by-category";
    public const string ByCategoryWithContent = "by-category/with-content";
    public const string ByExternalIdAndCategoryWithContent = "by-external-id-and-category/with-content";
    public const string FilteredList = "filtered-list";
    public const string FilteredCount = "filtered-count";
    public const string Upload = "upload";
    public const string Download = "{id:guid}/download";
    public const string DownloadContent = "{id:guid}/download/content";
}
