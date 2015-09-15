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
namespace VisualC
{
    public static partial class VSSolutionImplementation
    {
        public static void
        Convert(
            this C.ICommonCompilerOptions options,
            Bam.Core.Module module,
            VSSolutionBuilder.VSSettingsGroup settingsGroup,
            string condition)
        {
            // write nothing for disabled debug symbols, otherwise the source files rebuild continually
            // and reports a warning that the pdb file does not exist
            // the IDE can write None into the .vcxproj, but this has the same behaviour
            // https://connect.microsoft.com/VisualStudio/feedback/details/833494/project-with-debug-information-disabled-always-rebuilds
            if (options.DebugSymbols.GetValueOrDefault(false))
            {
                settingsGroup.AddSetting("DebugInformationFormat", "OldStyle", condition);
            }

            if (options.DisableWarnings.Count > 0)
            {
                settingsGroup.AddSetting("DisableSpecificWarnings", options.DisableWarnings, condition, inheritExisting: true);
            }

            if (options.IncludePaths.Count > 0)
            {
                settingsGroup.AddSetting("AdditionalIncludeDirectories", options.IncludePaths, condition, inheritExisting: true);
            }

            if (options.OmitFramePointer.HasValue)
            {
                settingsGroup.AddSetting("OmitFramePointers", options.OmitFramePointer.Value, condition);
            }

            if (options.Optimization.HasValue)
            {
                System.Func<string> optimization = () =>
                    {
                        switch (options.Optimization.Value)
                        {
                            case C.EOptimization.Off:
                                return "Disabled";

                            case C.EOptimization.Size:
                                return "MinSpace";

                            case C.EOptimization.Speed:
                                return "MaxSpeed";

                            case C.EOptimization.Full:
                                return "Full";

                            default:
                                throw new Bam.Core.Exception("Unknown optimization type, {0}", options.Optimization.Value.ToString());
                        }
                    };
                settingsGroup.AddSetting("Optimization", optimization(), condition);
            }

            if (options.PreprocessorDefines.Count > 0)
            {
                settingsGroup.AddSetting("PreprocessorDefinitions", options.PreprocessorDefines, condition, inheritExisting: true);
            }

            if (options.PreprocessorUndefines.Count > 0)
            {
                settingsGroup.AddSetting("UndefinePreprocessorDefinitions", options.PreprocessorUndefines, condition, inheritExisting: true);
            }

            if (options.TargetLanguage.HasValue)
            {
                System.Func<string> targetLanguage = () =>
                {
                    switch (options.TargetLanguage.Value)
                    {
                        case C.ETargetLanguage.C:
                            return "CompileAsC";

                        case C.ETargetLanguage.Cxx:
                            return "CompileAsCpp";

                        case C.ETargetLanguage.Default:
                            return "Default";

                        default:
                            throw new Bam.Core.Exception("Unknown target language, {0}", options.TargetLanguage.Value.ToString());
                    }
                };
                settingsGroup.AddSetting("CompileAs", targetLanguage(), condition);
            }

            if (options.WarningsAsErrors.HasValue)
            {
                settingsGroup.AddSetting("TreatWarningAsError", options.WarningsAsErrors.Value, condition);
            }

            if (options.OutputType.HasValue)
            {
                settingsGroup.AddSetting("PreprocessToFile", options.OutputType.Value == C.ECompilerOutput.Preprocess, condition);
                if (module is C.ObjectFile)
                {
                    settingsGroup.AddSetting("ObjectFileName", module.GeneratedPaths[C.ObjectFile.Key], condition);
                }
            }
        }
    }
}
