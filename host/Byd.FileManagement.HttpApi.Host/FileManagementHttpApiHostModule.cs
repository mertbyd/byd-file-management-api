using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using Byd.CoreExtensions;
using Byd.CoreExtensions.Extensions;
using Byd.CoreExtensions.Options;
using Byd.FileManagement.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Authorization;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;

namespace Byd.FileManagement;

[DependsOn(
    typeof(FileManagementApplicationModule),
    typeof(FileManagementEntityFrameworkCoreModule),
    typeof(FileManagementHttpApiModule),
    typeof(AbpAspNetCoreMvcModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule))]
// islevi: File HTTP hostunun ABP modul kompozisyonunu kurar ve tuketilen core modulunu bu host'a baglar.
// sistemdeki gorevi: File modulunu ayaga kaldiran calistirilabilir host; core semasini cozer, core modul ayarini beyan eder ve core pipeline'ini takar.
public class FileManagementHttpApiHostModule : AbpModule
{
    // Core tablolarinin sema ve on ekini acilisin en basinda cozer.
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        ServiceConfigurationContextExtension.ResolveSchemaAndPrefix();
    }

    // Core modul adi ve ortam adini beyan eder; core saglayicilari bu beyan olmadan acilista durur.
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var environmentName = context.Services.GetHostingEnvironment().EnvironmentName;
        Configure<CoreExtensionsModuleOptions>(options =>
        {
            options.ModuleName = Assembly.GetEntryAssembly()?.GetName().Name?
                .Replace(".HttpApi.Host", string.Empty);
            options.EnvironmentName = environmentName;
        });

        ConfigureAuthorization(context);

        context.Services.AddAbpSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "FileManagement API", Version = "v1" });
            options.CustomSchemaIds(type => type.FullName);
        });
    }
    //todo auth gelince kapatılacak 
    // Bu host'un onunde henuz bir kimlik dogrulama sunucusu yok. Development'ta yetki kontrolu
    // her zaman gecer ki uclar lokalde denenebilsin; diger ortamlarda hicbir sey gevsetilmez ve
    // gercek bir saglayici baglanana kadar korumali uclar kapali kalir.
    private static void ConfigureAuthorization(ServiceConfigurationContext context)
    {
        if (context.Services.GetHostingEnvironment().IsDevelopment())
        {
            context.Services.AddAlwaysAllowAuthorization();
        }
    }

    // HTTP pipeline'ini kurar; core mukerrer istek korumasi rate limiter'dan sonra, endpointlerden once takilir.
    public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        if (context.GetEnvironment().IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseCorrelationId();
        app.UseRouting();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "FileManagement API");
        });
        app.UseRateLimiter();
        // Uclardaki [Authorize] metadatasi bu middleware olmadan calismaz; yoksa istek 500 doner.
        app.UseAuthorization();
        app.UseCoreExtensions();//core module
        app.UseConfiguredEndpoints();
        return Task.CompletedTask;
    }
}
