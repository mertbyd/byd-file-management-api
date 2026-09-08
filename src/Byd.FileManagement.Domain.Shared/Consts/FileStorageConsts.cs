namespace Byd.FileManagement.Consts;

/// <summary>
/// Modulun tuketen host'un appsettings dosyasindan okudugu depolama anahtarlarini tanimlar.
/// </summary>
// islevi: Depolama ayar bolumunun adini ve tam anahtar yollarini derleme guvenli sabitlerde tutar.
// sistemdeki gorevi: Altyapi yapilandirmasini modul kendi okur; tuketen host yalniz bu bolumu
// appsettings'e koyar, kablolama kodu yazmaz. Anahtar yollari eksik ayar hatalarinda da kullanilir.
public static class FileStorageConsts
{
    // Modulun depolama bilgilerini aradigi ayar bolumu; bolum baglamasi bu adla yapilir.
    public const string SectionName = "FileManagement:FileStorage";

    private const string Section = SectionName + ":";

    public const string EndPointKey = Section + "EndPoint";
    public const string AccessKeyKey = Section + "AccessKey";
    public const string SecretKeyKey = Section + "SecretKey";
    public const string BucketNameKey = Section + "BucketName";
    public const string WithSslKey = Section + "WithSSL";
    public const string CreateBucketIfNotExistsKey = Section + "CreateBucketIfNotExists";
}
