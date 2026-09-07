namespace Byd.FileManagement.Consts;

/// <summary>
/// Modulun tuketen host'un appsettings dosyasindan okudugu depolama anahtarlarini tanimlar.
/// </summary>
// islevi: Depolama baglanti anahtarlarini derleme guvenli sabitlerde tutar.
// sistemdeki gorevi: Altyapi yapilandirmasini modul kendi okur; tuketen host yalniz bu bolumu
// appsettings'e koyar, kablolama kodu yazmaz.
public static class FileStorageConsts
{

    private const string Section = "FileManagement:FileStorage:";

    public const string EndPointKey = Section + "EndPoint";
    public const string AccessKeyKey = Section + "AccessKey";
    public const string SecretKeyKey = Section + "SecretKey";
    public const string BucketNameKey = Section + "BucketName";
    public const string WithSslKey = Section + "WithSSL";
    public const string CreateBucketIfNotExistsKey = Section + "CreateBucketIfNotExists";
}
