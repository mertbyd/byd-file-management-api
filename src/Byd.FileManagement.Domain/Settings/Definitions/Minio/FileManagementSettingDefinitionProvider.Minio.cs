using Byd.FileManagement.Consts;
using Volo.Abp.Settings;

namespace Byd.FileManagement.Settings;

// islevi: MinIO baglanti ayarlarini ABP ayar tanimlarina ekler.
// sistemdeki gorevi: Ana provider'in Define metodu bu partial metodu cagirir.
//
// NEDEN ALAN ALAN. Tek bir JSON govde yerine her alan ayri ayar oldugu icin yonetim ekraninda
// tek tek duzenlenir, sadece degisen alan yazilir ve yanlis JSON yazma ihtimali ortadan kalkar.
public partial class FileManagementSettingDefinitionProvider
{
    private void AddMinioSettings(ISettingDefinitionContext context)
    {
        // Baglanti alanlarinin varsayilani bostur: deger ortama gore degisir ve modulun
        // uyduracagi bir varsayilani yoktur. Deger girilmeden ilk dosya islemi, hangi alanin
        // eksik oldugunu soyleyen bir hatayla reddedilir.
        //
        // isVisibleToClients hepsinde false: grup gizli anahtar tasir, istemciye acilmaz.
        context.Add(
            new SettingDefinition(
                FileManagementSettings.Minio.EndPoint,
                defaultValue: null,
                isVisibleToClients: false),
            new SettingDefinition(
                FileManagementSettings.Minio.AccessKey,
                defaultValue: null,
                isVisibleToClients: false),
            new SettingDefinition(
                FileManagementSettings.Minio.SecretKey,
                defaultValue: null,
                isVisibleToClients: false),
            new SettingDefinition(
                FileManagementSettings.Minio.BucketName,
                defaultValue: null,
                isVisibleToClients: false));

        // Davranis alanlarinin makul varsayilani vardir; girilmezse modul calisir.
        context.Add(
            new SettingDefinition(
                FileManagementSettings.Minio.WithSsl,
                defaultValue: FileStorageConsts.WithSslDefault,
                isVisibleToClients: false),
            new SettingDefinition(
                FileManagementSettings.Minio.CreateBucketIfNotExists,
                defaultValue: FileStorageConsts.CreateBucketIfNotExistsDefault,
                isVisibleToClients: false),
            new SettingDefinition(
                FileManagementSettings.Minio.PresignedGetExpiry,
                defaultValue: FileStorageConsts.PresignedGetExpiryDefault,
                isVisibleToClients: false),
            new SettingDefinition(
                FileManagementSettings.Minio.PresignedPutExpiry,
                defaultValue: FileStorageConsts.PresignedPutExpiryDefault,
                isVisibleToClients: false));
    }
}
