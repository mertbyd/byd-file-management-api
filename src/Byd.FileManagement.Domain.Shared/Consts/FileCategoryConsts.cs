namespace Byd.FileManagement.Consts;

/// <summary>
/// Dosya kategorisi kalicilik ve girdi sabitlerini tanimlar.
/// </summary>
// islevi: Tablo adini, ad indeksini ve alan sinirini tek yerde tutar.
// sistemdeki gorevi: Domain, validation ve EF konfigurasyonu icin kararli sabit sahibidir.
// Kategori listesinin sahibi tuketicidir; modul hicbir kategori adi ya da kimligi beyan etmez.
public static class FileCategoryConsts
{
    // Dosya kategorilerinin tutuldugu tablonun EF model adidir.
    public const string TableName = "FileCategories";

    // Kategori adinin benzersizligini zorlayan indeksin adi.
    public const string NameIndex = "IX_FileCategories_Name";

    // Kategori adinin izin verilen azami uzunlugudur.
    public const int MaxNameLength = 128;
}
