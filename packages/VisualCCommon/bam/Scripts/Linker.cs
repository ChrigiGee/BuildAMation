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
namespace VisualCCommon
{
    public abstract class LinkerBase :
        C.LinkerTool
    {
        public LinkerBase(
            string toolPath,
            string libPath)
        {
            this.Macros.Add("InstallPath", Configure.InstallPath);
            this.Macros.Add("BinPath", Bam.Core.TokenizedString.Create(@"$(InstallPath)\VC\bin", this));
            this.Macros.Add("LinkerPath", Bam.Core.TokenizedString.Create(@"$(InstallPath)" + toolPath, this));
            this.Macros.Add("exeext", ".exe");
            this.Macros.Add("dynamicprefix", string.Empty);
            this.Macros.Add("dynamicext", ".dll");
            this.Macros.Add("libprefix", string.Empty);
            this.Macros.Add("libext", ".lib");

            this.InheritedEnvironmentVariables.Add("TEMP");
            this.InheritedEnvironmentVariables.Add("TMP");

            this.PublicPatch((settings, appliedTo) =>
            {
                var linking = settings as C.ICommonLinkerSettings;
                if (null != linking)
                {
                    linking.LibraryPaths.AddUnique(Bam.Core.TokenizedString.Create(@"$(InstallPath)" + libPath, this));
                }
            });
        }

        public override Bam.Core.Settings
        CreateDefaultSettings<T>(
            T module)
        {
            var settings = new VisualC.LinkerSettings(module);
            return settings;
        }

        public override Bam.Core.TokenizedString Executable
        {
            get
            {
                return this.Macros["LinkerPath"];
            }
        }

        private static string
        GetLibraryPath(
            Bam.Core.Module module)
        {
            if (module is C.StaticLibrary)
            {
                return module.GeneratedPaths[C.StaticLibrary.Key].ToString();
            }
            else if (module is C.DynamicLibrary)
            {
                return module.GeneratedPaths[C.DynamicLibrary.ImportLibraryKey].ToString();
            }
            else if (module is C.CSDKModule)
            {
                // collection of libraries, none in particular
                return null;
            }
            else if (module is C.HeaderLibrary)
            {
                // no library
                return null;
            }
            else if (module is C.ExternalFramework)
            {
                // dealt with elsewhere
                return null;
            }
            else
            {
                throw new Bam.Core.Exception("Unknown module library type: {0}", module.GetType());
            }
        }

        public override void
        ProcessLibraryDependency(
            C.CModule executable,
            C.CModule library)
        {
            var fullLibraryPath = GetLibraryPath(library);
            if (null == fullLibraryPath)
            {
                return;
            }
            var dir = Bam.Core.TokenizedString.Create(System.IO.Path.GetDirectoryName(fullLibraryPath), null);
            var libFilename = System.IO.Path.GetFileName(fullLibraryPath);
            var linker = executable.Settings as C.ICommonLinkerSettings;
            linker.Libraries.AddUnique(libFilename);
            linker.LibraryPaths.AddUnique(dir);
        }
    }

    [C.RegisterCLinker("VisualC", Bam.Core.EPlatform.Windows, C.EBit.ThirtyTwo)]
    [C.RegisterCxxLinker("VisualC", Bam.Core.EPlatform.Windows, C.EBit.ThirtyTwo)]
    public sealed class Linker32 :
        LinkerBase
    {
        public Linker32() :
            base(@"\VC\bin\link.exe", @"\VC\lib")
        {}
    }

    [C.RegisterCLinker("VisualC", Bam.Core.EPlatform.Windows, C.EBit.SixtyFour)]
    [C.RegisterCxxLinker("VisualC", Bam.Core.EPlatform.Windows, C.EBit.SixtyFour)]
    public sealed class Linker64 :
        LinkerBase
    {
        public Linker64() :
            base(@"\VC\bin\x86_amd64\link.exe", @"\VC\lib\amd64")
        {
            // some DLLs exist only in the 32-bit bin folder
            this.EnvironmentVariables.Add("PATH", new Bam.Core.TokenizedStringArray(this.Macros["BinPath"]));
        }
    }
}