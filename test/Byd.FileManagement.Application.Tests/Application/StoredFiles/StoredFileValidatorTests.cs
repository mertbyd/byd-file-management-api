using System;
using Byd.CoreExtensions.ExceptionCodes;
using Byd.FileManagement.Consts;
using Byd.FileManagement.Dtos.StoredFiles;
using Byd.FileManagement.ExceptionCodes;
using Byd.FileManagement.Validators.StoredFiles;
using Shouldly;
using Xunit;

namespace Byd.FileManagement.Application.StoredFiles;

/// <summary>
/// Yukleme girdisinin boyut sozlesmesini dogrular.
/// </summary>
// islevi: Sinirin altindaki verinin gectigini, ustundekinin kararli hata koduyla reddedildigini ve
// bos verinin boyut kuralina hic girmeden zorunluluk koduyla dustugunu kanitlar.
// sistemdeki gorevi: Buyuk govdenin depoya ve bellege ulasmadan once kesildigini koruyan testtir.
public class StoredFileValidatorTests
    : FileManagementApplicationTestBase<FileManagementApplicationTestModule>
{
    // Bos verinin zorunluluk koduyla dustugunu dogrular.
    [Fact]
    public void Should_reject_empty_data_before_size_rule()
    {
        var result = new UploadStoredFileDtoValidator().Validate(BuildUpload([]));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(error => error.ErrorCode == GeneralExceptionCodes.RequiredValueMissing);
        result.Errors.ShouldNotContain(error => error.ErrorCode == StoredFileExceptionCodes.DataTooLarge);
    }

    // Presigned yuklemenin byte kurali olmadan ortak zorunluluklari uyguladigini dogrular.
    [Fact]
    public void Should_validate_presigned_upload_common_fields()
    {
        var input = new PresignedUploadStoredFileDto
        {
            ExternalId = Guid.Empty,
            CategoryId = Guid.Empty,
            FileName = string.Empty
        };

        var result = new PresignedUploadStoredFileDtoValidator().Validate(input);

        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(3);
        result.Errors.ShouldAllBe(error => error.ErrorCode == StoredFileExceptionCodes.RequiredValueMissing);
    }

    // Presigned yuklemenin dosya adi kalicilik sinirini bounded kodla reddettigini dogrular.
    [Fact]
    public void Should_reject_too_long_presigned_upload_file_name()
    {
        var input = new PresignedUploadStoredFileDto
        {
            ExternalId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            FileName = new string('a', StoredFileConsts.MaxOriginalFileNameLength + 1)
        };

        var result = new PresignedUploadStoredFileDtoValidator().Validate(input);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldHaveSingleItem().ErrorCode.ShouldBe(StoredFileExceptionCodes.FileNameTooLong);
    }

    private static UploadStoredFileDto BuildUpload(byte[] data)
        => new()
        {
            Data = data,
            FileName = "rapor.pdf",
            ExternalId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid()
        };
}
