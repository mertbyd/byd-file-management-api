using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Byd.FileManagement.Data.StoredFiles;
using Volo.Abp.Data;
using Volo.Abp.Uow;

namespace Byd.FileManagement;

/// <summary>
/// Tuketen host'un modulun acilis isini tetikledigi uzanti.
/// </summary>
// islevi: Dosya durumu lookup'ini sabit kimlikleriyle yazar.
// sistemdeki gorevi: Modulun tuketen host'a actigi tek acilis kancasi. Bu cagri yapilmazsa durum
// satirlari hic yazilmaz ve ilk yukleme var olmayan bir duruma baglanmaya calisir.
// Kategori burada yazilmaz: kategori listesinin sahibi tuketicidir, kendi kategorilerini kendisi acar.
public static class FileManagementApplicationBuilderExtensions
{
    // Modulun acilis isini calistirir.
    public static IApplicationBuilder UseFileManagement(this IApplicationBuilder app)
    {
        // Pipeline kurulumu senkron oldugu icin acilis isi burada beklenir. Istek yolunda degil,
        // process basina bir kez calisan acilis adimi oldugu icin bloklama kabul edilir.
        SeedLookupsAsync(app.ApplicationServices).GetAwaiter().GetResult();
        return app;
    }
    // Durum lookup'ini tek unit of work icinde yazar; yazim idempotenttir, mevcut satirlara dokunmaz.
    private static async Task SeedLookupsAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var unitOfWorkManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
        using var unitOfWork = unitOfWorkManager.Begin();
        await scope.ServiceProvider.GetRequiredService<StoredFileStatusDataSeeder>()
            .SeedAsync(new DataSeedContext());
        await unitOfWork.CompleteAsync();
    }
}
