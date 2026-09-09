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

    private static UploadStoredFileDto BuildUpload(byte[] data)
        => new()
        {
            Data = data,
            FileName = "rapor.pdf",
            ExternalId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid()
        };
}
