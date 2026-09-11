using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Byd.FileManagement.BlobStoring;

/// <summary>
/// MinIO'nun AMQP hedefine yayinladigi S3 "nesne yazildi" bildiriminin modulun okudugu kismi.
/// </summary>
// islevi: Bildirim govdesini tipli kayitlara cevirir ve her kaydin blob adini ve boyutunu verir.
// sistemdeki gorevi: MinIO'nun tel formatini tek dosyada tutar; alan adlari JSON anahtari olarak kodda
// dolasmaz, kayit sekli bildirim seklini birebir izler (Records[].s3.object.key/size).
public sealed record MinioObjectCreatedNotification(IReadOnlyList<MinioObjectCreatedNotification.Entry> Records)
{
    // Bildirimdeki yazilan nesneler; MinIO her kayitta s3 dugumunu doldurur.
    public IEnumerable<StoredObject> Objects => Records.Select(entry => entry.S3.Object);

    // Govdeyi cozer. Bozuk ya da eksik alanli govde yeniden denense de duzelmez; false doner ki
    // mesaj kuyrukta sonsuz donmesin.
    public static bool TryParse(
        ReadOnlySpan<byte> body,
        [NotNullWhen(true)] out MinioObjectCreatedNotification? notification)
    {
        try
        {
            notification = JsonSerializer.Deserialize(
                body,
                MinioNotificationJsonContext.Default.MinioObjectCreatedNotification);
        }
        catch (JsonException)
        {
            notification = null;
        }
        return notification is not null;
    }
    public sealed record Entry(S3Section S3);
    public sealed record S3Section(StoredObject Object);
    public sealed record StoredObject(string Key, long Size)
    {
        // Anahtar URL kodlu gelir ve cercevenin kapsam onekini tasir (host/{blob} veya
        // tenants/{id}/{blob}); blob adi son parcadir.
        public string BlobName => Path.GetFileName(Uri.UnescapeDataString(Key));
    }
}
// Kaynak uretimli serilestirici: yansima kullanmaz. Eksik ya da null zorunlu alan JsonException
// uretir, boylece yarim kayit handler'a hic ulasmaz.
[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    RespectNullableAnnotations = true,
    RespectRequiredConstructorParameters = true)]
[JsonSerializable(typeof(MinioObjectCreatedNotification))]
internal sealed partial class MinioNotificationJsonContext : JsonSerializerContext;
