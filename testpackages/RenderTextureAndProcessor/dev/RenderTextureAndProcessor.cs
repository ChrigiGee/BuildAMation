// Automatically generated by Opus v0.00
namespace RenderTextureAndProcessor
{
    // Define module classes here
    class RenderTexture : C.WindowsApplication
    {
        public RenderTexture()
        {
            this.headerFiles.AddRelativePaths(this, "source", "common", "*.h");
            this.headerFiles.AddRelativePaths(this, "source", "rendertexture", "*.h");
        }

        class SourceFiles : C.CPlusPlus.ObjectFileCollection
        {
            public SourceFiles()
            {
                this.AddRelativePaths(this, "source", "common", "*.cpp");
                this.AddRelativePaths(this, "source", "rendertexture", "*.cpp");
                this.UpdateOptions += new Opus.Core.UpdateOptionCollectionDelegate(SourceFiles_UpdateOptions);
            }

            void SourceFiles_UpdateOptions(Opus.Core.IModule module, Opus.Core.Target target)
            {
                {
                    C.ICPlusPlusCompilerOptions options = module.Options as C.ICPlusPlusCompilerOptions;
                    options.ExceptionHandler = C.CPlusPlus.EExceptionHandler.Synchronous;
                }

                {
                    C.ICCompilerOptions options = module.Options as C.ICCompilerOptions;
                    options.IncludePaths.Add(this, @"source/common");
                }
            }
        }

        [Opus.Core.SourceFiles]
        SourceFiles sourceFiles = new SourceFiles();

        [C.HeaderFiles]
        Opus.Core.FileCollection headerFiles = new Opus.Core.FileCollection();

        [C.RequiredLibraries(Platform=Opus.Core.EPlatform.Windows, Toolchains=new string[] { "visualc" })]
        Opus.Core.StringArray requiredLibrariesVC = new Opus.Core.StringArray(
            "KERNEL32.lib",
            "GDI32.lib",
            "USER32.lib",
            "OPENGL32.lib",
            "WS2_32.lib",
            "SHELL32.lib"
        );

        [C.RequiredLibraries(Platform = Opus.Core.EPlatform.Windows, Toolchains = new string[] { "mingw" })]
        Opus.Core.StringArray requiredLibrariesMingw = new Opus.Core.StringArray(
            "-lws2_32",
            "-lopengl32",
            "-lgdi32"
        );

        [Opus.Core.DependentModules(Platform = Opus.Core.EPlatform.Windows, Toolchains = new string[] { "visualc" })]
        Opus.Core.TypeArray dependentModules = new Opus.Core.TypeArray(
            typeof(WindowsSDK.WindowsSDK),
            typeof(OpenGLSDK.OpenGL)
        );

        [Opus.Core.RequiredModules]
        Opus.Core.TypeArray requiredModules = new Opus.Core.TypeArray(typeof(TextureProcessor));
    }

    class TextureProcessor : C.Application
    {
        public TextureProcessor()
        {
            this.headerFiles.AddRelativePaths(this, "source", "common", "*.h");
            this.headerFiles.AddRelativePaths(this, "source", "textureprocessor", "*.h");
        }

        class SourceFiles : C.CPlusPlus.ObjectFileCollection
        {
            public SourceFiles()
            {
                this.AddRelativePaths(this, "source", "common", "*.cpp");
                this.AddRelativePaths(this, "source", "textureprocessor", "*.cpp");
                this.UpdateOptions += new Opus.Core.UpdateOptionCollectionDelegate(SourceFiles_UpdateOptions);
            }

            void SourceFiles_UpdateOptions(Opus.Core.IModule module, Opus.Core.Target target)
            {
                {
                    C.ICPlusPlusCompilerOptions options = module.Options as C.ICPlusPlusCompilerOptions;
                    options.ExceptionHandler = C.CPlusPlus.EExceptionHandler.Synchronous;
                }

                {
                    C.ICCompilerOptions options = module.Options as C.ICCompilerOptions;
                    options.IncludePaths.Add(this, @"source/common");
                }
            }
        }

        [C.RequiredLibraries(Platform = Opus.Core.EPlatform.Windows, Toolchains = new string[] { "visualc" })]
        Opus.Core.StringArray requiredLibrariesVC = new Opus.Core.StringArray(
            "KERNEL32.lib",
            "WS2_32.lib"
        );

        [C.RequiredLibraries(Platform = Opus.Core.EPlatform.Windows, Toolchains = new string[] { "mingw" })]
        Opus.Core.StringArray requiredLibrariesMingw = new Opus.Core.StringArray("-lws2_32");

        [Opus.Core.SourceFiles]
        SourceFiles sourceFiles = new SourceFiles();

        [C.HeaderFiles]
        Opus.Core.FileCollection headerFiles = new Opus.Core.FileCollection();

        [Opus.Core.DependentModules(Platform = Opus.Core.EPlatform.Windows, Toolchains = new string[] { "visualc" })]
        Opus.Core.TypeArray dependentModules = new Opus.Core.TypeArray(typeof(WindowsSDK.WindowsSDK));
    }
}