// Automatically generated file from OpusOptionCodeGenerator.
// Command line:
// -i=../../../C/dev/Scripts/ILinkerOptions.cs:../../../C/dev/Scripts/ILinkerOptionsOSX.cs:ILinkerOptions.cs -n=GccCommon -c=LinkerOptionCollection -p -d -dd=../../../CommandLineProcessor/dev/Scripts/CommandLineDelegate.cs:../../../XcodeProjectProcessor/dev/Scripts/Delegate.cs -pv=PrivateData

namespace GccCommon
{
    public partial class LinkerOptionCollection
    {
        #region C.ILinkerOptions Option delegates
        private static void OutputTypeCommandLineProcessor(object sender, Opus.Core.StringArray commandLineBuilder, Opus.Core.Option option, Opus.Core.Target target)
        {
            var enumOption = option as Opus.Core.ValueTypeOption<C.ELinkerOutput>;
            var options = sender as LinkerOptionCollection;
            switch (enumOption.Value)
            {
                case C.ELinkerOutput.Executable:
                    {
                    #if true
                        var outputPath = options.OwningNode.Module.Locations[C.Application.OutputFile].GetSinglePath();
                        // TODO: isn't there an option for this on the tool?
                        commandLineBuilder.Add(System.String.Format("-o {0}", outputPath));
                    #else
                        string outputPathName = options.OutputFilePath;
                        if (outputPathName.Contains(" "))
                        {
                            commandLineBuilder.Add(System.String.Format("-o \"{0}\"", outputPathName));
                        }
                        else
                        {
                            commandLineBuilder.Add(System.String.Format("-o {0}", outputPathName));
                        }
                    #endif
                    }
                    break;
                case C.ELinkerOutput.DynamicLibrary:
                    {
                    #if true
                        var outputPath = options.OwningNode.Module.Locations[C.Application.OutputFile].GetSinglePath();
                        // TODO: isn't there an option for this on the tool?
                        commandLineBuilder.Add(System.String.Format("-o {0}", outputPath));
                    #else
                        string outputPathName = options.OutputFilePath;
                        if (outputPathName.Contains(" "))
                        {
                            commandLineBuilder.Add(System.String.Format("-o \"{0}\"", outputPathName));
                        }
                        else
                        {
                            commandLineBuilder.Add(System.String.Format("-o {0}", outputPathName));
                        }
                    #endif
                        // TODO: this option needs to be pulled out of the common output type option
                        // TODO: this needs more work, re: revisions
                        // see http://tldp.org/HOWTO/Program-Library-HOWTO/shared-libraries.html
                        // see http://www.adp-gmbh.ch/cpp/gcc/create_lib.html
                        // see http://lists.apple.com/archives/unix-porting/2003/Oct/msg00032.html
                        if (Opus.Core.OSUtilities.IsUnixHosting)
                        {
                        #if true
                            var leafname = System.IO.Path.GetFileName(outputPath);
                            var splitLeafName = leafname.Split('.');
                            // index 0: filename without extension
                            // index 1: 'so'
                            // index 2: major version number
                            var soName = System.String.Format("{0}.{1}.{2}", splitLeafName[0], splitLeafName[1], splitLeafName[2]);
                            commandLineBuilder.Add(System.String.Format("-Wl,-soname,{0}", soName));
                        #else
                            if (outputPathName.Contains(" "))
                            {
                                commandLineBuilder.Add(System.String.Format("-Wl,-soname,\"{0}\"", outputPathName));
                            }
                            else
                            {
                                commandLineBuilder.Add(System.String.Format("-Wl,-soname,{0}", outputPathName));
                            }
                        #endif
                        }
                        else if (Opus.Core.OSUtilities.IsOSXHosting)
                        {
                        #if true
                            var filename = System.IO.Path.GetFileName(outputPath);
                            commandLineBuilder.Add(System.String.Format("-Wl,-dylib_install_name,@executable_path/{0}", filename));
                        #else
                            var filename = System.IO.Path.GetFileName(outputPathName);
                            if (filename.Contains(" "))
                            {
                                commandLineBuilder.Add(System.String.Format("-Wl,-dylib_install_name,\"@executable_path/{0}\"", filename));
                            }
                            else
                            {
                                commandLineBuilder.Add(System.String.Format("-Wl,-dylib_install_name,@executable_path/{0}", filename));
                            }
                        #endif
                            var linkerOptions = sender as C.ILinkerOptions;
                            commandLineBuilder.Add(System.String.Format("-Wl,-current_version,{0}.{1}.{2}", linkerOptions.MajorVersion, linkerOptions.MinorVersion, linkerOptions.PatchVersion));
                            // TODO: this needs to have a proper option
                            commandLineBuilder.Add(System.String.Format("-Wl,-compatibility_version,{0}.{1}.{2}", linkerOptions.MajorVersion, linkerOptions.MinorVersion, linkerOptions.PatchVersion));
                        }
                    }
                    break;
                default:
                    throw new Opus.Core.Exception("Unrecognized value for C.ELinkerOutput");
            }
        }
        private static void OutputTypeXcodeProjectProcessor(object sender, XcodeBuilder.PBXProject project, XcodeBuilder.XCodeNodeData currentObject, XcodeBuilder.XCBuildConfiguration configuration, Opus.Core.Option option, Opus.Core.Target target)
        {
            var enumOption = option as Opus.Core.ValueTypeOption<C.ELinkerOutput>;
            if (enumOption.Value != C.ELinkerOutput.DynamicLibrary)
            {
                return;
            }

            var options = sender as LinkerOptionCollection;
            {
                var installNameOption = configuration.Options["LD_DYLIB_INSTALL_NAME"];
                var outputPath = options.OwningNode.Module.Locations[C.Application.OutputFile].GetSinglePath();
                var filename = System.IO.Path.GetFileName(outputPath);
                var installName = System.String.Format("@executable_path/{0}", filename);
                installNameOption.AddUnique(installName);
            }
            var linkerOptions = sender as C.ILinkerOptions;
            {
                var currentVersionOption = configuration.Options["DYLIB_CURRENT_VERSION"];
                var version = System.String.Format("{0}.{1}.{2}", linkerOptions.MajorVersion, linkerOptions.MinorVersion, linkerOptions.PatchVersion);
                currentVersionOption.AddUnique(version);
            }
            {
                var compatibilityVersionOption = configuration.Options["DYLIB_COMPATIBILITY_VERSION"];
                var version = System.String.Format("{0}.{1}.{2}", linkerOptions.MajorVersion, linkerOptions.MinorVersion, linkerOptions.PatchVersion);
                compatibilityVersionOption.AddUnique(version);
            }
        }
        private static void DoNotAutoIncludeStandardLibrariesCommandLineProcessor(object sender, Opus.Core.StringArray commandLineBuilder, Opus.Core.Option option, Opus.Core.Target target)
        {
            var ignoreStandardLibrariesOption = option as Opus.Core.ValueTypeOption<bool>;
            if (ignoreStandardLibrariesOption.Value)
            {
                commandLineBuilder.Add("-nostdlib");
            }
        }
        private static void DoNotAutoIncludeStandardLibrariesXcodeProjectProcessor(object sender, XcodeBuilder.PBXProject project, XcodeBuilder.XCodeNodeData currentObject, XcodeBuilder.XCBuildConfiguration configuration, Opus.Core.Option option, Opus.Core.Target target)
        {
            var ignoreStandardLibs = option as Opus.Core.ValueTypeOption<bool>;
            var linkWithStandardLibsOption = configuration.Options["LINK_WITH_STANDARD_LIBRARIES"];
            if (ignoreStandardLibs.Value)
            {
                linkWithStandardLibsOption.AddUnique("NO");
            }
            else
            {
                linkWithStandardLibsOption.AddUnique("YES");
            }
            if (linkWithStandardLibsOption.Count != 1)
            {
                throw new Opus.Core.Exception("More than one ignore standard libraries option has been set");
            }
        }
        private static void DebugSymbolsCommandLineProcessor(object sender, Opus.Core.StringArray commandLineBuilder, Opus.Core.Option option, Opus.Core.Target target)
        {
            var debugSymbolsOption = option as Opus.Core.ValueTypeOption<bool>;
            if (debugSymbolsOption.Value)
            {
                commandLineBuilder.Add("-g");
            }
        }
        private static void DebugSymbolsXcodeProjectProcessor(object sender, XcodeBuilder.PBXProject project, XcodeBuilder.XCodeNodeData currentObject, XcodeBuilder.XCBuildConfiguration configuration, Opus.Core.Option option, Opus.Core.Target target)
        {
            var debugSymbols = option as Opus.Core.ValueTypeOption<bool>;
            var otherLDOptions = configuration.Options["OTHER_LDFLAGS"];
            if (debugSymbols.Value)
            {
                otherLDOptions.AddUnique("-g");
            }
        }
        private static void SubSystemCommandLineProcessor(object sender, Opus.Core.StringArray commandLineBuilder, Opus.Core.Option option, Opus.Core.Target target)
        {
            // empty
        }
        private static void SubSystemXcodeProjectProcessor(object sender, XcodeBuilder.PBXProject project, XcodeBuilder.XCodeNodeData currentObject, XcodeBuilder.XCBuildConfiguration configuration, Opus.Core.Option option, Opus.Core.Target target)
        {
            // empty
        }
        private static void DynamicLibraryCommandLineProcessor(object sender, Opus.Core.StringArray commandLineBuilder, Opus.Core.Option option, Opus.Core.Target target)
        {
            var dynamicLibraryOption = option as Opus.Core.ValueTypeOption<bool>;
            if (dynamicLibraryOption.Value)
            {
                if (Opus.Core.OSUtilities.IsUnixHosting)
                {
                    commandLineBuilder.Add("-shared");
                }
                else if (Opus.Core.OSUtilities.IsOSXHosting)
                {
                    commandLineBuilder.Add("-dynamiclib");
                }
            }
        }
        private static void DynamicLibraryXcodeProjectProcessor(object sender, XcodeBuilder.PBXProject project, XcodeBuilder.XCodeNodeData currentObject, XcodeBuilder.XCBuildConfiguration configuration, Opus.Core.Option option, Opus.Core.Target target)
        {
            // TODO: this looks like it might actually be MACH_O_TYPE=mh_dylib or mh_execute
            var dynamicLibrary = option as Opus.Core.ValueTypeOption<bool>;
            var otherLDOptions = configuration.Options["OTHER_LDFLAGS"];
            if (dynamicLibrary.Value)
            {
                otherLDOptions.AddUnique("-dynamiclib");
            }
        }
        private static void LibraryPathsCommandLineProcessor(object sender, Opus.Core.StringArray commandLineBuilder, Opus.Core.Option option, Opus.Core.Target target)
        {
            var libraryPathsOption = option as Opus.Core.ReferenceTypeOption<Opus.Core.DirectoryCollection>;
            // TODO: convert to var
            foreach (string libraryPath in libraryPathsOption.Value)
            {
                if (libraryPath.Contains(" "))
                {
                    commandLineBuilder.Add(System.String.Format("-L\"{0}\"", libraryPath));
                }
                else
                {
                    commandLineBuilder.Add(System.String.Format("-L{0}", libraryPath));
                }
            }
        }
        private static void LibraryPathsXcodeProjectProcessor(object sender, XcodeBuilder.PBXProject project, XcodeBuilder.XCodeNodeData currentObject, XcodeBuilder.XCBuildConfiguration configuration, Opus.Core.Option option, Opus.Core.Target target)
        {
            var libraryPathsOption = option as Opus.Core.ReferenceTypeOption<Opus.Core.DirectoryCollection>;
            var librarySearchPathsOption = configuration.Options["LIBRARY_SEARCH_PATHS"];
            librarySearchPathsOption.AddRangeUnique(libraryPathsOption.Value.ToStringArray());
        }
        private static void StandardLibrariesCommandLineProcessor(object sender, Opus.Core.StringArray commandLineBuilder, Opus.Core.Option option, Opus.Core.Target target)
        {
            var librariesOption = option as Opus.Core.ReferenceTypeOption<Opus.Core.FileCollection>;
            // TODO: change to var, and returning Locations
            foreach (Opus.Core.Location libraryPath in librariesOption.Value)
            {
                commandLineBuilder.Add(libraryPath.GetSinglePath());
            }
        }
        private static void StandardLibrariesXcodeProjectProcessor(object sender, XcodeBuilder.PBXProject project, XcodeBuilder.XCodeNodeData currentObject, XcodeBuilder.XCBuildConfiguration configuration, Opus.Core.Option option, Opus.Core.Target target)
        {
            // empty
        }
        private static void LibrariesCommandLineProcessor(object sender, Opus.Core.StringArray commandLineBuilder, Opus.Core.Option option, Opus.Core.Target target)
        {
            var librariesOption = option as Opus.Core.ReferenceTypeOption<Opus.Core.FileCollection>;
            // TODO: change to var, and returning Locations
            foreach (Opus.Core.Location libraryPath in librariesOption.Value)
            {
                commandLineBuilder.Add(libraryPath.GetSinglePath());
            }
        }
        private static void LibrariesXcodeProjectProcessor(object sender, XcodeBuilder.PBXProject project, XcodeBuilder.XCodeNodeData currentObject, XcodeBuilder.XCBuildConfiguration configuration, Opus.Core.Option option, Opus.Core.Target target)
        {
            // empty
        }
        private static void GenerateMapFileCommandLineProcessor(object sender, Opus.Core.StringArray commandLineBuilder, Opus.Core.Option option, Opus.Core.Target target)
        {
            var boolOption = option as Opus.Core.ValueTypeOption<bool>;
            if (boolOption.Value)
            {
                //var options = sender as LinkerOptionCollection;
                var mapFileLoc = (sender as LinkerOptionCollection).OwningNode.Module.Locations[C.Application.MapFile];
                if (Opus.Core.OSUtilities.IsUnixHosting)
                {
                    #if true
                    commandLineBuilder.Add(System.String.Format("-Wl,-Map,{0}", mapFileLoc.GetSinglePath()));
                    // TODO: map file
                    #else
                    if (options.MapFilePath.Contains(" "))
                    {
                        commandLineBuilder.Add(System.String.Format("-Wl,-Map,\"{0}\"", options.MapFilePath));
                    }
                    else
                    {
                        commandLineBuilder.Add(System.String.Format("-Wl,-Map,{0}", options.MapFilePath));
                    }
                    #endif
                }
                else if (Opus.Core.OSUtilities.IsOSXHosting)
                {
                    #if true
                    commandLineBuilder.Add(System.String.Format("-Wl,-map,{0}", mapFileLoc.GetSinglePath()));
                    #else
                    if (options.MapFilePath.Contains(" "))
                    {
                        commandLineBuilder.Add(System.String.Format("-Wl,-map,\"{0}\"", options.MapFilePath));
                    }
                    else
                    {
                        commandLineBuilder.Add(System.String.Format("-Wl,-map,{0}", options.MapFilePath));
                    }
                    #endif
                }
            }
        }
        private static void GenerateMapFileXcodeProjectProcessor(object sender, XcodeBuilder.PBXProject project, XcodeBuilder.XCodeNodeData currentObject, XcodeBuilder.XCBuildConfiguration configuration, Opus.Core.Option option, Opus.Core.Target target)
        {
            var generateMapfile = option as Opus.Core.ValueTypeOption<bool>;
            if (generateMapfile.Value)
            {
                #if true
                var mapFileLoc = (sender as LinkerOptionCollection).OwningNode.Module.Locations[C.Application.MapFile];
                var generateMapfileOption = configuration.Options["LD_MAP_FILE_PATH"];
                generateMapfileOption.AddUnique(mapFileLoc.GetSinglePath());
                #else
                var generateMapfileOption = configuration.Options["LD_MAP_FILE_PATH"];
                var options = sender as LinkerOptionCollection;
                generateMapfileOption.AddUnique(options.MapFilePath);
                #endif
                if (generateMapfileOption.Count != 1)
                {
                    throw new Opus.Core.Exception("More than one map file location option has been set");
                }
            }
        }
        private static void AdditionalOptionsCommandLineProcessor(object sender, Opus.Core.StringArray commandLineBuilder, Opus.Core.Option option, Opus.Core.Target target)
        {
            var stringOption = option as Opus.Core.ReferenceTypeOption<string>;
            var arguments = stringOption.Value.Split(' ');
            foreach (var argument in arguments)
            {
                commandLineBuilder.Add(argument);
            }
        }
        private static void AdditionalOptionsXcodeProjectProcessor(object sender, XcodeBuilder.PBXProject project, XcodeBuilder.XCodeNodeData currentObject, XcodeBuilder.XCBuildConfiguration configuration, Opus.Core.Option option, Opus.Core.Target target)
        {
            var stringOption = option as Opus.Core.ReferenceTypeOption<string>;
            var arguments = stringOption.Value.Split(' ');
            var otherLDOptions = configuration.Options["OTHER_LDFLAGS"];
            foreach (var argument in arguments)
            {
                otherLDOptions.AddUnique(argument);
            }
        }
        #endregion
        #region C.ILinkerOptionsOSX Option delegates
        private static void FrameworksCommandLineProcessor(object sender, Opus.Core.StringArray commandLineBuilder, Opus.Core.Option option, Opus.Core.Target target)
        {
            if (!Opus.Core.OSUtilities.IsOSXHosting)
            {
                return;
            }
            var stringArrayOption = option as Opus.Core.ReferenceTypeOption<Opus.Core.StringArray>;
            foreach (var framework in stringArrayOption.Value)
            {
                commandLineBuilder.Add(System.String.Format("-framework {0}", framework));
            }
        }
        private static void FrameworksXcodeProjectProcessor(object sender, XcodeBuilder.PBXProject project, XcodeBuilder.XCodeNodeData currentObject, XcodeBuilder.XCBuildConfiguration configuration, Opus.Core.Option option, Opus.Core.Target target)
        {
            var frameworks = option as Opus.Core.ReferenceTypeOption<Opus.Core.StringArray>;
            foreach (var framework in frameworks.Value)
            {
                var fileReference = project.FileReferences.Get(framework, XcodeBuilder.PBXFileReference.EType.Framework, framework, null);
                var frameworksBuildPhase = project.FrameworksBuildPhases.Get("Frameworks", currentObject.Name);
                var buildFile = project.BuildFiles.Get(framework, fileReference, frameworksBuildPhase);
                if (null == buildFile)
                {
                    throw new Opus.Core.Exception("Build file not available");
                }
            }
        }
        private static void FrameworkSearchDirectoriesCommandLineProcessor(object sender, Opus.Core.StringArray commandLineBuilder, Opus.Core.Option option, Opus.Core.Target target)
        {
            var switchPrefix = "-F";
            var frameworkIncludePathsOption = option as Opus.Core.ReferenceTypeOption<Opus.Core.DirectoryCollection>;
            // TODO: convert to 'var'
            foreach (string includePath in frameworkIncludePathsOption.Value)
            {
                if (includePath.Contains(" "))
                {
                    commandLineBuilder.Add(System.String.Format("{0}\"{1}\"", switchPrefix, includePath));
                }
                else
                {
                    commandLineBuilder.Add(System.String.Format("{0}{1}", switchPrefix, includePath));
                }
            }
        }
        private static void FrameworkSearchDirectoriesXcodeProjectProcessor(object sender, XcodeBuilder.PBXProject project, XcodeBuilder.XCodeNodeData currentObject, XcodeBuilder.XCBuildConfiguration configuration, Opus.Core.Option option, Opus.Core.Target target)
        {
            var frameworkPathsOption = option as Opus.Core.ReferenceTypeOption<Opus.Core.DirectoryCollection>;
            configuration.Options["FRAMEWORK_SEARCH_PATHS"].AddRangeUnique(frameworkPathsOption.Value.ToStringArray());
        }
        private static void SuppressReadOnlyRelocationsCommandLineProcessor(object sender, Opus.Core.StringArray commandLineBuilder, Opus.Core.Option option, Opus.Core.Target target)
        {
            if (!target.HasPlatform(Opus.Core.EPlatform.OSX))
            {
                return;
            }
            var readOnlyRelocations = option as Opus.Core.ValueTypeOption<bool>;
            if (readOnlyRelocations.Value)
            {
                commandLineBuilder.Add(System.String.Format("-Wl,-read_only_relocs,suppress"));
            }
        }
        private static void SuppressReadOnlyRelocationsXcodeProjectProcessor(object sender, XcodeBuilder.PBXProject project, XcodeBuilder.XCodeNodeData currentObject, XcodeBuilder.XCBuildConfiguration configuration, Opus.Core.Option option, Opus.Core.Target target)
        {
            var readOnlyRelocations = option as Opus.Core.ValueTypeOption<bool>;
            var otherLDOptions = configuration.Options["OTHER_LDFLAGS"];
            if (readOnlyRelocations.Value)
            {
                otherLDOptions.AddUnique("-Wl,-read_only_relocs,suppress");
            }
        }
        #endregion
        #region ILinkerOptions Option delegates
        private static void CanUseOriginCommandLineProcessor(object sender, Opus.Core.StringArray commandLineBuilder, Opus.Core.Option option, Opus.Core.Target target)
        {
            // $ORIGIN not supported on OSX linkers - use install name, etc
            if (target.HasPlatform(Opus.Core.EPlatform.OSX))
            {
                return;
            }

            var boolOption = option as Opus.Core.ValueTypeOption<bool>;
            if (boolOption.Value)
            {
                commandLineBuilder.Add("-Wl,-z,origin");
            }
        }
        private static void CanUseOriginXcodeProjectProcessor(object sender, XcodeBuilder.PBXProject project, XcodeBuilder.XCodeNodeData currentObject, XcodeBuilder.XCBuildConfiguration configuration, Opus.Core.Option option, Opus.Core.Target target)
        {
            // $ORIGIN not supported on OSX linkers - use install name, etc
            if (target.HasPlatform(Opus.Core.EPlatform.OSX))
            {
                return;
            }

            var useOrigin = option as Opus.Core.ValueTypeOption<bool>;
            var otherLDOptions = configuration.Options["OTHER_LDFLAGS"];
            if (useOrigin.Value)
            {
                otherLDOptions.AddUnique("-Wl,-z,origin");
            }
        }
        private static void AllowUndefinedSymbolsCommandLineProcessor(object sender, Opus.Core.StringArray commandLineBuilder, Opus.Core.Option option, Opus.Core.Target target)
        {
            var boolOption = option as Opus.Core.ValueTypeOption<bool>;
            if (boolOption.Value)
            {
                if (Opus.Core.OSUtilities.IsOSXHosting)
                {
                    // TODO: I did originally think suppress here, but there is an issue with that and 'two level namespaces'
                    commandLineBuilder.Add("-Wl,-undefined,dynamic_lookup");
                }
                else
                {
                    commandLineBuilder.Add("-Wl,-z,nodefs");
                }
            }
            else
            {
                if (Opus.Core.OSUtilities.IsOSXHosting)
                {
                    commandLineBuilder.Add("-Wl,-undefined,error");
                }
                else
                {
                    commandLineBuilder.Add("-Wl,-z,defs");
                }
            }
        }
        private static void AllowUndefinedSymbolsXcodeProjectProcessor(object sender, XcodeBuilder.PBXProject project, XcodeBuilder.XCodeNodeData currentObject, XcodeBuilder.XCBuildConfiguration configuration, Opus.Core.Option option, Opus.Core.Target target)
        {
            var allowUndefined = option as Opus.Core.ValueTypeOption<bool>;
            var otherLDOptions = configuration.Options["OTHER_LDFLAGS"];
            if (allowUndefined.Value)
            {
                // TODO: I did originally think suppress here, but there is an issue with that and 'two level namespaces'
                otherLDOptions.AddUnique("-Wl,-undefined,dynamic_lookup");
            }
            else
            {
                otherLDOptions.AddUnique("-Wl,-undefined,error");
            }
        }
        private static void RPathCommandLineProcessor(object sender, Opus.Core.StringArray commandLineBuilder, Opus.Core.Option option, Opus.Core.Target target)
        {
            var stringsOption = option as Opus.Core.ReferenceTypeOption<Opus.Core.StringArray>;
            foreach (string rpath in stringsOption.Value)
            {
                commandLineBuilder.Add(System.String.Format("-Wl,-rpath,{0}", rpath));
            }
        }
        private static void RPathXcodeProjectProcessor(object sender, XcodeBuilder.PBXProject project, XcodeBuilder.XCodeNodeData currentObject, XcodeBuilder.XCBuildConfiguration configuration, Opus.Core.Option option, Opus.Core.Target target)
        {
            var rpathOption = option as Opus.Core.ReferenceTypeOption<Opus.Core.StringArray>;
            configuration.Options["LD_RUNPATH_SEARCH_PATHS"].AddRangeUnique(rpathOption.Value);
        }
        private static void SixtyFourBitCommandLineProcessor(object sender, Opus.Core.StringArray commandLineBuilder, Opus.Core.Option option, Opus.Core.Target target)
        {
            var sixtyFourBitOption = option as Opus.Core.ValueTypeOption<bool>;
            if (sixtyFourBitOption.Value)
            {
                commandLineBuilder.Add("-m64");
            }
            else
            {
                commandLineBuilder.Add("-m32");
            }
        }
        private static void SixtyFourBitXcodeProjectProcessor(object sender, XcodeBuilder.PBXProject project, XcodeBuilder.XCodeNodeData currentObject, XcodeBuilder.XCBuildConfiguration configuration, Opus.Core.Option option, Opus.Core.Target target)
        {
        }
        #endregion
        protected override void SetDelegates(Opus.Core.DependencyNode node)
        {
            this["OutputType"].PrivateData = new PrivateData(OutputTypeCommandLineProcessor,OutputTypeXcodeProjectProcessor);
            this["DoNotAutoIncludeStandardLibraries"].PrivateData = new PrivateData(DoNotAutoIncludeStandardLibrariesCommandLineProcessor,DoNotAutoIncludeStandardLibrariesXcodeProjectProcessor);
            this["DebugSymbols"].PrivateData = new PrivateData(DebugSymbolsCommandLineProcessor,DebugSymbolsXcodeProjectProcessor);
            this["SubSystem"].PrivateData = new PrivateData(SubSystemCommandLineProcessor,SubSystemXcodeProjectProcessor);
            this["DynamicLibrary"].PrivateData = new PrivateData(DynamicLibraryCommandLineProcessor,DynamicLibraryXcodeProjectProcessor);
            this["LibraryPaths"].PrivateData = new PrivateData(LibraryPathsCommandLineProcessor,LibraryPathsXcodeProjectProcessor);
            this["StandardLibraries"].PrivateData = new PrivateData(StandardLibrariesCommandLineProcessor,StandardLibrariesXcodeProjectProcessor);
            this["Libraries"].PrivateData = new PrivateData(LibrariesCommandLineProcessor,LibrariesXcodeProjectProcessor);
            this["GenerateMapFile"].PrivateData = new PrivateData(GenerateMapFileCommandLineProcessor,GenerateMapFileXcodeProjectProcessor);
            this["AdditionalOptions"].PrivateData = new PrivateData(AdditionalOptionsCommandLineProcessor,AdditionalOptionsXcodeProjectProcessor);
            // Property 'MajorVersion' is state only
            // Property 'MinorVersion' is state only
            // Property 'PatchVersion' is state only
            this["Frameworks"].PrivateData = new PrivateData(FrameworksCommandLineProcessor,FrameworksXcodeProjectProcessor);
            this["FrameworkSearchDirectories"].PrivateData = new PrivateData(FrameworkSearchDirectoriesCommandLineProcessor,FrameworkSearchDirectoriesXcodeProjectProcessor);
            this["SuppressReadOnlyRelocations"].PrivateData = new PrivateData(SuppressReadOnlyRelocationsCommandLineProcessor,SuppressReadOnlyRelocationsXcodeProjectProcessor);
            this["CanUseOrigin"].PrivateData = new PrivateData(CanUseOriginCommandLineProcessor,CanUseOriginXcodeProjectProcessor);
            this["AllowUndefinedSymbols"].PrivateData = new PrivateData(AllowUndefinedSymbolsCommandLineProcessor,AllowUndefinedSymbolsXcodeProjectProcessor);
            this["RPath"].PrivateData = new PrivateData(RPathCommandLineProcessor,RPathXcodeProjectProcessor);
            this["SixtyFourBit"].PrivateData = new PrivateData(SixtyFourBitCommandLineProcessor,SixtyFourBitXcodeProjectProcessor);
        }
    }
}
