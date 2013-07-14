// Automatically generated by Opus v0.00
namespace Test10
{
    class MyStaticLibrary : C.StaticLibrary
    {
        public MyStaticLibrary()
        {
            this.sourceFile.SetRelativePath(this, "source", "stlib.c");
        }

        [Opus.Core.SourceFiles]
        C.ObjectFile sourceFile = new C.ObjectFile();
    }

    class MyDynamicLibrary : C.DynamicLibrary
    {
        public MyDynamicLibrary()
        {
            this.sourceFile.SetRelativePath(this, "source", "dylib.c");
        }

        [Opus.Core.SourceFiles]
        C.ObjectFile sourceFile = new C.ObjectFile();

        [Opus.Core.DependentModules(Platform=Opus.Core.EPlatform.Windows, ToolsetTypes=new[]{typeof(VisualC.Toolset)})]
        Opus.Core.TypeArray dependents = new Opus.Core.TypeArray(typeof(WindowsSDK.WindowsSDK));

        [C.RequiredLibraries(Platform = Opus.Core.EPlatform.Windows, ToolsetTypes = new[] { typeof(VisualC.Toolset) })]
        Opus.Core.StringArray libraries = new Opus.Core.StringArray("KERNEL32.lib");
    }

    class MyStandaloneApp : C.Application
    {
        public MyStandaloneApp()
        {
            this.sourceFile.SetRelativePath(this, "source", "standaloneapp.c");
        }

        [Opus.Core.SourceFiles]
        C.ObjectFile sourceFile = new C.ObjectFile();

        [Opus.Core.DependentModules]
        Opus.Core.TypeArray dependents = new Opus.Core.TypeArray(typeof(MyStaticLibrary));

        [Opus.Core.DependentModules(Platform = Opus.Core.EPlatform.Windows, ToolsetTypes = new[] { typeof(VisualC.Toolset) })]
        Opus.Core.TypeArray windowsDependents = new Opus.Core.TypeArray(typeof(WindowsSDK.WindowsSDK));

        [C.RequiredLibraries(Platform = Opus.Core.EPlatform.Windows, ToolsetTypes = new[] { typeof(VisualC.Toolset) })]
        Opus.Core.StringArray libraries = new Opus.Core.StringArray("KERNEL32.lib");
    }

    class DllDependentApp : C.Application
    {
        public DllDependentApp()
        {
            this.sourceFile.SetRelativePath(this, "source", "dlldependentapp.c");
        }

        [Opus.Core.SourceFiles]
        C.ObjectFile sourceFile = new C.ObjectFile();

        [Opus.Core.DependentModules]
        Opus.Core.TypeArray dependents = new Opus.Core.TypeArray(typeof(MyDynamicLibrary));

        [Opus.Core.DependentModules(Platform = Opus.Core.EPlatform.Windows, ToolsetTypes = new[] { typeof(VisualC.Toolset) })]
        Opus.Core.TypeArray windowsDependents = new Opus.Core.TypeArray(typeof(WindowsSDK.WindowsSDK));

        [C.RequiredLibraries(Platform = Opus.Core.EPlatform.Windows, ToolsetTypes = new[] { typeof(VisualC.Toolset) })]
        Opus.Core.StringArray libraries = new Opus.Core.StringArray("KERNEL32.lib");
    }

#if OPUSPACKAGE_FILEUTILITIES_DEV
    class PublishDynamicLibraries : FileUtilities.CopyFile
    {
        public PublishDynamicLibraries()
        {
            this.Set(typeof(MyDynamicLibrary), C.OutputFileFlags.Executable);
        }

        [FileUtilities.BesideModule(C.OutputFileFlags.Executable)]
        System.Type nextTo = typeof(DllDependentApp);
    }
#elif OPUSPACKAGE_FILEUTILITIES_1_0
    class PublishDynamicLibraries : FileUtilities.CopyFiles
    {
        [FileUtilities.SourceModules(C.OutputFileFlags.Executable)]
        Opus.Core.TypeArray sourceTargets = new Opus.Core.TypeArray(typeof(MyDynamicLibrary));

        [FileUtilities.DestinationModuleDirectory(C.OutputFileFlags.Executable)]
        Opus.Core.TypeArray destinationTarget = new Opus.Core.TypeArray(typeof(DllDependentApp));
    }
#else
#error Unknown FileUtilities package version
#endif
}
