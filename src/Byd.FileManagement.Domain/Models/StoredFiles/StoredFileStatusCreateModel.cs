namespace Byd.FileManagement.Models.StoredFiles;

/// <summary>
/// Yeni dosya durumu kaydinin domain create girdisini tanimlar.
/// </summary>
// islevi: Seed akisinin urettigi durum alanlarini Manager'in entity kurma akimina tasir.
// sistemdeki gorevi: StoredFileStatus olusturma senaryosunun transporttan bagimsiz girdi tipidir.
public sealed class StoredFileStatusCreateModel
{
    public string Name { get; set; } = default!;
    public int? Code { get; set; }
}
