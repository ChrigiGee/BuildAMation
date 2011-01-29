// Automatically generated by Opus v0.00
namespace Test13
{
    // Define module classes here
    class QtApplication : C.Application
    {
        private const string WinTarget = "win.*-.*-.*";
        private const string WinVCTarget = "win.*-.*-visualc";
        private const string WinVCDebugTarget = "win.*-debug-visualc";
        private const string WinVCOptimizedTarget = "win.*-optimized-visualc";
        private const string WinMingwTarget = "win.*-.*-mingw";
        private const string WinMingwDebugTarget = "win.*-debug-mingw";
        private const string WinMingwOptimizedTarget = "win.*-optimized-mingw";
        private const string UnixGccTarget = "unix.*-.*-gcc";

        class SourceFiles : C.CPlusPlus.ObjectFileCollection
        {
            public SourceFiles()
            {
                this.AddRelativePaths(this, "source", "*.cpp");
            }

            /*
            class MyMocFile : Qt.MocFile
            {
                public MyMocFile()
                {
                    this.SetRelativePath(this, "source", "myobject.h");
                }
            }
             */
            class MyMocFiles : Qt.MocFileCollection
            {
                public MyMocFiles()
                {
                    this.AddRelativePaths(this, "source", "*.h");
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
            typeof(Qt.Qt)
        );

        [Opus.Core.DependentModules(WinVCTarget)]
        Opus.Core.TypeArray winVCDependents = new Opus.Core.TypeArray(
            typeof(WindowsSDK.WindowsSDK)
        );

        [C.RequiredLibraries(WinMingwDebugTarget)]
        Opus.Core.StringArray winMingwDebugLibraries = new Opus.Core.StringArray(
            "-lQtCored4",
            "-lQtGuid4"
        );

        [C.RequiredLibraries(WinMingwOptimizedTarget)]
        Opus.Core.StringArray winMingwOptimizedLibraries = new Opus.Core.StringArray(
            "-lQtCore4",
            "-lQtGui4"
        );

        [C.RequiredLibraries(WinVCTarget)]
        Opus.Core.StringArray winVCLibraries = new Opus.Core.StringArray(
            "KERNEL32.lib"
        );

        [C.RequiredLibraries(WinVCDebugTarget)]
        Opus.Core.StringArray winVCDebugLibraries = new Opus.Core.StringArray(
            "QtCored4.lib",
            "QtGuid4.lib"
        );

        [C.RequiredLibraries(WinVCOptimizedTarget)]
        Opus.Core.StringArray winVCOptimizedLibraries = new Opus.Core.StringArray(
            "QtCore4.lib",
            "QtGui4.lib"
        );

        [C.RequiredLibraries(UnixGccTarget)]
        Opus.Core.StringArray unixGCCLibraries = new Opus.Core.StringArray(
            "-lQtCore",
            "-lQtGui"
        );
    }
}
 