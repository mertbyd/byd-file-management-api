using System;
using System.Collections.Generic;
using Byd.FileManagement.Dtos.StoredFiles;
using Byd.FileManagement.Entities.StoredFiles;
using Byd.FileManagement.Models.StoredFiles;
using Riok.Mapperly.Abstractions;

namespace Byd.FileManagement.Mappers.StoredFiles;

/// <summary>
/// Saklanan dosya entity ve DTO eslemelerini tanimlar.
/// </summary>
// islevi: Tekil ve liste repository ciktilarini Mapperly tarafindan uretilen kodla donusturur.
// sistemdeki gorevi: Dosya metadata okuma katmanlari arasindaki alan tasimanin tek sahibidir.
[Mapper]
public partial class StoredFileMapper
{
    // Tek saklanan dosya entity'sini public cevap sozlesmesine esler.
    public partial StoredFileDto Map(StoredFile source);

    // Saklanan dosya entity listesini public cevap listesine esler.
    public partial List<StoredFileDto> Map(List<StoredFile> source);

    // Saklanan dosya metadata alanlariyla depodan okunan byte'lari tek cevapta esler.
    public partial StoredFileWithContentDto MapWithContent(StoredFile source, byte[] data);

    // Yukleme istegini Manager'in bekledigi domain create modeline esler.
    public partial StoredFileCreateModel Map(UploadStoredFileDto source);

    // Presigned yukleme istegini ayni domain create modeline esler.
    public partial StoredFileCreateModel Map(PresignedUploadStoredFileDto source);

    // Manager'in kurdugu, henuz kaydedilmemis entity'yi onbellekte bekleyecek modele esler.
    public partial StoredFilePendingUploadModel MapToPendingUpload(StoredFile source);

    // Kayit kimligini provider'dan gelen imzali URL ve bitis zamaniyla tek cevapta esler.
    // Yukaridaki MapWithContent ile ayni sekil: entity alanlari calisma zamani degeriyle tek
    // Mapperly metodunda birlesir, AppService'te elle nesne kurulmaz.
    public partial PresignedUrlDto MapToPresignedUrl(StoredFile source, string url, DateTime expiresAt);

    // Kayit alanlarini depodan okunan byte'larla birlestirip indirme sozlesmesine esler.
    // ContentType kayitta yoktur; ayni kaynak alandan, asagidaki cozumleyiciyle uretilir.
    [MapProperty(
        nameof(StoredFile.OriginalFileName),
        nameof(DownloadStoredFileDto.ContentType),
        Use = nameof(ResolveContentType))]
    public partial DownloadStoredFileDto Map(StoredFile source, byte[] data);

    // Dosya adindan MIME turunu uretir. Cozulemeyen uzanti yukleme aninda reddedildigi icin
    // burada ikinci bir dogrulama yapilmaz.
    //
    // Default = false zorunlu: isaretlenmezse Mapperly bu metodu genel bir string->string donusumu
    // sayar ve mapper'daki butun metin alanlarina uygular; o zaman FileName de MIME turune donusur.
    // Bu isaretle metot yalnizca yukaridaki MapProperty'nin acikca cagirdigi yerde kullanilir.
    [UserMapping(Default = false)]
    private static string ResolveContentType(string originalFileName)
        => MimeTypes.GetMimeType(originalFileName);
}
