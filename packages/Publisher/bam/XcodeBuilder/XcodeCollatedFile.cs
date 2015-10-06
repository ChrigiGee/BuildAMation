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
namespace Publisher
{
    public sealed class XcodeCollatedFile :
        ICollatedFilePolicy
    {
        void
        ICollatedFilePolicy.Collate(
            CollatedFile sender,
            Bam.Core.ExecutionContext context,
            Bam.Core.TokenizedString packageRoot)
        {
            var sourcePath = sender.SourcePath;
            if (null == sender.Reference)
            {
                // no copy is needed, but as we're copying other files relative to this, record where they have to go
                // therefore ignore any subdirectory on this module

                // this has to be the path that Xcode writes to
                sender.DestinationDirectory = Bam.Core.TokenizedString.Create("$(packagebuilddir)/$(config)", sender).Parse();

                // make an app bundle if required
                if ((sender.SubDirectory != null) && sender.SubDirectory.Contains(".app/"))
                {
                    var meta = sender.SourceModule.MetaData as XcodeBuilder.XcodeMeta;
                    meta.Target.MakeApplicationBundle();
                }

                return;
            }

            if (sender.SourceModule.PackageDefinition == sender.Reference.PackageDefinition)
            {
                // same package has the same output folder, so don't bother copying
                // TODO: does the destination directory need to be set?
                return;
            }

            var destinationPath = sender.Reference.DestinationDirectory;
            if (null != sender.SubDirectory)
            {
                destinationPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(destinationPath, sender.SubDirectory));
            }
            destinationPath += System.IO.Path.DirectorySeparatorChar;
            sender.DestinationDirectory = destinationPath;

            var commandLine = new Bam.Core.StringArray();
            (sender.Settings as CommandLineProcessor.IConvertToCommandLine).Convert(sender, commandLine);

            if (sender.SourceModule != null && sender.SourceModule.MetaData != null)
            {
                var commands = new Bam.Core.StringArray();
                commands.Add(System.String.Format("[[ ! -d {0} ]] && mkdir -p {0}", destinationPath));
                commands.Add(System.String.Format("{0} {1} $CONFIGURATION_BUILD_DIR/$EXECUTABLE_NAME {2}", (sender.Tool as Bam.Core.ICommandLineTool).Executable, commandLine.ToString(' '), destinationPath));
                (sender.SourceModule.MetaData as XcodeBuilder.XcodeCommonProject).AddPostBuildCommands(commands);
            }
            else
            {
                var commands = new Bam.Core.StringArray();
                commands.Add(System.String.Format("{0} {1} {2} $CONFIGURATION_BUILD_DIR/{3}/", (sender.Tool as Bam.Core.ICommandLineTool).Executable, commandLine.ToString(' '), sourcePath, sender.SubDirectory));
                (sender.Reference.SourceModule.MetaData as XcodeBuilder.XcodeCommonProject).AddPostBuildCommands(commands);
            }
        }
    }
}