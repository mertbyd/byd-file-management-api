namespace Byd.FileManagement.Consts;

/// <summary>
/// Saklanan dosya kaydinin tablo, indeks ve alan sinirlarini tanimlar.
/// </summary>
// islevi: Dosya metadata kaydinin kalicilik sabitlerini tek yerde tutar.
// sistemdeki gorevi: Domain, EF konfigurasyonu ve okuma uclarinin ortak sabit sozlesmesidir.
public static class StoredFileConsts
{
    // Saklanan dosya metadata kayitlarinin tutuldugu tablonun EF model adidir.
    public const string TableName = "StoredFiles";

    // Is kaydi ve kategori slotunda tek dosya bulunmasini zorlayan indeks adi.
    public const string SlotIndex = "IX_StoredFiles_Slot";

    // Tenant kirilimindaki okumalari destekleyen indeks adi.
    public const string TenantIndex = "IX_StoredFiles_TenantId";

    // Kaydi yazan modulun adini tasiyan kolonun azami uzunlugu.
    public const int MaxModuleNameLength = 128;


    // Saglayicidaki blob adinin azami uzunlugu.
    public const int MaxBlobNameLength = 512;

    // Kullanicinin yukledigi ozgun dosya adinin azami uzunlugu.
    public const int MaxOriginalFileNameLength = 260;

}
