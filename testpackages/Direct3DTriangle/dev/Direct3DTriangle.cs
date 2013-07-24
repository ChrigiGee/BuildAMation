// Automatically generated by Opus v0.00
namespace Direct3DTriangle
{
    // Define module classes here
    [Opus.Core.ModuleTargets(Platform = Opus.Core.EPlatform.Windows, ToolsetTypes = new[] { typeof(VisualC.Toolset) })]
    class D3D9TriangleTest : C.WindowsApplication
    {
        public D3D9TriangleTest()
        {
            var sourceDir = this.Locations["PackageDir"].ChildDirectory("source");
            this.headerFiles.Include(sourceDir, "*.h");
        }

        class SourceFiles : C.Cxx.ObjectFileCollection
        {
            public SourceFiles()
            {
                var sourceDir = this.Locations["PackageDir"].ChildDirectory("source");
                this.Include(sourceDir, "*.cpp");

                this.UpdateOptions += new Opus.Core.UpdateOptionCollectionDelegate(SourceFiles_VCDefines);
                this.UpdateOptions += new Opus.Core.UpdateOptionCollectionDelegate(SourceFiles_EnableException);
            }

            void SourceFiles_EnableException(Opus.Core.IModule module, Opus.Core.Target target)
            {
                var compilerOptions = module.Options as C.ICxxCompilerOptions;
                compilerOptions.ExceptionHandler = C.Cxx.EExceptionHandler.Asynchronous;
            }

            void SourceFiles_VCDefines(Opus.Core.IModule module, Opus.Core.Target target)
            {
                if (module.Options is VisualCCommon.ICCompilerOptions)
                {
                    var compilerOptions = module.Options as C.ICCompilerOptions;
                    compilerOptions.Defines.Add("_CRT_SECURE_NO_WARNINGS");
                }
            }
        }

        [Opus.Core.SourceFiles]
        SourceFiles sourceFiles = new SourceFiles();

        [C.HeaderFiles]
        Opus.Core.FileCollection headerFiles = new Opus.Core.FileCollection();

        [Opus.Core.DependentModules]
        Opus.Core.TypeArray dependents = new Opus.Core.TypeArray(
            typeof(DirectXSDK.Direct3D9),
            typeof(WindowsSDK.WindowsSDK)
        );

        [C.RequiredLibraries(Platform = Opus.Core.EPlatform.Windows, ToolsetTypes = new[] { typeof(VisualC.Toolset) })]
        Opus.Core.StringArray winVCLibraries = new Opus.Core.StringArray(
            "KERNEL32.lib",
            "USER32.lib",
            "DxErr.lib"
        );
    }
}
