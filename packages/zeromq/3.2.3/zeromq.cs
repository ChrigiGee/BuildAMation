// Automatically generated by Opus v0.50
namespace zeromq
{
    class ZMQSharedLibrary : C.DynamicLibrary
    {
        public ZMQSharedLibrary()
        {
            var includeDir = this.PackageLocation.SubDirectory("zeromq-3.2.3", "include");
            this.headers.Include(includeDir, "*.hpp");
        }

        class SourceFiles : C.Cxx.ObjectFileCollection
        {
            public SourceFiles()
            {
                var sourceDir = this.PackageLocation.SubDirectory("zeromq-3.2.3", "src");
                this.Include(sourceDir, "*.cpp");

                this.UpdateOptions += new Opus.Core.UpdateOptionCollectionDelegate(IncludePath);
                this.UpdateOptions += new Opus.Core.UpdateOptionCollectionDelegate(InternalIncludePath);
                this.UpdateOptions += new Opus.Core.UpdateOptionCollectionDelegate(Exceptions);
                this.UpdateOptions += new Opus.Core.UpdateOptionCollectionDelegate(DisableWarnings);
                this.UpdateOptions += new Opus.Core.UpdateOptionCollectionDelegate(DllExport);
            }

            void DllExport(Opus.Core.IModule module, Opus.Core.Target target)
            {
                if (target.HasPlatform(Opus.Core.EPlatform.Windows) && target.HasToolsetType(typeof(VisualC.Toolset)))
                {
                    var options = module.Options as C.ICCompilerOptions;
                    if (null != options)
                    {
                        options.Defines.Add("DLL_EXPORT");
                    }
                }
            }

            void DisableWarnings(Opus.Core.IModule module, Opus.Core.Target target)
            {
                var options = module.Options as VisualCCommon.ICCompilerOptions;
                if (null != options)
                {
                    options.WarningLevel = VisualCCommon.EWarningLevel.Level3;
                }

                if (target.HasPlatform(Opus.Core.EPlatform.Windows) && target.HasToolsetType(typeof(VisualC.Toolset)))
                {
                    var cOptions = module.Options as C.ICCompilerOptions;
                    if (null != cOptions)
                    {
                        cOptions.Defines.Add("_CRT_SECURE_NO_WARNINGS");
                        cOptions.DisableWarnings.Add("4099"); // C4099: 'zmq::i_msg_sink' : type name first seen using 'class' now seen using 'struct'
                        cOptions.DisableWarnings.Add("4800"); // warning C4800: 'const int' : forcing value to bool 'true' or 'false' (performance warning)
                    }
                }
            }

            void Exceptions(Opus.Core.IModule module, Opus.Core.Target target)
            {
                var options = module.Options as C.ICxxCompilerOptions;
                if (null != options)
                {
                    options.ExceptionHandler = C.Cxx.EExceptionHandler.Asynchronous;
                }
            }

            void InternalIncludePath(Opus.Core.IModule module, Opus.Core.Target target)
            {
                var options = module.Options as C.ICCompilerOptions;
                if (null != options)
                {
                    if (target.HasPlatform(Opus.Core.EPlatform.Windows))
                    {
                        options.IncludePaths.Include(this.PackageLocation, "zeromq-3.2.3", "builds", "msvc");
                    }
                }
            }

            [C.ExportCompilerOptionsDelegate]
            void IncludePath(Opus.Core.IModule module, Opus.Core.Target target)
            {
                var options = module.Options as C.ICCompilerOptions;
                if (null != options)
                {
                    options.IncludePaths.Include(this.PackageLocation, "zeromq-3.2.3", "include");
                }
            }
        }

        [Opus.Core.SourceFiles]
        SourceFiles source = new SourceFiles();

        [C.HeaderFiles]
        Opus.Core.FileCollection headers = new Opus.Core.FileCollection();

        [Opus.Core.DependentModules(Platform=Opus.Core.EPlatform.Windows, ToolsetTypes=new [] { typeof(VisualC.Toolset)})]
        Opus.Core.TypeArray winDependents = new Opus.Core.TypeArray(
            typeof(WindowsSDK.WindowsSDK)
            );

        [C.RequiredLibraries(Platform = Opus.Core.EPlatform.Windows, ToolsetTypes = new[] { typeof(VisualC.Toolset) })]
        Opus.Core.StringArray winLibs = new Opus.Core.StringArray(
            "Ws2_32.lib",
            "Advapi32.lib"
            );
    }
}
