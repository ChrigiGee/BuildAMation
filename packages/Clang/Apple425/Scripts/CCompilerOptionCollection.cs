#region License
// Copyright 2010-2015 Mark Final
//
// This file is part of BuildAMation.
//
// BuildAMation is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// BuildAMation is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with BuildAMation.  If not, see <http://www.gnu.org/licenses/>.
#endregion // License
using C.V2.DefaultSettings;
using Clang.V2.DefaultSettings;
namespace Clang
{
    public static partial class XcodeImplementation
    {
        public static void
        Convert(
            this C.V2.ICommonCompilerOptions options,
            Bam.Core.V2.Module module,
            XcodeBuilder.V2.Configuration configuration)
        {
            var objectFile = module as C.V2.ObjectFile;
            if (null != options.Bits)
            {
                switch (options.Bits)
                {
                    case C.V2.EBit.ThirtyTwo:
                        {
                            configuration["VALID_ARCHS"] = new XcodeBuilder.V2.UniqueConfigurationValue("i386");
                            configuration["ARCHS"] = new XcodeBuilder.V2.UniqueConfigurationValue("$(ARCHS_STANDARD_32_BIT)");
                        }
                        break;

                    case C.V2.EBit.SixtyFour:
                        {
                            configuration["VALID_ARCHS"] = new XcodeBuilder.V2.UniqueConfigurationValue("x86_64");
                            configuration["ARCHS"] = new XcodeBuilder.V2.UniqueConfigurationValue("$(ARCHS_STANDARD_64_BIT)");
                        }
                        break;

                    default:
                        throw new Bam.Core.Exception("Unknown bit depth");
                }
            }
            if (null != options.DebugSymbols)
            {
                configuration["GCC_GENERATE_DEBUGGING_SYMBOLS"] = new XcodeBuilder.V2.UniqueConfigurationValue((options.DebugSymbols == true) ? "YES" : "NO");
            }
            if (options.DisableWarnings.Count > 0)
            {
                var warnings = new XcodeBuilder.V2.MultiConfigurationValue();
                foreach (var warning in options.DisableWarnings)
                {
                    warnings.Add(System.String.Format("-Wno-{0}", warning));
                }
                configuration["WARNING_CFLAGS"] = warnings;
            }
            if (options.IncludePaths.Count > 0)
            {
                var paths = new XcodeBuilder.V2.MultiConfigurationValue();
                foreach (var path in options.IncludePaths)
                {
                    paths.Add(path.ToString());
                }
                configuration["USER_HEADER_SEARCH_PATHS"] = paths;
            }
            if (null != options.LanguageStandard)
            {
                switch (options.LanguageStandard)
                {
                    case C.ELanguageStandard.C89:
                        configuration["GCC_C_LANGUAGE_STANDARD"] = new XcodeBuilder.V2.UniqueConfigurationValue("c89");
                        break;
                    case C.ELanguageStandard.C99:
                        configuration["GCC_C_LANGUAGE_STANDARD"] = new XcodeBuilder.V2.UniqueConfigurationValue("c99");
                        break;
                    default:
                        // TODO: Might want to split this across C specific and Cxx specific options
                        throw new Bam.Core.Exception("Invalid language standard");
                }
            }
            if (null != options.OmitFramePointer)
            {
                var arg = (true == options.OmitFramePointer) ? "-fomit-frame-pointer" : "-fno-omit-frame-pointer";
                configuration["OTHER_CFLAGS"] = new XcodeBuilder.V2.MultiConfigurationValue(arg);
            }
            if (null != options.Optimization)
            {
                switch (options.Optimization)
                {
                    case C.EOptimization.Off:
                        configuration["GCC_OPTIMIZATION_LEVEL"] = new XcodeBuilder.V2.UniqueConfigurationValue("0");
                        break;
                    case C.EOptimization.Size:
                        configuration["GCC_OPTIMIZATION_LEVEL"] = new XcodeBuilder.V2.UniqueConfigurationValue("s");
                        break;
                    case C.EOptimization.Speed:
                        configuration["GCC_OPTIMIZATION_LEVEL"] = new XcodeBuilder.V2.UniqueConfigurationValue("1");
                        break;
                    case C.EOptimization.Full:
                        configuration["GCC_OPTIMIZATION_LEVEL"] = new XcodeBuilder.V2.UniqueConfigurationValue("3");
                        break;
                }
            }
            if (options.PreprocessorDefines.Count > 0)
            {
                var defines = new XcodeBuilder.V2.MultiConfigurationValue();
                foreach (var define in options.PreprocessorDefines)
                {
                    if (System.String.IsNullOrEmpty(define.Value))
                    {
                        defines.Add(define.Key);
                    }
                    else
                    {
                        defines.Add(System.String.Format("{0}={1}", define.Key, define.Value));
                    }
                }
                configuration["GCC_PREPROCESSOR_DEFINITIONS"] = defines;
            }
            if (options.PreprocessorUndefines.Count > 0)
            {
                var undefines = new XcodeBuilder.V2.MultiConfigurationValue();
                foreach (var undefine in options.PreprocessorUndefines)
                {
                    undefines.Add(System.String.Format("-U{0}", undefine));
                }
                configuration["OTHER_CFLAGS"] = undefines;
            }
            if (options.SystemIncludePaths.Count > 0)
            {
                var paths = new XcodeBuilder.V2.MultiConfigurationValue();
                foreach (var path in options.SystemIncludePaths)
                {
                    paths.Add(path.ToString());
                }
                configuration["HEADER_SEARCH_PATHS"] = paths;
            }
            if (null != options.TargetLanguage)
            {
                switch (options.TargetLanguage)
                {
                    case C.ETargetLanguage.Default:
                        configuration["GCC_INPUT_FILETYPE"] = new XcodeBuilder.V2.UniqueConfigurationValue("automatic");
                        break;
                    case C.ETargetLanguage.C:
                        configuration["GCC_INPUT_FILETYPE"] = new XcodeBuilder.V2.UniqueConfigurationValue("sourcecode.c.c");
                        break;
                    case C.ETargetLanguage.Cxx:
                        configuration["GCC_INPUT_FILETYPE"] = new XcodeBuilder.V2.UniqueConfigurationValue("sourcecode.cpp.cpp");
                        break;
                    default:
                        throw new Bam.Core.Exception("Unsupported target language");
                }
            }
            if (null != options.WarningsAsErrors)
            {
                configuration["GCC_TREAT_WARNINGS_AS_ERRORS"] = new XcodeBuilder.V2.UniqueConfigurationValue((true == options.WarningsAsErrors) ? "YES" : "NO");
            }
            if (null != options.OutputType)
            {
                // TODO: anything?
            }
        }
    }

    public static partial class NativeImplementation
    {
        public static void
        Convert(
            this C.V2.ICommonCompilerOptions options,
            Bam.Core.V2.Module module,
            Bam.Core.StringArray commandLine)
        {
            var objectFile = module as C.V2.ObjectFile;
            if (options.Bits == C.V2.EBit.SixtyFour)
            {
                commandLine.Add("-arch x86_64");
            }
            else
            {
                commandLine.Add("-arch i386");
            }
            if (true == options.DebugSymbols)
            {
                commandLine.Add("-g");
            }
            foreach (var warning in options.DisableWarnings)
            {
                commandLine.Add(System.String.Format("-Wno-{0}", warning));
            }
            foreach (var path in options.IncludePaths)
            {
                var formatString = path.ContainsSpace ? "-I\"{0}\"" : "-I{0}";
                commandLine.Add(System.String.Format(formatString, path));
            }
            switch (options.LanguageStandard)
            {
                case C.ELanguageStandard.C89:
                    commandLine.Add("-std=c89");
                    break;
                case C.ELanguageStandard.C99:
                    commandLine.Add("-std=c99");
                    break;
                default:
                    // TODO: Might want to split this across C specific and Cxx specific options
                    throw new Bam.Core.Exception("Invalid language standard");
            }
            if (true == options.OmitFramePointer)
            {
                commandLine.Add("-fomit-frame-pointer");
            }
            else
            {
                commandLine.Add("-fno-omit-frame-pointer");
            }
            switch (options.Optimization)
            {
                case C.EOptimization.Off:
                    commandLine.Add("-O0");
                    break;
                case C.EOptimization.Size:
                    commandLine.Add("-Os");
                    break;
                case C.EOptimization.Speed:
                    commandLine.Add("-O1");
                    break;
                case C.EOptimization.Full:
                    commandLine.Add("-O3");
                    break;
            }
            foreach (var define in options.PreprocessorDefines)
            {
                if (System.String.IsNullOrEmpty(define.Value))
                {
                    commandLine.Add(System.String.Format("-D{0}", define.Key));
                }
                else
                {
                    commandLine.Add(System.String.Format("-D{0}={1}", define.Key, define.Value));
                }
            }
            foreach (var undefine in options.PreprocessorUndefines)
            {
                commandLine.Add(System.String.Format("-U{0}", undefine));
            }
            foreach (var path in options.SystemIncludePaths)
            {
                var formatString = path.ContainsSpace ? "-I\"{0}\"" : "-I{0}";
                commandLine.Add(System.String.Format(formatString, path));
            }
            switch (options.TargetLanguage)
            {
                case C.ETargetLanguage.C:
                    commandLine.Add("-x c");
                    break;
                case C.ETargetLanguage.Cxx:
                    commandLine.Add("-x c++");
                    break;
                default:
                    throw new Bam.Core.Exception("Unsupported target language");
            }
            if (true == options.WarningsAsErrors)
            {
                commandLine.Add("-Werror");
            }
            switch (options.OutputType)
            {
                case C.ECompilerOutput.CompileOnly:
                    commandLine.Add(System.String.Format("-c {0}", objectFile.InputPath.ToString()));
                    commandLine.Add(System.String.Format("-o {0}", module.GeneratedPaths[C.V2.ObjectFile.Key].ToString()));
                    break;
                case C.ECompilerOutput.Preprocess:
                    commandLine.Add(System.String.Format("-E {0}", objectFile.InputPath.ToString()));
                    commandLine.Add(System.String.Format("-o {0}", module.GeneratedPaths[C.V2.ObjectFile.Key].ToString()));
                    break;
            }
        }
    }

    public static partial class NativeImplementation
    {
        public static void
        Convert(
            this C.V2.ICommonArchiverOptions options,
            Bam.Core.V2.Module module,
            Bam.Core.StringArray commandLine)
        {
            switch (options.OutputType)
            {
                case C.EArchiverOutput.StaticLibrary:
                    commandLine.Add(module.GeneratedPaths[C.V2.StaticLibrary.Key].ToString());
                    break;
            }
        }

        public static void
        Convert(
            this V2.IArchiverOptions options,
            Bam.Core.V2.Module module,
            Bam.Core.StringArray commandLine)
        {
            if (options.Ranlib)
            {
                commandLine.Add("-s");
            }
            if (options.DoNotWarnIfLibraryCreated)
            {
                commandLine.Add("-c");
            }
            switch (options.Command)
            {
                case V2.EArchiverCommand.Replace:
                    commandLine.Add("-r");
                    break;

                default:
                    throw new Bam.Core.Exception("No such archiver command");
            }
        }
    }

    public static partial class NativeImplementation
    {
        public static void
        Convert(
            this C.V2.ICommonLinkerOptions options,
            Bam.Core.V2.Module module,
            Bam.Core.StringArray commandLine)
        {
            var applicationFile = module as C.V2.ConsoleApplication;
            switch (options.OutputType)
            {
                case C.ELinkerOutput.Executable:
                    commandLine.Add(System.String.Format("-o {0}", module.GeneratedPaths[C.V2.ConsoleApplication.Key].ToString()));
                    break;

                case C.ELinkerOutput.DynamicLibrary:
                    commandLine.Add("-dynamiclib");
                    commandLine.Add(System.String.Format("-o {0}", module.GeneratedPaths[C.V2.ConsoleApplication.Key].ToString()));
                    // TODO: dylib_install_name
                    // TODO: current_version
                    // TODO: compatability_version
                    break;
            }
            foreach (var path in options.LibraryPaths)
            {
                var format = path.ContainsSpace ? "-L\"{0}\"" : "-L{0}";
                commandLine.Add(System.String.Format(format, path.ToString()));
            }
        }
    }

namespace V2
{
    namespace DefaultSettings
    {
        public static partial class DefaultSettingsExtensions
        {
            public static void Defaults(this IArchiverOptions settings, Bam.Core.V2.Module module)
            {
                settings.Ranlib = true;
                settings.DoNotWarnIfLibraryCreated = true;
                settings.Command = EArchiverCommand.Replace;
            }
        }
    }

    public enum EArchiverCommand
    {
        Replace
    }

    public interface IArchiverOptions
    {
        bool Ranlib
        {
            get;
            set;
        }

        bool DoNotWarnIfLibraryCreated
        {
            get;
            set;
        }

        EArchiverCommand Command
        {
            get;
            set;
        }
    }

    public class CompilerSettings :
        Bam.Core.V2.Settings,
        CommandLineProcessor.V2.IConvertToCommandLine,
        XcodeProjectProcessor.V2.IConvertToProject,
        C.V2.ICommonCompilerOptions,
        C.V2.ICOnlyCompilerOptions
    {
        public CompilerSettings(Bam.Core.V2.Module module)
            : this(module, true)
        {
        }

        public CompilerSettings(Bam.Core.V2.Module module, bool useDefaults)
        {
            (this as C.V2.ICommonCompilerOptions).Empty();
            if (useDefaults)
            {
                (this as C.V2.ICommonCompilerOptions).Defaults(module);
            }
        }

        void CommandLineProcessor.V2.IConvertToCommandLine.Convert(Bam.Core.V2.Module module, Bam.Core.StringArray commandLine)
        {
            (this as C.V2.ICommonCompilerOptions).Convert(module, commandLine);
        }

        void XcodeProjectProcessor.V2.IConvertToProject.Convert(Bam.Core.V2.Module module, XcodeBuilder.V2.Configuration configuration)
        {
            (this as C.V2.ICommonCompilerOptions).Convert(module, configuration);
        }

        C.V2.EBit? C.V2.ICommonCompilerOptions.Bits
        {
            get;
            set;
        }

        C.V2.PreprocessorDefinitions C.V2.ICommonCompilerOptions.PreprocessorDefines
        {
            get;
            set;
        }

        Bam.Core.Array<Bam.Core.V2.TokenizedString> C.V2.ICommonCompilerOptions.IncludePaths
        {
            get;
            set;
        }

        Bam.Core.Array<Bam.Core.V2.TokenizedString> C.V2.ICommonCompilerOptions.SystemIncludePaths
        {
            get;
            set;
        }

        C.ECompilerOutput? C.V2.ICommonCompilerOptions.OutputType
        {
            get;
            set;
        }

        bool? C.V2.ICommonCompilerOptions.DebugSymbols
        {
            get;
            set;
        }

        bool? C.V2.ICommonCompilerOptions.WarningsAsErrors
        {
            get;
            set;
        }

        C.EOptimization? C.V2.ICommonCompilerOptions.Optimization
        {
            get;
            set;
        }

        C.ETargetLanguage? C.V2.ICommonCompilerOptions.TargetLanguage
        {
            get;
            set;
        }

        C.ELanguageStandard? C.V2.ICommonCompilerOptions.LanguageStandard
        {
            get;
            set;
        }

        bool? C.V2.ICommonCompilerOptions.OmitFramePointer
        {
            get;
            set;
        }

        Bam.Core.StringArray C.V2.ICommonCompilerOptions.DisableWarnings
        {
            get;
            set;
        }

        Bam.Core.StringArray C.V2.ICommonCompilerOptions.PreprocessorUndefines
        {
            get;
            set;
        }
    }

    public sealed class CxxCompilerSettings :
        Bam.Core.V2.Settings,
        C.V2.ICommonCompilerOptions,
        C.V2.ICxxOnlyCompilerOptions
    {
        C.V2.EBit? C.V2.ICommonCompilerOptions.Bits
        {
            get;
            set;
        }

        C.V2.PreprocessorDefinitions C.V2.ICommonCompilerOptions.PreprocessorDefines
        {
            get;
            set;
        }

        Bam.Core.Array<Bam.Core.V2.TokenizedString> C.V2.ICommonCompilerOptions.IncludePaths
        {
            get;
            set;
        }

        Bam.Core.Array<Bam.Core.V2.TokenizedString> C.V2.ICommonCompilerOptions.SystemIncludePaths
        {
            get;
            set;
        }

        C.ECompilerOutput? C.V2.ICommonCompilerOptions.OutputType
        {
            get;
            set;
        }

        bool? C.V2.ICommonCompilerOptions.DebugSymbols
        {
            get;
            set;
        }

        bool? C.V2.ICommonCompilerOptions.WarningsAsErrors
        {
            get;
            set;
        }

        C.EOptimization? C.V2.ICommonCompilerOptions.Optimization
        {
            get;
            set;
        }

        C.ETargetLanguage? C.V2.ICommonCompilerOptions.TargetLanguage
        {
            get;
            set;
        }

        C.ELanguageStandard? C.V2.ICommonCompilerOptions.LanguageStandard
        {
            get;
            set;
        }

        bool? C.V2.ICommonCompilerOptions.OmitFramePointer
        {
            get;
            set;
        }

        Bam.Core.StringArray C.V2.ICommonCompilerOptions.DisableWarnings
        {
            get;
            set;
        }

        Bam.Core.StringArray C.V2.ICommonCompilerOptions.PreprocessorUndefines
        {
            get;
            set;
        }

        C.Cxx.EExceptionHandler C.V2.ICxxOnlyCompilerOptions.ExceptionHandler
        {
            get;
            set;
        }
    }

    public class LibrarianSettings :
        Bam.Core.V2.Settings,
        CommandLineProcessor.V2.IConvertToCommandLine,
        C.V2.ICommonArchiverOptions,
        IArchiverOptions
    {
        public LibrarianSettings(Bam.Core.V2.Module module)
        {
            (this as C.V2.ICommonArchiverOptions).Defaults(module);
            (this as IArchiverOptions).Defaults(module);
        }

        void CommandLineProcessor.V2.IConvertToCommandLine.Convert(Bam.Core.V2.Module module, Bam.Core.StringArray commandLine)
        {
            (this as IArchiverOptions).Convert(module, commandLine);
            // output file comes last, before inputs
            (this as C.V2.ICommonArchiverOptions).Convert(module, commandLine);
        }

        C.EArchiverOutput C.V2.ICommonArchiverOptions.OutputType
        {
            get;
            set;
        }

        bool IArchiverOptions.Ranlib
        {
            get;
            set;
        }

        bool IArchiverOptions.DoNotWarnIfLibraryCreated
        {
            get;
            set;
        }

        EArchiverCommand IArchiverOptions.Command
        {
            get;
            set;
        }
    }

    public class LinkerSettings :
        Bam.Core.V2.Settings,
        C.V2.ICommonLinkerOptions,
        CommandLineProcessor.V2.IConvertToCommandLine
    {
        public LinkerSettings(Bam.Core.V2.Module module)
        {
            (this as C.V2.ICommonLinkerOptions).Defaults(module);
        }

        C.ELinkerOutput C.V2.ICommonLinkerOptions.OutputType
        {
            get;
            set;
        }

        Bam.Core.Array<Bam.Core.V2.TokenizedString> C.V2.ICommonLinkerOptions.LibraryPaths
        {
            get;
            set;
        }

        void CommandLineProcessor.V2.IConvertToCommandLine.Convert(Bam.Core.V2.Module module, Bam.Core.StringArray commandLine)
        {
            (this as C.V2.ICommonLinkerOptions).Convert(module, commandLine);
        }
    }

    public static class Configure
    {
        static Configure()
        {
            InstallPath = Bam.Core.V2.TokenizedString.Create(@"/usr/bin", null);
        }

        public static Bam.Core.V2.TokenizedString InstallPath
        {
            get;
            private set;
        }
    }

    [C.V2.RegisterArchiver("Clang", Bam.Core.EPlatform.OSX)]
    public sealed class Librarian :
        C.V2.LibrarianTool
    {
        public Librarian()
        {
            this.Macros.Add("InstallPath", Configure.InstallPath);
            this.Macros.Add("libprefix", "lib");
            this.Macros.Add("libext", ".a");
            this.Macros.Add("LibrarianPath", Bam.Core.V2.TokenizedString.Create("$(InstallPath)/ar", this));
        }

        public override Bam.Core.V2.Settings CreateDefaultSettings<T>(T module)
        {
            var settings = new LibrarianSettings(module);
            return settings;
        }

        public override Bam.Core.V2.TokenizedString Executable
        {
            get
            {
                return this.Macros["LibrarianPath"];
            }
        }
    }

    [C.V2.RegisterLinker("Clang", Bam.Core.EPlatform.OSX)]
    public sealed class Linker :
        C.V2.LinkerTool
    {
        public Linker()
        {
            this.Macros.Add("InstallPath", Configure.InstallPath);
            this.Macros.Add("exeext", string.Empty);
            this.Macros.Add("LinkerPath", Bam.Core.V2.TokenizedString.Create("$(InstallPath)/clang", this));
        }

        public override bool UseLPrefixLibraryPaths
        {
            get
            {
                return true;
            }
        }

        public override Bam.Core.V2.Settings CreateDefaultSettings<T>(T module)
        {
            var settings = new LinkerSettings(module);
            return settings;
        }

        public override Bam.Core.V2.TokenizedString Executable
        {
            get
            {
                return this.Macros["LinkerPath"];
            }
        }
    }

    public abstract class CompilerBase :
        C.V2.CompilerTool
    {
        protected CompilerBase()
        {
            this.Macros.Add("InstallPath", Configure.InstallPath);
            this.Macros.Add("objext", ".o");
        }

        public override Bam.Core.V2.TokenizedString Executable
        {
            get
            {
                return this.Macros["CompilerPath"];
            }
        }

        public override Bam.Core.V2.Settings CreateDefaultSettings<T>(T module)
        {
            if (typeof(C.Cxx.V2.ObjectFile).IsInstanceOfType(module) ||
                typeof(C.Cxx.V2.ObjectFileCollection).IsInstanceOfType(module))
            {
                var settings = new CxxCompilerSettings();
                this.OverrideDefaultSettings(settings);
                return settings;
            }
            else if (typeof(C.V2.ObjectFile).IsInstanceOfType(module) ||
                     typeof(C.V2.CObjectFileCollection).IsInstanceOfType(module))
            {
                var settings = new CompilerSettings(module);
                this.OverrideDefaultSettings(settings);
                return settings;
            }
            // TODO: objective C
            else
            {
                throw new Bam.Core.Exception("Could not determine type of module {0}", typeof(T).ToString());
            }
        }

        protected abstract void OverrideDefaultSettings(Bam.Core.V2.Settings settings);
    }

    [C.V2.RegisterCompiler("Clang", Bam.Core.EPlatform.OSX)]
    public sealed class CCompiler :
        CompilerBase
    {
        public CCompiler()
        {
            this.Macros.Add("CompilerPath", Bam.Core.V2.TokenizedString.Create("$(InstallPath)/clang", this));
        }

        protected override void OverrideDefaultSettings(Bam.Core.V2.Settings settings)
        {
            var cSettings = settings as C.V2.ICommonCompilerOptions;
            cSettings.TargetLanguage = C.ETargetLanguage.C;
        }
    }

    public sealed class CxxCompiler :
        CompilerBase
    {
        public CxxCompiler()
        {
            this.Macros.Add("CompilerPath", Bam.Core.V2.TokenizedString.Create("$(InstallPath)/clang++", this));
        }

        protected override void OverrideDefaultSettings(Bam.Core.V2.Settings settings)
        {
            var cSettings = settings as C.V2.ICommonCompilerOptions;
            cSettings.TargetLanguage = C.ETargetLanguage.Cxx;
        }
    }
}

    public partial class CCompilerOptionCollection :
        ClangCommon.CCompilerOptionCollection,
        C.ICCompilerOptionsOSX
    {
        public
        CCompilerOptionCollection(
            Bam.Core.DependencyNode owningNode) : base(owningNode)
        {}

        protected override void
        SetDefaultOptionValues(
            Bam.Core.DependencyNode node)
        {
            base.SetDefaultOptionValues(node);

            var options = this as C.ICCompilerOptionsOSX;
            options.FrameworkSearchDirectories = new Bam.Core.DirectoryCollection();
            options.SDKVersion = "10.8";
            options.DeploymentTarget = "10.8";
            options.SupportedPlatform = C.EOSXPlatform.MacOSX;
            options.CompilerName = "com.apple.compilers.llvm.clang.1_0";
        }
    }
}
