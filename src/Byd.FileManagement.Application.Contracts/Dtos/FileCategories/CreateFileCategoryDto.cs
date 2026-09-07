namespace Byd.FileManagement.Dtos.FileCategories;

/// <summary>
/// Yeni dosya kategorisi olusturma istegini tanimlar.
/// </summary>
// islevi: FileCategory create endpointinin public girdi tipidir.
// sistemdeki gorevi: Kimlik ve audit alanlarini sunucuda birakip degistirilebilir alanlari kabul eder.
public sealed class CreateFileCategoryDto : FileCategoryInputDto
{
}
