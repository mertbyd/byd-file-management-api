using Volo.Abp.Modularity;

namespace Byd.FileManagement;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class FileManagementApplicationTestBase<TStartupModule> : FileManagementTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
