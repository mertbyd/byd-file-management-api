namespace Byd.FileManagement.Options;

/// <summary>
/// Modulun blob container'ini baglarken kullandigi depolama bilgilerini tasir.
/// </summary>
// islevi: MinIO baglanti alanlarini tek tipli bir sozlesmede toplar.
// sistemdeki gorevi: Modul ile ayar kaynagi arasindaki tek sinirdir. Alanlar acilista
// MinioSettings grubunun alt ayarlarindan tek tek okunur; modul hicbir yapilandirma dosyasi
// okumaz, deger yalnizca setting management uzerinden girilir.
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
    // Varsayilan burada degil, ayar tanimindadir; iki yerde durursa ayrisir.
    public bool CreateBucketIfNotExists { get; set; }
}
