// Automatically generated by Opus v0.30
namespace AMDAPPSDK
{
    // Add modules here
    class AMDAPPSDK : C.ThirdPartyModule
    {
        static AMDAPPSDK()
        {
            InstallPath = null;

            if (Opus.Core.State.HasCategory("AMDAPPSDK") && Opus.Core.State.Has("AMDAPPSDK", "InstallPath"))
            {
                InstallPath = Opus.Core.State.Get("AMDAPPSDK", "InstallPath") as string;
                Opus.Core.Log.DebugMessage("AMDAPPSDK install path set from command line to '{0}'", InstallPath);
            }

            if (!Opus.Core.OSUtilities.IsWindowsHosting)
            {
                throw new Opus.Core.Exception("AMDAPPSDK is only supported on Windows currently");
            }

            if (null == InstallPath)
            {
                using (Microsoft.Win32.RegistryKey key = Opus.Core.Win32RegistryUtilities.Open32BitLMSoftwareKey(@"ATI Technologies\Install\AMD APP SDK Developer"))
                {
                    if (null == key)
                    {
                        throw new Opus.Core.Exception("AMDAPPSDK was not installed");
                    }

                    string installPath = key.GetValue("InstallDir") as string;
                    if (null == installPath)
                    {
                        throw new Opus.Core.Exception("AMDAPPSDK was not installed correctly");
                    }

                    installPath = installPath.TrimEnd(new[] { System.IO.Path.DirectorySeparatorChar });
                    Opus.Core.Log.DebugMessage("VisualStudio 2008: Installation path from registry '{0}'", installPath);
                    InstallPath = installPath;
                }
            }

            IncludePath = System.IO.Path.Combine(InstallPath, "include");
            LibraryPath = System.IO.Path.Combine(InstallPath, "lib");
        }

        public AMDAPPSDK()
        {
            this.UpdateOptions += new Opus.Core.UpdateOptionCollectionDelegate(AMDAPPSDK_IncludePath);
            this.UpdateOptions += new Opus.Core.UpdateOptionCollectionDelegate(AMDAPPSDK_LinkerOptions);
            this.UpdateOptions += new Opus.Core.UpdateOptionCollectionDelegate(AMDAPPSDK_EnableExceptionHandling);
        }

        [C.ExportCompilerOptionsDelegate]
        void AMDAPPSDK_EnableExceptionHandling(Opus.Core.IModule module, Opus.Core.Target target)
        {
            var compilerOptions = module.Options as C.ICxxCompilerOptions;
            if (null == compilerOptions)
            {
                return;
            }

            compilerOptions.ExceptionHandler = C.Cxx.EExceptionHandler.Asynchronous;
        }

        [C.ExportCompilerOptionsDelegate]
        void AMDAPPSDK_IncludePath(Opus.Core.IModule module, Opus.Core.Target target)
        {
            var compilerOptions = module.Options as C.ICCompilerOptions;
            if (null == compilerOptions)
            {
                return;
            }

            compilerOptions.IncludePaths.Add(IncludePath);
        }

        [C.ExportLinkerOptionsDelegate]
        void AMDAPPSDK_LinkerOptions(Opus.Core.IModule module, Opus.Core.Target target)
        {
            var linkerOptions = module.Options as C.ILinkerOptions;
            if (null == linkerOptions)
            {
                return;
            }

            // set library paths
            string platformLibraryPath = null;
            if (target.HasPlatform(Opus.Core.EPlatform.Win32))
            {
                platformLibraryPath = System.IO.Path.Combine(LibraryPath, "x86");
            }
            else if (target.HasPlatform(Opus.Core.EPlatform.Win64))
            {
                platformLibraryPath = System.IO.Path.Combine(LibraryPath, "x86_64");
            }
            else
            {
                throw new Opus.Core.Exception("Unsupported platform for the DirectX package");
            }
            linkerOptions.LibraryPaths.Add(platformLibraryPath);

            // libraries to link against
            linkerOptions.Libraries.Add("OpenCL.lib");
        }

        [Opus.Core.DependentModules(Platform = Opus.Core.EPlatform.Windows)]
        Opus.Core.TypeArray winDependents = new Opus.Core.TypeArray(typeof(WindowsSDK.WindowsSDK));

        public static string InstallPath
        {
            get;
            private set;
        }

        public static string IncludePath
        {
            get;
            private set;
        }

        public static string LibraryPath
        {
            get;
            private set;
        }
    }
}
