namespace Byd.FileManagement.Consts;

/// <summary>
/// Dosya kategorisi HTTP rotalarini tanimlar.
/// </summary>
// islevi: FileCategory CRUD endpointlerinin kararli rota sozlesmesini tek yerde tutar.
// sistemdeki gorevi: Controller ile istemcilerin ayni derleme guvenli route degerlerini kullanmasini saglar.
public static class FileCategoryRoutes
{
    // Dosya kategorisi endpoint ailesinin kok rotasidir.
    public const string Root = "api/file-management/file-categories";

    // Kimlik alan CRUD endpointlerinin rota kalibidir.
    public const string ById = "{id:guid}";
}
