namespace Byd.FileManagement.Dtos.FileCategories;

/// <summary>
/// Dosya kategorisi create ve update isteklerinin ortak alanlarini tasir.
/// </summary>
// islevi: Public CRUD girdisinin degistirilebilir kategori alanlarini tanimlar.
// sistemdeki gorevi: FluentValidation ile Mapperly arasindaki ortak request sozlesmesidir.
public abstract class FileCategoryInputDto
{
    public string Name { get; set; } = default!;
    public int? Code { get; set; }
}
