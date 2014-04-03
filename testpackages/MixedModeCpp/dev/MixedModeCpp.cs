// Automatically generated by Opus v0.00
namespace MixedModeCpp
{
    // Define module classes here
    [Opus.Core.ModuleTargets(Platform = Opus.Core.EPlatform.Windows, ToolsetTypes = new[] { typeof(VisualC.Toolset) })]
    class TestApplication : C.Application
    {
        public TestApplication()
        {
            this.UpdateOptions += new Opus.Core.UpdateOptionCollectionDelegate(TestApplication_UpdateOptions);
        }

        void TestApplication_UpdateOptions(Opus.Core.IModule module, Opus.Core.Target target)
        {
            var options = module.Options as C.ILinkerOptions;
            options.DoNotAutoIncludeStandardLibraries = false;
        }

        class SourceFiles : C.Cxx.ObjectFileCollection
        {
            public SourceFiles()
            {
                var sourceDir = this.PackageLocation.SubDirectory("source");
                this.Include(sourceDir, "native.cpp");
            }
        }

        class ManagedSourceFiles : VisualCCommon.ManagedCxxObjectFileCollection
        {
            public ManagedSourceFiles()
            {
                var sourceDir = this.PackageLocation.SubDirectory("source");
                this.Include(sourceDir, "managed.cpp");
            }
        }

        [Opus.Core.SourceFiles]
        SourceFiles nativeSourceFiles = new SourceFiles();

        [Opus.Core.SourceFiles]
        ManagedSourceFiles managedSourceFiles = new ManagedSourceFiles();

        [Opus.Core.DependentModules]
        Opus.Core.TypeArray dependentModules = new Opus.Core.TypeArray(
            typeof(WindowsSDK.WindowsSDK)
        );

        [C.RequiredLibraries]
        Opus.Core.StringArray libraries = new Opus.Core.StringArray(
            "KERNEL32.lib",
            "mscoree.lib"
        );
    }
}
