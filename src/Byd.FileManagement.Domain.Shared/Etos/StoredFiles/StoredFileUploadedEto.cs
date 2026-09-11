namespace Byd.FileManagement.Etos.StoredFiles;

/// <summary>
/// Depolamaya bir nesne yazildigini bildiren yerel olay.
/// </summary>
// islevi: Yazilan nesnenin blob adini ve saglayicinin bildirdigi byte boyutunu tasir.
// sistemdeki gorevi: MinIO bildirim consumer'i ile presigned yuklemeyi tamamlayan handler arasindaki
// surec ici sozlesmedir. Dagitik degildir; RabbitMQ'ya geri yayinlanmaz.
public sealed record StoredFileUploadedEto(string BlobName, long Size);
