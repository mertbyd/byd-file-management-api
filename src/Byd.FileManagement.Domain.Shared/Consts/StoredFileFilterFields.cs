namespace Byd.FileManagement.Consts;

/// <summary>
/// Saklanan dosya filtre yuzeyinde disariya acilan alan adlari.
/// </summary>
// islevi: DataTable izin listesindeki tel alanlarini ve entity property yollarini derleme guvenli sabitler olarak tanimlar.
// sistemdeki gorevi: Filtre DTO'su ile repository sorgu motoru arasindaki kararli alan adi sozlesmesidir.
public static class StoredFileFilterFields
{
    public const string ModuleName = "moduleName";
    public const string ExternalId = "externalId";
    public const string CategoryId = "categoryId";
    public const string TenantId = "tenantId";
    public const string CreationTime = "creationTime";

    public static class PropertyPaths
    {
        public const string ModuleName = "ModuleName";
        public const string ExternalId = "ExternalId";
        public const string CategoryId = "CategoryId";
        public const string TenantId = "TenantId";
        public const string CreationTime = "CreationTime";
    }
}
