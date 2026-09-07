using System;

namespace Byd.FileManagement.Consts;

/// <summary>
/// Saklanan dosya durumu lookup'inin kalicilik ve baslangic veri sabitlerini tanimlar.
/// </summary>
// islevi: Durum tablosunun adini, alan sinirini ve iki sistem durumunun sabit kimliklerini tutar.
// sistemdeki gorevi: Yukleme akisi ile okuma filtresi ayni durum kimliklerini buradan okur.
public static class StoredFileStatusConsts
{
    // Dosya durumlarinin tutuldugu tablonun EF model adidir.
    public const string TableName = "StoredFileStatuses";

    // Durum adinin izin verilen azami uzunlugudur.
    public const int MaxNameLength = 64;


    // Satir yazildi, byte henuz depoya gitmedi.
    public const string PendingSeedName = "Beklemede";
    public const int PendingCode = 1;

    // Byte depoya yazildi, kayit okuma uclarinda gorunur.
    public const string ApprovedSeedName = "Onaylı";
    public const int ApprovedCode = 2;

    // Durum kimlikleri yukleme akisi ve okuma filtresi tarafindan sabit olarak kullanilir.
    public static readonly Guid PendingId = Guid.Parse("3c7d4e20-0000-4000-8000-000000000001");
    public static readonly Guid ApprovedId = Guid.Parse("3c7d4e20-0000-4000-8000-000000000002");
}
