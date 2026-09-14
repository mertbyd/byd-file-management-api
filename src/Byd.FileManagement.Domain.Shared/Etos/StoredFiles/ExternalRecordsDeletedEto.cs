using System;
using System.Collections.Generic;
using Byd.FileManagement.Consts;
using Volo.Abp.EventBus;

namespace Byd.FileManagement.Etos.StoredFiles;

/// <summary>
/// Silinen is kayitlarinin dis kimliklerini tasiyan dagitik olay.
/// </summary>
// islevi: Hangi tenant'in hangi is kayitlarinin silindigini tasir.
// sistemdeki gorevi: Yayinlayan modul ile dosya modulu arasindaki tel sozlesmesidir; sinif Domain.Shared'da
// durdugu icin yayinlayan modul kopya tutmaz, ayni tipi referans eder.
[EventName(StoredFileEventNames.ExternalRecordsDeleted)]
public class ExternalRecordsDeletedEto
{
    // Silinen kayitlarin tenant'i; host kayitlari icin bos.
    public Guid? TenantId { get; set; }

    // Dosyalari silinecek is kayitlarinin dis kimlikleri.
    public List<Guid> ExternalIds { get; set; } = [];
}
