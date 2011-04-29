// <copyright file="VCProject.cs" company="Mark Final">
//  Opus package
// </copyright>
// <summary>VSSolutionBuilder package</summary>
// <author>Mark Final</author>
namespace VSSolutionBuilder
{
    public class VCProject : ICProject
    {
        private string ProjectName = null;
        private string PathName = null;
        private System.Uri PackageUri = null;
        private System.Guid ProjectGuid = System.Guid.NewGuid();
        private System.Collections.Generic.List<string> PlatformList = new System.Collections.Generic.List<string>();
        private ProjectConfigurationCollection ProjectConfigurations = new ProjectConfigurationCollection();
        private ProjectFileCollection SourceFileCollection = new ProjectFileCollection();
        private ProjectFileCollection HeaderFileCollection = new ProjectFileCollection();
        private System.Collections.Generic.List<IProject> DependentProjectList = new System.Collections.Generic.List<IProject>();

        public VCProject(string moduleName, string projectPathName, string packageDirectory)
        {
            this.ProjectName = moduleName;
            this.PathName = projectPathName;
            this.PackageDirectory = packageDirectory;

            if (packageDirectory[packageDirectory.Length - 1] == System.IO.Path.DirectorySeparatorChar)
            {
                this.PackageUri = new System.Uri(packageDirectory, System.UriKind.Absolute);
            }
            else
            {
                this.PackageUri = new System.Uri(packageDirectory + System.IO.Path.DirectorySeparatorChar, System.UriKind.Absolute);
            }
        }

        string IProject.Name
        {
            get
            {
                return this.ProjectName;
            }
        }

        string IProject.PathName
        {
            get
            {
                return this.PathName;
            }
        }

        System.Guid IProject.Guid
        {
            get
            {
                return this.ProjectGuid;
            }
        }

        public string PackageDirectory
        {
            get;
            private set;
        }

        System.Collections.Generic.List<string> IProject.Platforms
        {
            get
            {
                return this.PlatformList;
            }
        }

        ProjectConfigurationCollection IProject.Configurations
        {
            get
            {
                return this.ProjectConfigurations;
            }
        }

        ProjectFileCollection IProject.SourceFiles
        {
            get
            {
                return this.SourceFileCollection;
            }
        }

        ProjectFileCollection ICProject.HeaderFiles
        {
            get
            {
                return this.HeaderFileCollection;
            }
        }

        System.Collections.Generic.List<IProject> IProject.DependentProjects
        {
            get
            {
                return this.DependentProjectList;
            }
        }

        void IProject.Serialize()
        {
            System.Xml.XmlDocument xmlDocument = null;
            try
            {
                System.Uri projectLocationUri = new System.Uri(this.PathName, System.UriKind.RelativeOrAbsolute);

                xmlDocument = new System.Xml.XmlDocument();

                xmlDocument.AppendChild(xmlDocument.CreateComment("Automatically generated by Opus v" + Opus.Core.State.OpusVersionString));
                System.Xml.XmlElement vsProjectElement = xmlDocument.CreateElement("VisualStudioProject");

                // preamble
                vsProjectElement.SetAttribute("ProjectType", "Visual C++");
                vsProjectElement.SetAttribute("Version", VisualC.Project.Version);
                vsProjectElement.SetAttribute("Name", this.ProjectName);
                vsProjectElement.SetAttribute("ProjectGUID", this.ProjectGuid.ToString("B").ToUpper());

                // platforms
                System.Xml.XmlElement platformsElement = xmlDocument.CreateElement("Platforms");
                foreach (string platform in this.PlatformList)
                {
                    System.Xml.XmlElement platformElement = xmlDocument.CreateElement("Platform");
                    platformElement.SetAttribute("Name", platform);
                    platformsElement.AppendChild(platformElement);
                }
                vsProjectElement.AppendChild(platformsElement);

                // tool files
                // TODO

                // configurations
                System.Xml.XmlElement configurationsElement = xmlDocument.CreateElement("Configurations");
                foreach (ProjectConfiguration configuration in this.ProjectConfigurations)
                {
                    configurationsElement.AppendChild(configuration.Serialize(xmlDocument, projectLocationUri));
                }
                vsProjectElement.AppendChild(configurationsElement);

                // files
                System.Xml.XmlElement filesElement = xmlDocument.CreateElement("Files");
                if (this.SourceFileCollection.Count > 0)
                {
                    filesElement.AppendChild(this.SourceFileCollection.Serialize(xmlDocument, "Source Files", projectLocationUri, this.PackageUri));
                }

                if (this.HeaderFileCollection.Count > 0)
                {
                    filesElement.AppendChild(this.HeaderFileCollection.Serialize(xmlDocument, "Header Files", projectLocationUri, this.PackageUri));
                }
                vsProjectElement.AppendChild(filesElement);

                xmlDocument.AppendChild(vsProjectElement);
            }
            catch (Opus.Core.Exception exception)
            {
                string message = System.String.Format("Xml construction error from project '{0}'", this.PathName);
                throw new Opus.Core.Exception(message, exception);
            }

            // write XML to disk
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(this.PathName));

            System.Xml.XmlWriterSettings xmlWriterSettings = new System.Xml.XmlWriterSettings();
            xmlWriterSettings.Indent = true;
            xmlWriterSettings.CloseOutput = true;
            xmlWriterSettings.OmitXmlDeclaration = false;
            xmlWriterSettings.NewLineOnAttributes = true;

            try
            {
                using (System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(this.PathName, xmlWriterSettings))
                {
                    xmlDocument.Save(xmlWriter);
                }
            }
            catch (Opus.Core.Exception exception)
            {
                string message = System.String.Format("Serialization error from project '{0}'", this.PathName);
                throw new Opus.Core.Exception(message, exception);
            }
        }
    }
}
