#region License
// Copyright (c) 2010-2016, Mark Final
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
using Bam.Core;
namespace CodeGenTest
{
    public sealed class BuildCodeGenTool :
        C.ConsoleApplication,
        Bam.Core.ICommandLineTool
    {
        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);

            this.CreateCSourceContainer("$(packagedir)/source/codegentool/main.c");

            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Windows) &&
                this.Linker is VisualCCommon.LinkerBase)
            {
                this.LinkAgainst<WindowsSDK.WindowsSDK>();
            }
        }

        public Bam.Core.Settings
        CreateDefaultSettings<T>(
            T module) where T : Bam.Core.Module
        {
            return new GeneratedSourceSettings();
        }

        public System.Collections.Generic.Dictionary<string, Bam.Core.TokenizedStringArray> EnvironmentVariables
        {
            get;
            private set;
        }

        public Bam.Core.StringArray InheritedEnvironmentVariables
        {
            get;
            private set;
        }

        public Bam.Core.TokenizedString Executable
        {
            get
            {
                return this.GeneratedPaths[C.ConsoleApplication.Key];
            }
        }

        public Bam.Core.TokenizedStringArray InitialArguments
        {
            get
            {
                return null;
            }
        }

        public Bam.Core.TokenizedStringArray TerminatingArguments
        {
            get
            {
                return null;
            }
        }

        public string UseResponseFileOption
        {
            get
            {
                return null;
            }
        }
    }
}
