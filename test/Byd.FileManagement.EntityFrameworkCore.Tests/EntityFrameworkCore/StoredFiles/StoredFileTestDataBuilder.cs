using System;
using Byd.FileManagement.Models.StoredFiles;

namespace Byd.FileManagement.EntityFrameworkCore.StoredFiles;

/// <summary>
/// StoredFile testlerinin ortak create girdisini uretir.
/// </summary>
// islevi: Zorunlu alanlari gecerli varsayilanlarla doldurup testin ilgilendigi alanlari disariya birakir.
// sistemdeki gorevi: Her testin ayni on kosul bloklarini tekrar yazmasini onleyen test veri kaynagidir.
public static class StoredFileTestDataBuilder
{
    // Verilen is kaydi ve kategori adresiyle gecerli bir create modeli kurar.
    // Kategori kimligi disaridan gelir; modul kategori seed etmez, cagiran kendi kategorisini acar.
    public static StoredFileCreateModel Build(Guid externalId, Guid categoryId)
        => new()
        {
            ExternalId = externalId,
            CategoryId = categoryId,
            FileName = "belge.pdf"
        };
}
