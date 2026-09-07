namespace Byd.FileManagement.Models.FileCategories;

/// <summary>
/// Dosya kategorisi create ve update islemlerinin ortak domain girdisini tasir.
/// </summary>
// islevi: Public DTO alanlarini entity mutasyonundan ayiran tipli domain modelini tanimlar.
// sistemdeki gorevi: Mapperly ile FileCategoryManager arasindaki ortak girdi sozlesmesidir.
public abstract class FileCategoryInputModel
{
    public string Name { get; set; } = default!;
    public int? Code { get; set; }
}
