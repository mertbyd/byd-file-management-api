using System;

namespace Byd.FileManagement.Dtos.StoredFiles;

/// <summary>
/// Byte tasiyan ve presigned dosya yukleme isteklerinin ortak alanlarini tanimlar.
/// </summary>
// islevi: Is kaydi, kategori ve dosya adini iki yukleme yolunun ortak sozlesmesinde toplar.
// sistemdeki gorevi: DTO ve FluentValidation ailelerinin ayni alan listesini tekrar etmesini onler.
public abstract class StoredFileUploadInputDtoBase
{
    public Guid ExternalId { get; set; }
    public Guid CategoryId { get; set; }
    public string FileName { get; set; } = default!;
}
