// Automatically generated by Opus v0.00
namespace Test13
{
    // Define module classes here
    class QtApplication : C.Application
    {
        [Opus.Core.ModuleTargets(Platform=Opus.Core.EPlatform.Windows)]
        class Win32ResourceFile : C.Win32Resource
        {
            public Win32ResourceFile()
            {
                this.ResourceFile.SetRelativePath(this, "resources", "QtApplication.rc");
            }
        }

        class SourceFiles : C.Cxx.ObjectFileCollection
        {
            public SourceFiles()
            {
                this.Include(this, "source", "*.cpp");

                this.UpdateOptions += new Opus.Core.UpdateOptionCollectionDelegate(SourceFiles_UpdateOptions);
            }

            void SourceFiles_UpdateOptions(Opus.Core.IModule module, Opus.Core.Target target)
            {
                if (module.Options is MingwCommon.ICCompilerOptions)
                {
                    (module.Options as MingwCommon.ICCompilerOptions).Pedantic = false;
                }
                else if (module.Options is GccCommon.ICCompilerOptions)
                {
                    (module.Options as GccCommon.ICCompilerOptions).Pedantic = false;
                }
            }

            /*
            class MyMocFile : QtCommon.MocFile
            {
                public MyMocFile()
                {
                    this.SetRelativePath(this, "source", "myobject.h");
                }
            }
             */
            class MyMocFiles : QtCommon.MocFileCollection
            {
                public MyMocFiles()
                {
                    this.Include(this, "source", "*.h");

                    Opus.Core.IModule mocFile = this.GetChildModule(this, "source", "myobject2.h");
                    if (null != mocFile)
                    {
                        mocFile.UpdateOptions += new Opus.Core.UpdateOptionCollectionDelegate(mocFile_UpdateOptions);
                    }
                }

                void mocFile_UpdateOptions(Opus.Core.IModule module, Opus.Core.Target target)
                {
                    QtCommon.IMocOptions options = module.Options as QtCommon.IMocOptions;
                    if (null != options)
                    {
                        options.Defines.Add("CUSTOM_MOC_DEFINE_FOR_MYOBJECTS2");
                    }
                }
            }

            [Opus.Core.DependentModules]
            //Opus.Core.TypeArray dependents = new Opus.Core.TypeArray(typeof(SourceFiles.MyMocFile));
            Opus.Core.TypeArray dependents = new Opus.Core.TypeArray(typeof(SourceFiles.MyMocFiles));
        }

        [Opus.Core.SourceFiles]
        SourceFiles sourceFiles = new SourceFiles();

        [Opus.Core.DependentModules]
        Opus.Core.TypeArray dependents = new Opus.Core.TypeArray(
            typeof(Qt.Core),
            typeof(Qt.Gui)
            );

        [Opus.Core.DependentModules(Platform = Opus.Core.EPlatform.Windows, ToolsetTypes = new[] { typeof(VisualC.Toolset) })]
        Opus.Core.TypeArray winVCDependents = new Opus.Core.TypeArray(typeof(WindowsSDK.WindowsSDK));

        [C.RequiredLibraries(Platform = Opus.Core.EPlatform.Windows, ToolsetTypes = new[] { typeof(VisualC.Toolset) })]
        Opus.Core.StringArray winVCLibraries = new Opus.Core.StringArray("KERNEL32.lib");

        [Opus.Core.DependentModules(Platform = Opus.Core.EPlatform.Windows)]
        Opus.Core.TypeArray resourceFiles = new Opus.Core.TypeArray(
            typeof(Win32ResourceFile)
            );
    }

#if OPUSPACKAGE_FILEUTILITIES_DEV
    class PublishDynamicLibraries : FileUtilities.CopyFileCollection
    {
        public PublishDynamicLibraries(Opus.Core.Target target)
        {
            this.Include(target,
                         C.OutputFileFlags.Executable,
                         typeof(Qt.Core),
                         typeof(Qt.Gui));
        }

        [FileUtilities.BesideModule(C.OutputFileFlags.Executable)]
        System.Type nextTo = typeof(QtApplication);
    }
#elif OPUSPACKAGE_FILEUTILITIES_1_0
    class PublishDynamicLibraries : FileUtilities.CopyFiles
    {
        public PublishDynamicLibraries(Opus.Core.Target target)
        {
            this.UpdateOptions += new Opus.Core.UpdateOptionCollectionDelegate(PublishDynamicLibraries_UpdateOptions);
        }

        void PublishDynamicLibraries_UpdateOptions(Opus.Core.IModule module, Opus.Core.Target target)
        {
            Qt.Qt thirdPartyModule =
                Opus.Core.ModuleUtilities.GetModuleNoToolchain(typeof(Qt.Qt), target) as Qt.Qt;
            if (null == thirdPartyModule)
            {
                throw new Opus.Core.Exception("Cannot locate Qt module instance");
            }

            if (target.HasPlatform(Opus.Core.EPlatform.Windows))
            {
                if (target.HasConfiguration(Opus.Core.EConfiguration.Debug))
                {
                    this.sourceFiles.AddRelativePaths(thirdPartyModule.BinPath, "QtCored4.dll");
                    this.sourceFiles.AddRelativePaths(thirdPartyModule.BinPath, "QtGuid4.dll");
                }
                else
                {
                    this.sourceFiles.AddRelativePaths(thirdPartyModule.BinPath, "QtCore4.dll");
                    this.sourceFiles.AddRelativePaths(thirdPartyModule.BinPath, "QtGui4.dll");
                }
            }
            else if (target.HasPlatform(Opus.Core.EPlatform.Unix))
            {
                this.sourceFiles.AddRelativePaths(thirdPartyModule.BinPath, "libQtCore.so");
                this.sourceFiles.AddRelativePaths(thirdPartyModule.BinPath, "libQtGui.so");
            }
        }

        [Opus.Core.SourceFiles]
        Opus.Core.FileCollection sourceFiles = new Opus.Core.FileCollection();

        [FileUtilities.DestinationModuleDirectory(C.OutputFileFlags.Executable)]
        Opus.Core.TypeArray destinationTarget = new Opus.Core.TypeArray(typeof(QtApplication));
    }
#else
#error Unknown FileUtilities package version
#endif
}
 
