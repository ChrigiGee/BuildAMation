// Automatically generated by Opus v0.00
namespace Test5
{
    // Define module classes here
    class MyDynamicLibTestApp : C.Application
    {
        class SourceFiles : C.ObjectFileCollection
        {
            public SourceFiles()
            {
                this.Include(this, "source", "dynamicmain.c");
            }
        }

        [Opus.Core.SourceFiles]
        SourceFiles sourceFiles = new SourceFiles();

        [Opus.Core.DependentModules]
        Opus.Core.TypeArray dependents = new Opus.Core.TypeArray(
            typeof(Test4.MyDynamicLib),
            typeof(Test4.MyStaticLib)
        );

        [Opus.Core.DependentModules(Platform = Opus.Core.EPlatform.Windows, ToolsetTypes = new[] { typeof(VisualC.Toolset) })]
        Opus.Core.TypeArray winVCDependents = new Opus.Core.TypeArray(typeof(WindowsSDK.WindowsSDK));

        [C.RequiredLibraries(Platform = Opus.Core.EPlatform.Windows, ToolsetTypes = new[] { typeof(VisualC.Toolset) })]
        Opus.Core.StringArray libraries = new Opus.Core.StringArray("KERNEL32.lib");
    }

#if OPUSPACKAGE_FILEUTILITIES_DEV
    class PublishDynamicLibraries : FileUtilities.CopyFile
    {
        public PublishDynamicLibraries()
        {
            this.Set(typeof(Test4.MyDynamicLib), C.OutputFileFlags.Executable);
        }

        [FileUtilities.BesideModule(C.OutputFileFlags.Executable)]
        System.Type nextTo = typeof(MyDynamicLibTestApp);
    }

    [Opus.Core.ModuleTargets(Platform=Opus.Core.EPlatform.Windows, ToolsetTypes=new[]{typeof(VisualC.Toolset)})]
    class PublishPDBs : FileUtilities.CopyFileCollection
    {
        public PublishPDBs(Opus.Core.Target target)
        {
            this.Include(target,
                         C.OutputFileFlags.LinkerProgramDatabase,
                         typeof(Test4.MyDynamicLib),
                         typeof(MyDynamicLibTestApp));
            this.UpdateOptions += delegate(Opus.Core.IModule module, Opus.Core.Target delegateTarget) {
                FileUtilities.ICopyFileOptions options = module.Options as FileUtilities.ICopyFileOptions;
                if (null != options)
                {
                    options.DestinationDirectory = @"c:\PDBs";
                }
            };
        }
    }
#elif OPUSPACKAGE_FILEUTILITIES_1_0
    class PublishDynamicLibraries : FileUtilities.CopyFiles
    {
        [FileUtilities.SourceModules(C.OutputFileFlags.Executable)]
        Opus.Core.TypeArray sourceTargets = new Opus.Core.TypeArray(typeof(Test4.MyDynamicLib));

        [FileUtilities.DestinationModuleDirectory(C.OutputFileFlags.Executable)]
        Opus.Core.TypeArray destinationTarget = new Opus.Core.TypeArray(typeof(MyDynamicLibTestApp));
    }

    [Opus.Core.ModuleTargets(Platform=Opus.Core.EPlatform.Windows)]
    class PublishPDBs : FileUtilities.CopyFiles
    {
        public PublishPDBs()
        {
            this.destinationDirectory.AddAbsoluteDirectory(@"c:\PDBs", false);
        }

        [FileUtilities.SourceModules(C.OutputFileFlags.LinkerProgramDatabase)]
        Opus.Core.TypeArray sourceTargets = new Opus.Core.TypeArray(
            typeof(Test4.MyDynamicLib),
            typeof(MyDynamicLibTestApp)
        );

        [FileUtilities.DestinationDirectoryPath]
        Opus.Core.DirectoryCollection destinationDirectory = new Opus.Core.DirectoryCollection();
    }
#else
#error Unrecognized FileUtilities package version
#endif
}
