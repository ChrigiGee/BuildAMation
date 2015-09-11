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
    public abstract class Toolset :
        Bam.Core.IToolset
    {
        public string installPath;
        public string bin32Folder;
        public string bin64Folder;
        public string bin6432Folder;
        public Bam.Core.StringArray lib32Folder = new Bam.Core.StringArray();
        public Bam.Core.StringArray lib64Folder = new Bam.Core.StringArray();
        protected Bam.Core.StringArray environment = new Bam.Core.StringArray();

        protected abstract void
        GetInstallPath();

        protected abstract string
        GetVersion(
            Bam.Core.BaseTarget baseTarget);

        protected System.Collections.Generic.Dictionary<System.Type, Bam.Core.ToolAndOptionType> toolConfig = new System.Collections.Generic.Dictionary<System.Type, Bam.Core.ToolAndOptionType>();

        protected
        Toolset()
        {
            this.toolConfig[typeof(C.INullOpTool)] = new Bam.Core.ToolAndOptionType(null, null);
            this.toolConfig[typeof(C.IThirdPartyTool)] = new Bam.Core.ToolAndOptionType(null, typeof(C.ThirdPartyOptionCollection));
        }

        protected virtual string
        GetBinPath(
            Bam.Core.BaseTarget baseTarget)
        {
            this.GetInstallPath();

            if (baseTarget.HasPlatform(Bam.Core.EPlatform.Win64))
            {
                if (Bam.Core.OSUtilities.Is64BitHosting)
                {
                    return this.bin64Folder;
                }
                else
                {
                    return this.bin6432Folder;
                }
            }
            else
            {
                return this.bin32Folder;
            }
        }

        private static string
        GetVCRegistryKeyPath(
            string platformName,
            string versionNumber,
            string editionName,
            int LCID)
        {
            var keyPath = System.String.Format(@"Microsoft\DevDiv\{0}\Servicing\{1}\{2}\{3}", platformName, versionNumber, editionName, LCID);
            return keyPath;
        }

        private static string
        GetVersionAndEditionString(
            string registryKeyPath,
            string versionNumber)
        {
            string edition = null;
            using (var key = Bam.Core.Win32RegistryUtilities.Open32BitLMSoftwareKey(registryKeyPath))
            {
                if (null == key)
                {
                    throw new Bam.Core.Exception(System.String.Format("Unable to locate registry key, '{0}', for the VisualStudio service pack", registryKeyPath), false);
                }

                edition = key.GetValue("SPName") as string;
            }

            var version = System.String.Format("{0}{1}", versionNumber, edition);
            return version;
        }

        protected string
        GetVersionString(
            string versionNumber)
        {
            var LCID = 1033; // Culture (English - United States)
            string platformName;
            string editionName;
            string vcRegistryKeyPath;

            // try professional version first
            platformName = "VS";
            editionName = "PRO";
            vcRegistryKeyPath = GetVCRegistryKeyPath(platformName, versionNumber, editionName, LCID);
            if (Bam.Core.Win32RegistryUtilities.Does32BitLMSoftwareKeyExist(vcRegistryKeyPath))
            {
                var versionString = GetVersionAndEditionString(vcRegistryKeyPath, versionNumber);
                return versionString;
            }
            else
            {
                // now try Express edition
                platformName = "VC";
                editionName = "EXP";
                vcRegistryKeyPath = GetVCRegistryKeyPath(platformName, versionNumber, editionName, LCID);
                if (Bam.Core.Win32RegistryUtilities.Does32BitLMSoftwareKeyExist(vcRegistryKeyPath))
                {
                    var versionString = GetVersionAndEditionString(vcRegistryKeyPath, versionNumber);
                    return versionString;
                }
                else
                {
                    // now try regular edition
                    platformName = "VC";
                    editionName = "RED";
                    vcRegistryKeyPath = GetVCRegistryKeyPath(platformName, versionNumber, editionName, LCID);
                    if (Bam.Core.Win32RegistryUtilities.Does32BitLMSoftwareKeyExist(vcRegistryKeyPath))
                    {
                        var versionString = GetVersionAndEditionString(vcRegistryKeyPath, versionNumber);
                        return versionString;
                    }
                    else
                    {
                        // now try latest
                        platformName = "VC";
                        editionName = "CompilerCore";
                        vcRegistryKeyPath = GetVCRegistryKeyPath(platformName, versionNumber, editionName, LCID);
                        if (Bam.Core.Win32RegistryUtilities.Does32BitLMSoftwareKeyExist(vcRegistryKeyPath))
                        {
                            var versionString = GetVersionAndEditionString(vcRegistryKeyPath, versionNumber);
                            return versionString;
                        }
                        else
                        {
                            throw new Bam.Core.Exception("Unable to locate registry key, '{0}', for the VisualStudio {1} service pack", vcRegistryKeyPath, versionNumber);
                        }
                    }
                }
            }
        }

        #region IToolset Members

        string
        Bam.Core.IToolset.BinPath(
            Bam.Core.BaseTarget baseTarget)
        {
            return this.GetBinPath(baseTarget);
        }

        Bam.Core.StringArray Bam.Core.IToolset.Environment
        {
            get
            {
                this.GetInstallPath();
                return this.environment;
            }
        }

        string
        Bam.Core.IToolset.InstallPath(
            Bam.Core.BaseTarget baseTarget)
        {
            this.GetInstallPath();
            return this.installPath;
        }

        string
        Bam.Core.IToolset.Version(
            Bam.Core.BaseTarget baseTarget)
        {
            return this.GetVersion(baseTarget);
        }

        bool
        Bam.Core.IToolset.HasTool(
            System.Type toolType)
        {
            return this.toolConfig.ContainsKey(toolType);
        }

        Bam.Core.ITool
        Bam.Core.IToolset.Tool(
            System.Type toolType)
        {
            if (!(this as Bam.Core.IToolset).HasTool(toolType))
            {
                throw new Bam.Core.Exception("Tool '{0}' was not registered with toolset '{1}'", toolType.ToString(), this.ToString());
            }

            return this.toolConfig[toolType].Tool;
        }

        System.Type
        Bam.Core.IToolset.ToolOptionType(
            System.Type toolType)
        {
            if (!(this as Bam.Core.IToolset).HasTool(toolType))
            {
                throw new Bam.Core.Exception("Tool '{0}' has no option type registered with toolset '{1}'", toolType.ToString(), this.ToString());
            }

            return this.toolConfig[toolType].OptionsType;
        }

        #endregion
    }
}