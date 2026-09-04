using Byd.FileManagement.Consts;

namespace Byd.FileManagement;

public static class FileManagementDbProperties
{
    public static string DbTablePrefix { get; set; } = "FileManagement";

    public static string? DbSchema { get; set; } = DatabaseConsts.ModuleSchema;

    public const string ConnectionStringName = "FileManagement";
}
