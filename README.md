# Byd.FileManagement

Dosya yukleme, indirme, sureli dogrudan baglanti ve silme yeteneklerini tasiyan ABP DDD modulu. Her dosya bir is
kaydina (`ExternalId`) ve bir dosya kategorisine baglanir; byte'lar MinIO'da, metadata `file_management` semasinda durur.

## Paketler

Tuketen projede her katman ayni adli katman paketini referans alir ve o katmanin modulunu `[DependsOn]` ile ekler:

| Paket | Modul |
|---|---|
| `Byd.FileManagement.Domain.Shared` | `FileManagementDomainSharedModule` |
| `Byd.FileManagement.Domain` | `FileManagementDomainModule` |
| `Byd.FileManagement.Application.Contracts` | `FileManagementApplicationContractsModule` |
| `Byd.FileManagement.Application` | `FileManagementApplicationModule` |
| `Byd.FileManagement.EntityFrameworkCore` | `FileManagementEntityFrameworkCoreModule` |
| `Byd.FileManagement.HttpApi` | `FileManagementHttpApiModule` |
| `Byd.FileManagement.HttpApi.Client` | `FileManagementHttpApiClientModule` |

Modul `Byd.CoreExtensions` paketlerine baglidir; core'un host sozlesmesi de gecerlidir.

## Host baglantisi

- Baglanti dizesi adi `FileManagement`'tir.
- MinIO ayarlari `FileManagement.MinioSettings.*` ayar adlariyla okunur: `EndPoint`, `AccessKey`, `SecretKey`,
  `BucketName`, `WithSsl`, `CreateBucketIfNotExists`, `PresignedGetExpiry`, `PresignedPutExpiry`.
- Kendi depolama saglayicisini baglayan host `FileManagementModuleOptions.IsDefaultStorageEnabled`'i kapatir.
- Pipeline'da `app.UseCoreExtensions()` cagrisindan sonra `app.UseFileManagement()` cagrilir.

## Boyut siniri

Presigned yukleme, dosyanin kategorisindeki `MaxFileSize` (byte) degerine gore kabul edilir; degeri bos kategori
20 MB alir. Siniri asan yuklemenin byte'lari silinir ve kayit acilmaz. Byte yukleme (`UploadAsync`) HTTP uzerinden istek
govdesi siniriyla sinirlidir; kategori siniri ona uygulanmaz.
