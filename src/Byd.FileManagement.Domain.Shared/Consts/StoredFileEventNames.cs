namespace Byd.FileManagement.Consts;

/// <summary>
/// Saklanan dosya modulunun dinledigi dagitik olay adlarini tanimlar.
/// </summary>
// islevi: Olay adlarini derleme guvenli sabitlerde tutar.
// sistemdeki gorevi: Yayinlayan modul ile dosya modulunun ayni tel adinda bulusmasinin tek kaynagidir.
public static class StoredFileEventNames
{
    // Bir modul is kayitlarini sildiginde yayinlar; dosya modulu o kayitlarin dosyalarini siler.
    public const string ExternalRecordsDeleted = "FileManagement.StoredFiles.ExternalRecordsDeleted";
}
