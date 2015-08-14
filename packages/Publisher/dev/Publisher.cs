#region License
// Copyright (c) 2010-2015, Mark Final
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
//
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// * Neither the name of BuildAMation nor the names of its
//   contributors may be used to endorse or promote products derived from
//   this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion // License
using Bam.Core.V2;

[assembly: Bam.Core.RegisterToolset("Publish", typeof(Publisher.Toolset))]

namespace Publisher
{
namespace V2
{
    public sealed class PackageReference
    {
        public PackageReference(
            Bam.Core.V2.Module module,
            string subdirectory,
            Bam.Core.Array<PackageReference> references)
        {
            this.Module = module;
            this.SubDirectory = subdirectory;
            this.References = references;
        }

        public bool IsMarker
        {
            get
            {
                return (null == this.References);
            }
        }

        public Bam.Core.V2.Module Module
        {
            get;
            private set;
        }

        public string SubDirectory
        {
            get;
            private set;
        }

        public Bam.Core.Array<PackageReference> References
        {
            get;
            private set;
        }

        public string DestinationDir
        {
            get;
            set;
        }
    }

    public interface IPackagePolicy
    {
        void
        Package(
            Package sender,
            Bam.Core.V2.TokenizedString packageRoot,
            System.Collections.ObjectModel.ReadOnlyDictionary<Bam.Core.V2.Module, System.Collections.Generic.Dictionary<Bam.Core.V2.TokenizedString, PackageReference>> packageObjects);
    }

    public abstract class Package :
        Bam.Core.V2.Module
    {
        private System.Collections.Generic.Dictionary<Bam.Core.V2.Module, System.Collections.Generic.Dictionary<Bam.Core.V2.TokenizedString, PackageReference>> dependents = new System.Collections.Generic.Dictionary<Module, System.Collections.Generic.Dictionary<TokenizedString, PackageReference>>();
        private IPackagePolicy Policy = null;
        public static Bam.Core.V2.FileKey PackageRoot = Bam.Core.V2.FileKey.Generate("Package Root");

        protected Package()
        {
            this.RegisterGeneratedFile(PackageRoot, Bam.Core.V2.TokenizedString.Create("$(buildroot)/$(modulename)-$(config)", this));
        }

        public PackageReference
        Include<DependentModule>(
            Bam.Core.V2.FileKey key,
            string subdir = null) where DependentModule : Bam.Core.V2.Module, new()
        {
            var dependent = Bam.Core.V2.Graph.Instance.FindReferencedModule<DependentModule>();
            this.Requires(dependent);
            if (!this.dependents.ContainsKey(dependent))
            {
                this.dependents.Add(dependent, new System.Collections.Generic.Dictionary<TokenizedString, PackageReference>());
            }
            var packaging = new PackageReference(dependent, subdir, null);
            this.dependents[dependent].Add(dependent.GeneratedPaths[key], packaging);
            return packaging;
        }

        public void
        Include<DependentModule>(
            Bam.Core.V2.FileKey key,
            string subdir,
            PackageReference reference,
            params PackageReference[] additionalReferences) where DependentModule : Bam.Core.V2.Module, new()
        {
            var dependent = Bam.Core.V2.Graph.Instance.FindReferencedModule<DependentModule>();
            this.Requires(dependent);
            if (!this.dependents.ContainsKey(dependent))
            {
                this.dependents.Add(dependent, new System.Collections.Generic.Dictionary<TokenizedString, PackageReference>());
            }
            var refs = new Bam.Core.Array<PackageReference>(reference);
            refs.AddRangeUnique(new Bam.Core.Array<PackageReference>(additionalReferences));
            var packaging = new PackageReference(dependent, subdir, refs);
            this.dependents[dependent].Add(dependent.GeneratedPaths[key], packaging);
        }

        public void
        IncludeFiles<DependentModule>(
            string parameterizedFilePath,
            string subdir,
            PackageReference reference,
            params PackageReference[] additionalReferences) where DependentModule : Bam.Core.V2.Module, new()
        {
            var dependent = Bam.Core.V2.Graph.Instance.FindReferencedModule<DependentModule>();
            this.Requires(dependent);
            if (!this.dependents.ContainsKey(dependent))
            {
                this.dependents.Add(dependent, new System.Collections.Generic.Dictionary<TokenizedString, PackageReference>());
            }
            var refs = new Bam.Core.Array<PackageReference>(reference);
            refs.AddRangeUnique(new Bam.Core.Array<PackageReference>(additionalReferences));
            var tokenString = Bam.Core.V2.TokenizedString.Create(parameterizedFilePath, dependent);
            var packaging = new PackageReference(dependent, subdir, refs);
            this.dependents[dependent].Add(tokenString, packaging);
        }

        public override void Evaluate()
        {
            // TODO: should this do at least a timestamp check?
            // do nothing
        }

        protected override void
        ExecuteInternal(
            Bam.Core.V2.ExecutionContext context)
        {
            // TODO: the nested dictionary is not readonly - not sure how to construct this
            var packageObjects = new System.Collections.ObjectModel.ReadOnlyDictionary<Bam.Core.V2.Module, System.Collections.Generic.Dictionary<Bam.Core.V2.TokenizedString, PackageReference>>(this.dependents);
            this.Policy.Package(this, this.GeneratedPaths[PackageRoot], packageObjects);
        }

        protected override void GetExecutionPolicy(string mode)
        {
            var className = "Publisher.V2." + mode + "Packager";
            this.Policy = Bam.Core.V2.ExecutionPolicyUtilities<IPackagePolicy>.Create(className);
        }
    }
}
namespace V2
{
    class InnoSetupScript :
        Bam.Core.V2.Module
    {
        private System.Collections.Generic.Dictionary<Bam.Core.V2.Module, Bam.Core.V2.FileKey> Files = new System.Collections.Generic.Dictionary<Bam.Core.V2.Module, Bam.Core.V2.FileKey>();
        private System.Collections.Generic.Dictionary<Bam.Core.V2.Module, Bam.Core.V2.FileKey> Paths = new System.Collections.Generic.Dictionary<Bam.Core.V2.Module, Bam.Core.V2.FileKey>();

        public InnoSetupScript()
        {
            this.ScriptPath = Bam.Core.V2.TokenizedString.Create("$(buildroot)/$(modulename)/script.iss", this);
        }

        public Bam.Core.V2.TokenizedString ScriptPath
        {
            get;
            private set;
        }

        public void
        AddFile(
            Bam.Core.V2.Module module,
            Bam.Core.V2.FileKey key)
        {
            this.DependsOn(module);
            this.Files.Add(module, key);
        }

        public void
        AddPath(
            Bam.Core.V2.Module module,
            Bam.Core.V2.FileKey key)
        {
            this.DependsOn(module);
            this.Paths.Add(module, key);
        }

        public override void Evaluate()
        {
            // do nothing
        }

        protected override void
        ExecuteInternal(
            Bam.Core.V2.ExecutionContext context)
        {
            var path = this.ScriptPath.Parse();
            var dir = System.IO.Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }
            using (var scriptWriter = new System.IO.StreamWriter(path))
            {
                scriptWriter.WriteLine("[Setup]");
                scriptWriter.WriteLine("AppName={0}", this.GetType().ToString());
                scriptWriter.WriteLine("AppVersion=1.0");
                scriptWriter.WriteLine("DefaultDirName={{sd}}\\{0}", this.GetType().ToString());
                scriptWriter.WriteLine("ArchitecturesAllowed=x64");
                scriptWriter.WriteLine("ArchitecturesInstallIn64BitMode=x64");
                scriptWriter.WriteLine("Uninstallable=No");
                scriptWriter.WriteLine("[Files]");
                foreach (var dep in this.Files)
                {
                    scriptWriter.Write(System.String.Format("Source: \"{0}\"; ", dep.Key.GeneratedPaths[dep.Value]));
                    scriptWriter.Write("DestDir: \"{app}\"; ");
                    scriptWriter.Write("DestName: \"Test\"");
                }
                foreach (var dep in this.Paths)
                {
                    scriptWriter.Write(System.String.Format("Source: \"{0}\\*.*\"; ", dep.Key.GeneratedPaths[dep.Value]));
                    scriptWriter.Write("DestDir: \"{app}\"; ");
                    scriptWriter.Write("Flags: recursesubdirs");
                }
            }
        }

        protected override void GetExecutionPolicy(string mode)
        {
            // do nothing
        }
    }

    public sealed class InnoSetupCompilerSettings :
        Bam.Core.V2.Settings
    {
    }

    public sealed class InnoSetupCompiler :
        Bam.Core.V2.Tool
    {
        public override Bam.Core.V2.Settings CreateDefaultSettings<T>(T module)
        {
            return new InnoSetupCompilerSettings();
        }

        public override Bam.Core.V2.TokenizedString Executable
        {
            get
            {
                return Bam.Core.V2.TokenizedString.Create(@"C:\Program Files (x86)\Inno Setup 5\ISCC.exe", null);
            }
        }
    }

    [Bam.Core.V2.PlatformFilter(Bam.Core.EPlatform.Windows)]
    public abstract class InnoSetupInstaller :
        Bam.Core.V2.Module
    {
        private InnoSetupScript ScriptModule;
        private Bam.Core.V2.Tool Compiler;

        public InnoSetupInstaller()
        {
            // TODO: this actually needs to be a new class each time, otherwise multiple installers won't work
            // need to find a way to instantiate a non-abstract instance of an abstract class
            // looks like emit is needed
            this.ScriptModule = Bam.Core.V2.Graph.Instance.FindReferencedModule<InnoSetupScript>();
            this.DependsOn(this.ScriptModule);

            this.Compiler = Bam.Core.V2.Graph.Instance.FindReferencedModule<InnoSetupCompiler>();
            this.Requires(this.Compiler);
        }

        public void
        Include<DependentModule>(
            Bam.Core.V2.FileKey key) where DependentModule : Bam.Core.V2.Module, new()
        {
            var dependent = Bam.Core.V2.Graph.Instance.FindReferencedModule<DependentModule>();
            this.ScriptModule.AddFile(dependent, key);
        }

        public void
        SourceFolder<DependentModule>(
            Bam.Core.V2.FileKey key) where DependentModule : Bam.Core.V2.Module, new()
        {
            var dependent = Bam.Core.V2.Graph.Instance.FindReferencedModule<DependentModule>();
            this.ScriptModule.AddPath(dependent, key);
        }

        public override void Evaluate()
        {
            // do nothing
        }

        protected override void
        ExecuteInternal(
            Bam.Core.V2.ExecutionContext context)
        {
            var args = new Bam.Core.StringArray();
            args.Add(this.ScriptModule.ScriptPath.Parse());
            CommandLineProcessor.V2.Processor.Execute(context, this.Compiler, args);
        }

        protected override void GetExecutionPolicy(string mode)
        {
            // do nothing
        }
    }
}
namespace V2
{
    class NSISScript :
        Bam.Core.V2.Module
    {
        private System.Collections.Generic.Dictionary<Bam.Core.V2.Module, Bam.Core.V2.FileKey> Files = new System.Collections.Generic.Dictionary<Bam.Core.V2.Module, Bam.Core.V2.FileKey>();
        private System.Collections.Generic.Dictionary<Bam.Core.V2.Module, Bam.Core.V2.FileKey> Paths = new System.Collections.Generic.Dictionary<Bam.Core.V2.Module, Bam.Core.V2.FileKey>();

        public NSISScript()
        {
            this.ScriptPath = Bam.Core.V2.TokenizedString.Create("$(buildroot)/$(modulename)/script.nsi", this);
        }

        public Bam.Core.V2.TokenizedString ScriptPath
        {
            get;
            private set;
        }

        public void
        AddFile(
            Bam.Core.V2.Module module,
            Bam.Core.V2.FileKey key)
        {
            this.DependsOn(module);
            this.Files.Add(module, key);
        }

        public void
        AddPath(
            Bam.Core.V2.Module module,
            Bam.Core.V2.FileKey key)
        {
            this.DependsOn(module);
            this.Paths.Add(module, key);
        }

        public override void Evaluate()
        {
            // do nothing
        }

        protected override void
        ExecuteInternal(
            Bam.Core.V2.ExecutionContext context)
        {
            var path = this.ScriptPath.Parse();
            var dir = System.IO.Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }
            using (var scriptWriter = new System.IO.StreamWriter(path))
            {
                scriptWriter.WriteLine("Name \"{0}\"", this.GetType().ToString());
                scriptWriter.WriteLine("OutFile \"{0}\"", "Installer.exe");
                scriptWriter.WriteLine("InstallDir $PROGRAMFILES64\\{0}", this.GetType().ToString());
                scriptWriter.WriteLine("Page directory");
                scriptWriter.WriteLine("Page instfiles");
                scriptWriter.WriteLine("Section \"\"");
                foreach (var dep in this.Files)
                {
                    scriptWriter.WriteLine("\tSetOutPath $INSTDIR");
                    scriptWriter.WriteLine("\tFile {0}", dep.Key.GeneratedPaths[dep.Value]);
                }
                foreach (var dep in this.Paths)
                {
                    scriptWriter.WriteLine("\tSetOutPath $INSTDIR");
                    scriptWriter.WriteLine("\tFile /r {0}\\*.*", dep.Key.GeneratedPaths[dep.Value]);
                }
                scriptWriter.WriteLine("SectionEnd");
            }
        }

        protected override void GetExecutionPolicy(string mode)
        {
            // do nothing
        }
    }

    public sealed class NSISCompilerSettings :
        Bam.Core.V2.Settings
    {
    }

    public sealed class NSISCompiler :
        Bam.Core.V2.Tool
    {
        public override Bam.Core.V2.Settings CreateDefaultSettings<T>(T module)
        {
            return new NSISCompilerSettings();
        }

        public override Bam.Core.V2.TokenizedString Executable
        {
            get
            {
                return Bam.Core.V2.TokenizedString.Create(@"C:\Program Files (x86)\NSIS\makensis.exe", null);
            }
        }
    }

    [Bam.Core.V2.PlatformFilter(Bam.Core.EPlatform.Windows)]
    public abstract class NSISInstaller :
        Bam.Core.V2.Module
    {
        private NSISScript ScriptModule;
        private Bam.Core.V2.Tool Compiler;

        public NSISInstaller()
        {
            // TODO: this actually needs to be a new class each time, otherwise multiple installers won't work
            // need to find a way to instantiate a non-abstract instance of an abstract class
            // looks like emit is needed
            this.ScriptModule = Bam.Core.V2.Graph.Instance.FindReferencedModule<NSISScript>();
            this.DependsOn(this.ScriptModule);

            this.Compiler = Bam.Core.V2.Graph.Instance.FindReferencedModule<NSISCompiler>();
            this.Requires(this.Compiler);
        }

        public void
        Include<DependentModule>(
            Bam.Core.V2.FileKey key) where DependentModule : Bam.Core.V2.Module, new()
        {
            var dependent = Bam.Core.V2.Graph.Instance.FindReferencedModule<DependentModule>();
            this.ScriptModule.AddFile(dependent, key);
        }

        public void
        SourceFolder<DependentModule>(
            Bam.Core.V2.FileKey key) where DependentModule : Bam.Core.V2.Module, new()
        {
            var dependent = Bam.Core.V2.Graph.Instance.FindReferencedModule<DependentModule>();
            this.ScriptModule.AddPath(dependent, key);
        }

        public override void Evaluate()
        {
            // do nothing
        }

        protected override void
        ExecuteInternal(
            Bam.Core.V2.ExecutionContext context)
        {
            var args = new Bam.Core.StringArray();
            args.Add(this.ScriptModule.ScriptPath.Parse());
            CommandLineProcessor.V2.Processor.Execute(context, this.Compiler, args);
        }

        protected override void GetExecutionPolicy(string mode)
        {
            // do nothing
        }
    }
}
namespace V2
{
    class TarInputFiles :
        Bam.Core.V2.Module
    {
        private System.Collections.Generic.Dictionary<Bam.Core.V2.Module, Bam.Core.V2.FileKey> Files = new System.Collections.Generic.Dictionary<Bam.Core.V2.Module, Bam.Core.V2.FileKey>();

        public TarInputFiles()
        {
            this.ScriptPath = Bam.Core.V2.TokenizedString.Create("$(buildroot)/$(modulename)/tarinput.txt", this);
        }

        public Bam.Core.V2.TokenizedString ScriptPath
        {
            get;
            private set;
        }

        public void
        AddFile(
            Bam.Core.V2.Module module,
            Bam.Core.V2.FileKey key)
        {
            this.DependsOn(module);
            this.Files.Add(module, key);
        }

        public override void Evaluate()
        {
            // do nothing
        }

        protected override void
        ExecuteInternal(
            Bam.Core.V2.ExecutionContext context)
        {
            var path = this.ScriptPath.Parse();
            var dir = System.IO.Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }
            using (var scriptWriter = new System.IO.StreamWriter(path))
            {
                foreach (var dep in this.Files)
                {
                    var filePath = dep.Key.GeneratedPaths[dep.Value].ToString();
                    var fileDir = System.IO.Path.GetDirectoryName(filePath);
                    // TODO: this should probably be a setting
                    if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.OSX))
                    {
                        scriptWriter.WriteLine("-C");
                        scriptWriter.WriteLine(fileDir);
                    }
                    else
                    {
                        scriptWriter.WriteLine("-C {0}", fileDir);
                    }
                    scriptWriter.WriteLine(System.IO.Path.GetFileName(filePath));
                }
            }
        }

        protected override void GetExecutionPolicy(string mode)
        {
            // do nothing
        }
    }

    public sealed class TarSettings :
        Bam.Core.V2.Settings
    {
    }

    public sealed class TarCompiler :
        Bam.Core.V2.Tool
    {
        public override Bam.Core.V2.Settings CreateDefaultSettings<T>(T module)
        {
            return new TarSettings();
        }

        public override Bam.Core.V2.TokenizedString Executable
        {
            get
            {
                return Bam.Core.V2.TokenizedString.Create("tar", null);
            }
        }
    }

    [Bam.Core.V2.PlatformFilter(Bam.Core.EPlatform.Unix | Bam.Core.EPlatform.OSX)]
    public abstract class TarBall :
        Bam.Core.V2.Module
    {
        public static Bam.Core.V2.FileKey Key = Bam.Core.V2.FileKey.Generate("Installer");

        private TarInputFiles InputFiles;
        private Bam.Core.V2.Tool Compiler;

        public TarBall()
        {
            this.RegisterGeneratedFile(Key, Bam.Core.V2.TokenizedString.Create("$(buildroot)/installer.tar", this));

            // TODO: this actually needs to be a new class each time, otherwise multiple installers won't work
            // need to find a way to instantiate a non-abstract instance of an abstract class
            // looks like emit is needed
            this.InputFiles = Bam.Core.V2.Graph.Instance.FindReferencedModule<TarInputFiles>();
            this.DependsOn(this.InputFiles);

            this.Compiler = Bam.Core.V2.Graph.Instance.FindReferencedModule<TarCompiler>();
            this.Requires(this.Compiler);
        }

        public void
        Include<DependentModule>(
            Bam.Core.V2.FileKey key) where DependentModule : Bam.Core.V2.Module, new()
        {
            var dependent = Bam.Core.V2.Graph.Instance.FindReferencedModule<DependentModule>();
            this.InputFiles.AddFile(dependent, key);
        }

        public override void Evaluate()
        {
            // do nothing
        }

        protected override void
        ExecuteInternal(
            Bam.Core.V2.ExecutionContext context)
        {
            var args = new Bam.Core.StringArray();
            args.Add("-c");
            args.Add("-T");
            args.Add(this.InputFiles.ScriptPath.Parse());
            args.Add("-f");
            args.Add(this.GeneratedPaths[Key].ToString());
            CommandLineProcessor.V2.Processor.Execute(context, this.Compiler, args);
        }

        protected override void GetExecutionPolicy(string mode)
        {
            // do nothing
        }
    }
}
namespace V2
{
    public sealed class DiskImageSettings :
        Bam.Core.V2.Settings
    {
    }

    public sealed class DiskImageCompiler :
        Bam.Core.V2.Tool
    {
        public override Bam.Core.V2.Settings CreateDefaultSettings<T>(T module)
        {
            return new TarSettings();
        }

        public override Bam.Core.V2.TokenizedString Executable
        {
            get
            {
                return Bam.Core.V2.TokenizedString.Create("hdiutil", null);
            }
        }
    }

    [Bam.Core.V2.PlatformFilter(Bam.Core.EPlatform.OSX)]
    public abstract class DiskImage :
        Bam.Core.V2.Module
    {
        public static Bam.Core.V2.FileKey Key = Bam.Core.V2.FileKey.Generate("Installer");

        private Bam.Core.V2.TokenizedString SourceFolderPath;
        private Bam.Core.V2.Tool Compiler;

        public DiskImage()
        {
            this.RegisterGeneratedFile(Key, Bam.Core.V2.TokenizedString.Create("$(buildroot)/installer.dmg", this));

            this.Compiler = Bam.Core.V2.Graph.Instance.FindReferencedModule<DiskImageCompiler>();
            this.Requires(this.Compiler);
        }

        public void
        SourceFolder<DependentModule>(
            Bam.Core.V2.FileKey key) where DependentModule : Bam.Core.V2.Module, new()
        {
            var dependent = Bam.Core.V2.Graph.Instance.FindReferencedModule<DependentModule>();
            this.DependsOn(dependent);
            this.SourceFolderPath = dependent.GeneratedPaths[key];
        }

        public override void Evaluate()
        {
            // do nothing
        }

        protected override void
        ExecuteInternal(
            Bam.Core.V2.ExecutionContext context)
        {
            var volumeName = "My Volume";
            var diskImagePathName = this.GeneratedPaths[Key].ToString();

            // create the disk image
            {
                var args = new Bam.Core.StringArray();
                args.Add("create");
                args.Add("-quiet");
                args.Add("-srcfolder");
                args.Add(System.String.Format("\"{0}\"", this.SourceFolderPath.ToString()));
                args.Add("-size");
                args.Add("32m");
                args.Add("-fs");
                args.Add("HFS+");
                args.Add("-volname");
                args.Add(System.String.Format("\"{0}\"", volumeName));
                args.Add(diskImagePathName);
                CommandLineProcessor.V2.Processor.Execute(context, this.Compiler, args);
            }

            // mount disk image
            {
                var args = new Bam.Core.StringArray();
                args.Add("attach");
                args.Add("-quiet");
                args.Add(diskImagePathName);
                CommandLineProcessor.V2.Processor.Execute(context, this.Compiler, args);
            }

                // TODO
                /// do a copy

            // unmount disk image
            {
                var args = new Bam.Core.StringArray();
                args.Add("detach");
                args.Add("-quiet");
                args.Add(System.String.Format("\"/Volumes/{0}\"", volumeName));
                CommandLineProcessor.V2.Processor.Execute(context, this.Compiler, args);
            }

            // hdiutil convert myimg.dmg -format UDZO -o myoutputimg.dmg
            {
                var args = new Bam.Core.StringArray();
                args.Add("convert");
                args.Add("-quiet");
                args.Add(diskImagePathName);
                args.Add("-format");
                args.Add("UDZ0");
                args.Add("-o");
                args.Add(diskImagePathName);
                CommandLineProcessor.V2.Processor.Execute(context, this.Compiler, args);
            }
        }

        protected override void GetExecutionPolicy(string mode)
        {
            // do nothing
        }
    }
}
    // Add modules here
}
