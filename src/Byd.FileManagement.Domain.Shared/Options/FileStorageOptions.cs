namespace Byd.FileManagement.Options;

/// <summary>
/// Modulun blob container'ini baglarken kullandigi depolama bilgilerini tasir.
/// </summary>
// islevi: MinIO baglanti alanlarini tek tipli bir sozlesmede toplar.
// sistemdeki gorevi: Modul ile ayar kaynagi arasindaki tek sinirdir. Degerler modulun kendi
// PreConfigureServices'inde appsettings'ten baglanir; kendi kaynagindan beslemek isteyen host
// ayni options'i kendi PreConfigureServices'inde ezer ve appsettings'e hic dokunmaz.
public class FileStorageOptions
{
    // MinIO sunucusunun adresi, ornek "localhost:9000". Sema icermez.
    public string? EndPoint { get; set; }

    // MinIO erisim anahtari.
    public string? AccessKey { get; set; }

    // MinIO gizli anahtari.
    public string? SecretKey { get; set; }

    // Modulun byte'larini yazdigi bucket adi; her ortam kendi bucket'ini secer.
    public string? BucketName { get; set; }

    // Baglantinin TLS uzerinden kurulup kurulmayacagi.
    public bool WithSsl { get; set; }

    // Bucket yoksa modulun onu kendisi olusturup olusturmayacagi.
    public bool CreateBucketIfNotExists { get; set; } = true;
}
