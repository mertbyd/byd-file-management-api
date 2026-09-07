using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using Byd.FileManagement.Dtos.FileCategories;
using Byd.FileManagement.Interfaces.FileCategories;
using Byd.FileManagement.Managers.FileCategories;
using Byd.FileManagement.Mappers.FileCategories;

namespace Byd.FileManagement.Services.FileCategories;

/// <summary>
/// Dosya kategorilerinin CRUD orkestrasyonunu yurutur.
/// </summary>
// islevi: Girdi dogrulamasini, Manager mutasyonunu ve repository kaydini duz akisla birlestirir.
// sistemdeki gorevi: Controller ile FileCategory Manager/Repository zinciri arasindaki tek public giris noktasidir.
public class FileCategoryAppService : FileManagementAppService, IFileCategoryAppService
{
    private static readonly FileCategoryMapper Mapper = new();

    private FileCategoryManager Manager =>
        LazyGetRequiredService<FileCategoryManager>();

    private IFileCategoryRepository Repository =>
        LazyGetRequiredService<IFileCategoryRepository>();

    private IValidator<CreateFileCategoryDto> CreateValidator =>
        LazyGetRequiredService<IValidator<CreateFileCategoryDto>>();

    private IValidator<UpdateFileCategoryDto> UpdateValidator =>
        LazyGetRequiredService<IValidator<UpdateFileCategoryDto>>();

    // Kimligi verilen kategoriyi Manager guard'i uzerinden getirir.
    public virtual async Task<FileCategoryDto> GetAsync(Guid id)
    {
        var entity = await Manager.EnsureExistsAsync(id, CancellationTokenProvider.Token);
        return Mapper.Map(entity);
    }

    // Aktiflik filtresine uyan kategorileri repository'den getirir.
    public virtual async Task<List<FileCategoryDto>> GetListAsync()
    {
        var entities = await Repository.GetListAsync(
            includeDetails: false,
            cancellationToken: CancellationTokenProvider.Token);
        return Mapper.Map(entities);
    }

    // Create girdisini dogrular, Manager ile entity kurar ve tek UOW icinde kaydeder.
    public virtual async Task<FileCategoryDto> CreateAsync(CreateFileCategoryDto input)
    {
        var cancellationToken = CancellationTokenProvider.Token;
        await CreateValidator.ValidateAndThrowAsync(input, cancellationToken);
        var model = Mapper.Map(input);
        var entity = await Manager.CreateAsync(model, cancellationToken);
        await Repository.InsertAsync(entity, autoSave: true, cancellationToken: cancellationToken);
        return Mapper.Map(entity);
    }

    // Update girdisini dogrular, mevcut entity'yi Manager ile degistirir ve kaydeder.
    public virtual async Task<FileCategoryDto> UpdateAsync(
        Guid id,
        UpdateFileCategoryDto input)
    {
        var cancellationToken = CancellationTokenProvider.Token;
        await UpdateValidator.ValidateAndThrowAsync(input, cancellationToken);
        var entity = await Manager.EnsureExistsAsync(id, cancellationToken);
        var model = Mapper.Map(input);
        await Manager.UpdateAsync(entity, model, cancellationToken);
        await Repository.UpdateAsync(entity, autoSave: true, cancellationToken: cancellationToken);
        return Mapper.Map(entity);
    }

    // Kimligi verilen kategoriyi varlik guard'i sonrasinda kalici olarak siler.
    public virtual async Task DeleteAsync(Guid id)
    {
        var cancellationToken = CancellationTokenProvider.Token;
        var entity = await Manager.EnsureExistsAsync(id, cancellationToken);
        await Repository.DeleteAsync(entity, autoSave: true, cancellationToken: cancellationToken);
    }
}
