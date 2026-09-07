using System.Collections.Generic;
using Byd.FileManagement.Dtos.FileCategories;
using Byd.FileManagement.Entities.FileCategories;
using Byd.FileManagement.Models.FileCategories;
using Riok.Mapperly.Abstractions;

namespace Byd.FileManagement.Mappers.FileCategories;

/// <summary>
/// FileCategory DTO, domain model ve entity eslemelerini tanimlar.
/// </summary>
// islevi: CRUD girdileri ile ciktilari Mapperly tarafindan uretilen kodla donusturur.
// sistemdeki gorevi: FileCategory katmanlari arasindaki alan tasimanin tek sahibidir.
[Mapper]
public partial class FileCategoryMapper
{
    public partial FileCategoryCreateModel Map(CreateFileCategoryDto source);
    public partial FileCategoryUpdateModel Map(UpdateFileCategoryDto source);
    public partial FileCategoryDto Map(FileCategory source);
    public partial List<FileCategoryDto> Map(List<FileCategory> source);
}
