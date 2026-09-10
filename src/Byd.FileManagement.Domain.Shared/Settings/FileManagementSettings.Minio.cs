using Byd.FileManagement.Consts;

namespace Byd.FileManagement.Settings;

/// <summary>
/// MinIO baglanti ayarlarinin adlarini tanimlar.
/// </summary>
// islevi: MinioSettings grubunun ve altindaki her baglanti alaninin string sabitini tanimlar.
// sistemdeki gorevi: Tanim saglayicisi ve acilis okuyucusu ayar adini buradan alir; inline string
// kullanilmaz. Alan adlari FileStorageConsts'tan gelir, boylece ayar adi ile options alani ayrisamaz.
public partial class FileManagementSettings
{
    public static class Minio
    {
        public const string Default = GroupName + ".MinioSettings";

        // MinIO sunucusunun adresi. Ayni zamanda grubun varlik isaretidir: bos ise grup hic
        // doldurulmamis sayilir.
        public const string EndPoint = Default + "." + FileStorageConsts.EndPointField;

        // MinIO erisim anahtari.
        public const string AccessKey = Default + "." + FileStorageConsts.AccessKeyField;

        // MinIO gizli anahtari.
        public const string SecretKey = Default + "." + FileStorageConsts.SecretKeyField;

        // Modulun byte'larini yazdigi bucket adi.
        public const string BucketName = Default + "." + FileStorageConsts.BucketNameField;

        // Baglantinin TLS uzerinden kurulup kurulmayacagi.
        public const string WithSsl = Default + "." + FileStorageConsts.WithSslField;

        // Bucket yoksa modulun onu kendisi olusturup olusturmayacagi.
        public const string CreateBucketIfNotExists = Default + "." + FileStorageConsts.CreateBucketIfNotExistsField;

        // Presigned indirme baglantisinin gecerlilik suresi.
        public const string PresignedGetExpiry = Default + "." + FileStorageConsts.PresignedGetExpiryField;

        // Presigned yukleme baglantisinin gecerlilik suresi.
        public const string PresignedPutExpiry = Default + "." + FileStorageConsts.PresignedPutExpiryField;
    }
}
