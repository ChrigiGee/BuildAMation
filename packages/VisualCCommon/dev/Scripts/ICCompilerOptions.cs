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
namespace V2
{
namespace DefaultSettings
{
    public static partial class DefaultSettingsExtensions
    {
        public static void Defaults(this VisualCCommon.V2.ICommonCompilerOptions settings, Bam.Core.V2.Module module)
        {
            settings.NoLogo = true;
        }

        public static void
        SharedSettings(
            this VisualCCommon.V2.ICommonCompilerOptions shared,
            VisualCCommon.V2.ICommonCompilerOptions lhs,
            VisualCCommon.V2.ICommonCompilerOptions rhs)
        {
            shared.NoLogo = (lhs.NoLogo == rhs.NoLogo) ? lhs.NoLogo : null;
        }

        public static void
        Delta(
            this VisualCCommon.V2.ICommonCompilerOptions delta,
            VisualCCommon.V2.ICommonCompilerOptions lhs,
            VisualCCommon.V2.ICommonCompilerOptions rhs)
        {
            delta.NoLogo = (lhs.NoLogo != rhs.NoLogo) ? lhs.NoLogo : null;
        }

        public static void
        Clone(
            this VisualCCommon.V2.ICommonCompilerOptions settings,
            VisualCCommon.V2.ICommonCompilerOptions other)
        {
            settings.NoLogo = other.NoLogo;
        }
    }
}

    [Bam.Core.V2.SettingsExtensions(typeof(VisualCCommon.V2.DefaultSettings.DefaultSettingsExtensions))]
    public interface ICommonCompilerOptions : Bam.Core.V2.ISettingsBase
    {
        bool? NoLogo
        {
            get;
            set;
        }
    }

    [Bam.Core.V2.SettingsExtensions(typeof(C.V2.DefaultSettings.DefaultSettingsExtensions))]
    public interface ICOnlyCompilerOptions : Bam.Core.V2.ISettingsBase
    {
        int VCCommonCOnly
        {
            get;
            set;
        }
    }

    [Bam.Core.V2.SettingsExtensions(typeof(C.V2.DefaultSettings.DefaultSettingsExtensions))]
    public interface ICxxOnlyCompilerOptions : Bam.Core.V2.ISettingsBase
    {
        string VCCommonCxxOnly
        {
            get;
            set;
        }
    }
}
    // TODO: extend with runtime library option
    public interface ICCompilerOptions
    {
        bool NoLogo
        {
            get;
            set;
        }

        bool MinimalRebuild
        {
            get;
            set;
        }

        VisualCCommon.EWarningLevel WarningLevel
        {
            get;
            set;
        }

        VisualCCommon.EDebugType DebugType
        {
            get;
            set;
        }

        VisualCCommon.EBrowseInformation BrowseInformation
        {
            get;
            set;
        }

        bool StringPooling
        {
            get;
            set;
        }

        bool DisableLanguageExtensions
        {
            get;
            set;
        }

        bool ForceConformanceInForLoopScope
        {
            get;
            set;
        }

        bool UseFullPaths
        {
            get;
            set;
        }

        EManagedCompilation CompileAsManaged
        {
            get;
            set;
        }

        EBasicRuntimeChecks BasicRuntimeChecks
        {
            get;
            set;
        }

        bool SmallerTypeConversionRuntimeCheck
        {
            get;
            set;
        }

        EInlineFunctionExpansion InlineFunctionExpansion
        {
            get;
            set;
        }

        bool EnableIntrinsicFunctions
        {
            get;
            set;
        }

        ERuntimeLibrary RuntimeLibrary
        {
            get;
            set;
        }

        Bam.Core.StringArray ForcedInclude
        {
            get;
            set;
        }
    }
}
