using Volo.Abp.Reflection;

namespace Byd.FileManagement.Permissions;

/// <summary>
/// Modulun yetki sabitlerinin kokudur.
/// </summary>
// islevi: Yetki grubunun adini tanimlar ve tum public yetki sabitlerini tek listede toplar.
// sistemdeki gorevi: Yetki string'lerinin tek kaynagi; ozellik aileleri partial dosyalarda eklenir,
// bu dosya degismeden buyur.
public partial class FileManagementPermissions
{
    public const string GroupName = "FileManagement";

    // Tum public yetki sabitlerini ABP kayit ve test altyapisina acar.
    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(FileManagementPermissions));
    }
}
