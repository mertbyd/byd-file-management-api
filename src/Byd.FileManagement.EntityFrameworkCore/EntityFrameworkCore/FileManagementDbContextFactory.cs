using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Byd.FileManagement.EntityFrameworkCore;

/// <summary>
/// EF Core design-time komutlari icin File DbContext'ini olusturur.
/// </summary>
// islevi: migrations add ve database update cagrilarinda host ayarlarini okuyup context'i kurar.
// sistemdeki gorevi: Saglayici ayari AbpDbContextOptions icinde yasar ve yalnizca ABP uygulamasi
// ayaga kalkarken isler; dotnet ef uygulamayi ayaga kaldirmaz. Bu factory olmadan modulu tuketen
// host, modulun context'i icin kendi migration DbContext'ini yazmak zorunda kalir.
public class FileManagementDbContextFactory : IDesignTimeDbContextFactory<FileManagementDbContext>
{
    // Design-time DbContext'i startup projesinin ayarlarindan kurar.
    public FileManagementDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        var builder = new DbContextOptionsBuilder<FileManagementDbContext>()
            .UseNpgsql(configuration.GetConnectionString(FileManagementDbProperties.ConnectionStringName));

        return new FileManagementDbContext(builder.Options);
    }

    // Ayar dosyasi calisma klasorunden okunur; dotnet ef design-time uygulamasini startup
    // projesinin klasorunde calistirir ve appsettings.json orada durur.
    private static IConfigurationRoot BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile(
                $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                optional: true,
                reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }
}
