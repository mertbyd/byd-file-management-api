using Volo.Abp.Modularity;

namespace Byd.FileManagement;

/* Inherit from this class for your domain layer tests.
 * See SampleManager_Tests for example.
 */
public abstract class FileManagementDomainTestBase<TStartupModule> : FileManagementTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
