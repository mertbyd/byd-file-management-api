using Byd.FileManagement.Options;

namespace Byd.FileManagement.Consts;

/// <summary>
/// Depolama ayar grubunun alt ayar adlarini tanimlar.
/// </summary>
// islevi: Grubun altindaki her alanin adini derleme guvenli sabitlerde tutar.
// sistemdeki gorevi: Alan adi hem ayar adinin son parcasi hem de eksik alan hatasinin verisidir;
// ikisi de buradan gelir, boylece ayar adi ile options alani ayrisamaz.
public static class FileStorageConsts
{
    public const string EndPointField = nameof(FileStorageOptions.EndPoint);
    public const string AccessKeyField = nameof(FileStorageOptions.AccessKey);
    public const string SecretKeyField = nameof(FileStorageOptions.SecretKey);
    public const string BucketNameField = nameof(FileStorageOptions.BucketName);
    public const string WithSslField = nameof(FileStorageOptions.WithSsl);
    public const string CreateBucketIfNotExistsField = nameof(FileStorageOptions.CreateBucketIfNotExists);
    public const string PresignedGetExpiryField = nameof(FileStorageOptions.PresignedGetExpiry);
    public const string PresignedPutExpiryField = nameof(FileStorageOptions.PresignedPutExpiry);
}
