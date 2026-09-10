using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using Byd.FileManagement.BlobStoring;
using Byd.FileManagement.Consts;
using Byd.FileManagement.ExceptionCodes;
using Byd.FileManagement.Settings;
using Shouldly;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.Minio;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Settings;
using Volo.Abp.Timing;
using Xunit;

namespace Byd.FileManagement.Application.StoredFiles;

/// <summary>
/// MinIO presigned URL saglayicisinin ayar ve imzalama sozlesmesini dogrular.
/// </summary>
// islevi: Nesne anahtarinin kapsam onekini, GET/PUT URL adresini, mutlak bitis zamanini ve gecersiz
// sure reddini ag cagirmadan kanitlar.
// sistemdeki gorevi: MinIO SDK sinirini ve ortak setting okuyucusunu izole eden provider testidir.
public class StoredFilePresignedUrlProviderTests
{
    private static readonly DateTime Now = new(2026, 9, 9, 12, 0, 0, DateTimeKind.Utc);

    // GET baglantisinin bucket, nesne ve ayarli sureyi tasidigini dogrular.
    [Fact]
    public async Task Should_create_presigned_get_url()
    {
        var provider = CreateProvider("01:00:00", "02:00:00");

        var result = await provider.CreateGetAsync("blob-123");

        result.Url.ShouldContain("/presigned-tests/host/blob-123");
        result.Url.ShouldContain("X-Amz-Expires=3600");
        result.ExpiresAt.ShouldBe(Now.AddHours(1));
    }

    // PUT baglantisinin bucket, nesne ve ayarli sureyi tasidigini dogrular.
    [Fact]
    public async Task Should_create_presigned_put_url()
    {
        var provider = CreateProvider("01:00:00", "02:00:00");

        var result = await provider.CreatePutAsync("blob-456");

        result.Url.ShouldContain("/presigned-tests/host/blob-456");
        result.Url.ShouldContain("X-Amz-Expires=7200");
        result.ExpiresAt.ShouldBe(Now.AddHours(2));
    }

    // Tenant kapsaminda nesne anahtarinin tenants/{id}/ onekini tasidigini dogrular.
    // Bu kural cercevenin IMinioBlobNameCalculator'undan gelir; ham blob adiyla imzalanan URL
    // var olmayan bir nesneyi gosterirdi.
    [Fact]
    public async Task Should_scope_object_name_to_current_tenant()
    {
        var tenantId = Guid.NewGuid();
        var provider = CreateProvider("01:00:00", "02:00:00", tenantId);

        var result = await provider.CreateGetAsync("blob-123");

        result.Url.ShouldContain($"/presigned-tests/tenants/{tenantId:D}/blob-123");
    }

    // Response bitis zamaninin MinIO imzasindaki tam saniye suresiyle ayni oldugunu dogrular.
    [Fact]
    public async Task Should_align_expiry_time_with_signed_seconds()
    {
        var provider = CreateProvider("00:00:01.500", "01:00:00");

        var result = await provider.CreateGetAsync("blob-123");

        result.Url.ShouldContain("X-Amz-Expires=1");
        result.ExpiresAt.ShouldBe(Now.AddSeconds(1));
    }

    // Parse edilemeyen, suresi dolmus veya yedi gunu asan GET suresini reddeder.
    [Theory]
    [InlineData("gecersiz")]
    [InlineData("00:00:00")]
    [InlineData("8.00:00:00")]
    public async Task Should_reject_invalid_presigned_get_expiry(string expiry)
    {
        var provider = CreateProvider(expiry, "01:00:00");

        var exception = await Should.ThrowAsync<BusinessException>(async () =>
            await provider.CreateGetAsync("blob-123"));

        exception.Code.ShouldBe(FileStorageExceptionCodes.NotConfigured);
        exception.Data[FileStorageExceptionCodes.SettingKeyData]
            .ShouldBe(FileStorageConsts.PresignedGetExpiryField);
    }

    // Teste ozel setting kaynagi ve sabit saatle provider kurar.
    private static StoredFilePresignedUrlProvider CreateProvider(
        string getExpiry,
        string putExpiry,
        Guid? tenantId = null)
    {
        var values = new Dictionary<string, string?>
        {
            [FileManagementSettings.Minio.EndPoint] = "localhost:9000",
            [FileManagementSettings.Minio.AccessKey] = "test-access-key",
            [FileManagementSettings.Minio.SecretKey] = "test-secret-key",
            [FileManagementSettings.Minio.BucketName] = "presigned-tests",
            [FileManagementSettings.Minio.PresignedGetExpiry] = getExpiry,
            [FileManagementSettings.Minio.PresignedPutExpiry] = putExpiry
        };
        var settingProvider = Substitute.For<ISettingProvider>();
        settingProvider.GetOrNullAsync(Arg.Any<string>())
            .Returns(call => Task.FromResult(values.GetValueOrDefault(call.Arg<string>())));
        var clock = Substitute.For<IClock>();
        clock.Now.Returns(Now);

        // Anahtar hesaplayicisi taklit edilmez: cercevenin gercek kurali kosar, boylece test
        // presigned yolun IBlobContainer ile ayni nesneyi adresledigini kanitlar.
        var currentTenant = Substitute.For<ICurrentTenant>();
        currentTenant.Id.Returns(tenantId);
        var configurationProvider = Substitute.For<IBlobContainerConfigurationProvider>();
        configurationProvider.Get(Arg.Any<string>()).Returns(new BlobContainerConfiguration());

        return new StoredFilePresignedUrlProvider(
            settingProvider,
            clock,
            new DefaultMinioBlobNameCalculator(currentTenant),
            configurationProvider);
    }
}
