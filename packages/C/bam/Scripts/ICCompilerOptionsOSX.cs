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
using System.Linq;
namespace C
{
namespace DefaultSettings
{
    public static partial class DefaultSettingsExtensions
    {
        public static void
        Defaults(
            this C.ICCompilerOptionsOSX settings,
            Bam.Core.Module module)
        {
        }
        public static void
        Empty(
            this C.ICCompilerOptionsOSX settings)
        {
            settings.FrameworkSearchDirectories = new Bam.Core.Array<Bam.Core.TokenizedString>();
        }
        public static void
        SharedSettings(
            this C.ICCompilerOptionsOSX shared,
            C.ICCompilerOptionsOSX lhs,
            C.ICCompilerOptionsOSX rhs)
        {
            shared.FrameworkSearchDirectories = new Bam.Core.Array<Bam.Core.TokenizedString>(lhs.FrameworkSearchDirectories.Intersect(rhs.FrameworkSearchDirectories));
        }
        public static void
        Delta(
            this C.ICCompilerOptionsOSX delta,
            C.ICCompilerOptionsOSX lhs,
            C.ICCompilerOptionsOSX rhs)
        {
            delta.FrameworkSearchDirectories = new Bam.Core.Array<Bam.Core.TokenizedString>(lhs.FrameworkSearchDirectories.Except(rhs.FrameworkSearchDirectories));
        }
        public static void
        Clone(
            this C.ICCompilerOptionsOSX settings,
            C.ICCompilerOptionsOSX other)
        {
            foreach (var path in other.FrameworkSearchDirectories)
            {
                settings.FrameworkSearchDirectories.AddUnique(path);
            }
        }
    }
}
    [Bam.Core.SettingsExtensions(typeof(C.DefaultSettings.DefaultSettingsExtensions))]
    public interface ICCompilerOptionsOSX :
        Bam.Core.ISettingsBase
    {
        Bam.Core.Array<Bam.Core.TokenizedString> FrameworkSearchDirectories
        {
            get;
            set;
        }
    }

    namespace DefaultSettings
    {
        public static partial class DefaultSettingsExtensions
        {
            public static void
            Defaults(
                this C.ICCompilerOptionsWin settings,
                Bam.Core.Module module)
            {
                settings.CharacterSet = ECharacterSet.NotSet;
            }
            public static void
            Empty(
                this C.ICCompilerOptionsWin settings)
            {
            }
            public static void
            SharedSettings(
                this C.ICCompilerOptionsWin shared,
                C.ICCompilerOptionsWin lhs,
                C.ICCompilerOptionsWin rhs)
            {
                shared.CharacterSet = (lhs.CharacterSet == rhs.CharacterSet) ? lhs.CharacterSet : null;
            }
            public static void
            Delta(
                this C.ICCompilerOptionsWin delta,
                C.ICCompilerOptionsWin lhs,
                C.ICCompilerOptionsWin rhs)
            {
                delta.CharacterSet = (lhs.CharacterSet != rhs.CharacterSet) ? lhs.CharacterSet : null;
            }
            public static void
            Clone(
                this C.ICCompilerOptionsWin settings,
                C.ICCompilerOptionsWin other)
            {
                settings.CharacterSet = other.CharacterSet;
            }
        }
    }
    [Bam.Core.SettingsExtensions(typeof(C.DefaultSettings.DefaultSettingsExtensions))]
    public interface ICCompilerOptionsWin :
        Bam.Core.ISettingsBase
    {
        C.ECharacterSet? CharacterSet
        {
            get;
            set;
        }
    }
}
