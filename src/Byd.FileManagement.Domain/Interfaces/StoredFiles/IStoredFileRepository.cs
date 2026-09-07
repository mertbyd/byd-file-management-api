using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Byd.CoreExtensions.Interfaces;
using Byd.FileManagement.Entities.StoredFiles;

namespace Byd.FileManagement.Interfaces.StoredFiles;

/// <summary>
/// Saklanan dosya aggregate'i icin extended repository sozlesmesini tanimlar.
/// </summary>
// islevi: StoredFile CRUD, filtreleme, sayim ve bulk yeteneklerine is kaydi ve kategori
// kirilimindeki okuma sorgularini ekler.
// sistemdeki gorevi: Dosya metadata kullanim senaryolari ile EF Core uygulamasi arasindaki veri erisim siniridir.
// Tabanda is kaydi kimligine gore toplu okuma yok; N+1 uretmemek icin buraya eklenir.
public interface IStoredFileRepository : IExtendedRepository<StoredFile>
{
    // Verilen is kayitlarinin butun dosyalarini tek sorguda getirir.
    Task<List<StoredFile>> GetListByExternalIdsAsync(
        IEnumerable<Guid> externalIds,
        CancellationToken cancellationToken = default);

    // Bir is kaydinin verilen kategorideki dosyalarini getirir. Kategori bir belge turudur,
    // tek dosyalik bir yuva degil; bu yuzden sonuc listedir.
    Task<List<StoredFile>> GetListByCategoryAsync(
        Guid externalId,
        Guid categoryId,
        CancellationToken cancellationToken = default);
}
