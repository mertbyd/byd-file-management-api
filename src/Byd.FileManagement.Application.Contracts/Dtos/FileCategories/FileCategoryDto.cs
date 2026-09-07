using System;

namespace Byd.FileManagement.Dtos.FileCategories;

/// <summary>
/// Kalici dosya kategorisi kaydinin public cevap alanlarini tasir.
/// </summary>
// islevi: Kategori kimligini, lookup alanlarini, aktifligi ve olusturma zamanini HTTP ciktisina acar.
// sistemdeki gorevi: Entity veya EF tiplerini disariya sizdirmayan response sozlesmesidir.
public class FileCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public int? Code { get; set; }
    public DateTime CreationTime { get; set; }
}
