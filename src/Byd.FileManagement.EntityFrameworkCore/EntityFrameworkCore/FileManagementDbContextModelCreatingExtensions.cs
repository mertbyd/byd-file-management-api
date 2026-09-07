using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace Byd.FileManagement.EntityFrameworkCore;

public static class FileManagementDbContextModelCreatingExtensions
{
    public static void ConfigureFileManagement(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        // Bu assembly'deki tum IEntityTypeConfiguration siniflarini tarayip uygular;
        // yeni bir konfigurasyon eklendiginde burada elle kayit gerekmez.
        builder.ApplyConfigurationsFromAssembly(
            typeof(FileManagementDbContextModelCreatingExtensions).Assembly);
    }
}
