﻿// <copyright file="CSharpProject.cs" company="Mark Final">
//  Opus
// </copyright>
// <summary>Opus CSharp Project.</summary>
// <author>Mark Final</author>
namespace Opus
{
    public enum VisualStudioVersion
    {
        VS2008,
        VS2010
    }
    
    public static class CSharpProject
    {
        public static void Create(Core.PackageInformation package, VisualStudioVersion version, string[] resourceFilePathNames)
        {
            string projectFilename = package.DebugProjectFilename;
            System.Uri projectFilenameUri = new System.Uri(projectFilename);
            string packageName = package.Name;
            string scriptFilename = package.ScriptFile;
            string packageDependencyFilename = package.DependencyFile;

            string OpusDirectory = package.OpusDirectory;
            if (!System.IO.Directory.Exists(OpusDirectory))
            {
                System.IO.Directory.CreateDirectory(OpusDirectory);
            }
            
            System.Xml.XmlWriterSettings xmlWriterSettings = new System.Xml.XmlWriterSettings();
            xmlWriterSettings.Indent = true;
            xmlWriterSettings.CloseOutput = true;
            xmlWriterSettings.OmitXmlDeclaration = true;
            using (System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(projectFilename, xmlWriterSettings))
            {
                xmlWriter.WriteComment("Automatically generated by Opus v" + Core.State.OpusVersionString);
                
                const string ToolsVersion = "3.5";
                const string TargetFrameworkVersion = "v2.0";
              
                xmlWriter.WriteStartElement("Project", "http://schemas.microsoft.com/developer/msbuild/2003");
                xmlWriter.WriteAttributeString("ToolsVersion", ToolsVersion);
                xmlWriter.WriteAttributeString("DefaultTargets", "Build");
                {
                    xmlWriter.WriteStartElement("PropertyGroup");
                    {
                        xmlWriter.WriteStartElement("ProjectGuid");
                        {
                            System.Guid projectGUID = System.Guid.NewGuid();
                            xmlWriter.WriteString(projectGUID.ToString());
                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteStartElement("Configuration");
                        xmlWriter.WriteAttributeString("Condition", " '$(Configuration)' == '' ");
                        {
                            xmlWriter.WriteString("Debug");
                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteStartElement("Platform");
                        xmlWriter.WriteAttributeString("Condition", " '$(Platform)' == '' ");
                        {
                            xmlWriter.WriteString("Any CPU");
                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteStartElement("OutputType");
                        {
                            xmlWriter.WriteString("Library");
                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteStartElement("RootNamespace");
                        {
                            xmlWriter.WriteString(packageName);
                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteStartElement("AssemblyName");
                        {
                            xmlWriter.WriteString(System.IO.Path.GetFileNameWithoutExtension(scriptFilename));
                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteStartElement("TargetFrameworkVersion");
                        {
                            xmlWriter.WriteString(TargetFrameworkVersion);
                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteStartElement("StartArguments");
                        {
                            xmlWriter.WriteString("");
                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteStartElement("WarningLevel");
                        {
                            xmlWriter.WriteValue(4);
                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteStartElement("TreatWarningsAsErrors");
                        {
                            xmlWriter.WriteValue(true);
                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteStartElement("PropertyGroup");
                    xmlWriter.WriteAttributeString("Condition", " '$(Platform)' == 'Any CPU' ");
                    {
                        xmlWriter.WriteStartElement("PlatformTarget");
                        {
                            xmlWriter.WriteString("Any CPU");
                            xmlWriter.WriteEndElement();
                        }
                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteStartElement("PropertyGroup");
                    xmlWriter.WriteAttributeString("Condition", " '$(Configuration)' == 'Debug' ");
                    {
                        xmlWriter.WriteStartElement("OutputPath");
                        {
                            xmlWriter.WriteString(@"bin\Debug");
                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteStartElement("DebugSymbols");
                        {
                            xmlWriter.WriteValue(true);
                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteStartElement("DebugType");
                        {
                            xmlWriter.WriteString("Full");
                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteStartElement("Optimize");
                        {
                            xmlWriter.WriteValue(false);
                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteStartElement("CheckForOverflowUnderflow");
                        {
                            xmlWriter.WriteValue(true);
                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteStartElement("DefineConstants");
                        {
                            xmlWriter.WriteValue(Core.PackageUtilities.OpusVersionDefineForCompiler);
                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteStartElement("AllowUnsafeBlocks");
                        {
                            xmlWriter.WriteValue(false);
                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteStartElement("Import");
                    xmlWriter.WriteAttributeString("Project", @"$(MSBuildBinPath)\Microsoft.CSharp.Targets");
                    {
                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteStartElement("ItemGroup");
                    {
                        xmlWriter.WriteStartElement("Compile");
                        xmlWriter.WriteAttributeString("Include", scriptFilename);
                        {
                            xmlWriter.WriteEndElement();
                        }

                        xmlWriter.WriteStartElement("None");
                        xmlWriter.WriteAttributeString("Include", packageDependencyFilename);
                        {
                            xmlWriter.WriteEndElement();
                        }
                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteStartElement("ItemGroup");
                    {
                        // script files
                        {
                            Core.StringArray scripts = package.Scripts;
                            if (null != scripts)
                            {
                                foreach (string scriptFile in scripts)
                                {
                                    xmlWriter.WriteStartElement("Compile");
                                    {
                                        xmlWriter.WriteAttributeString("Include", scriptFile);
                                        {
                                            xmlWriter.WriteStartElement("Link");
                                            {
                                                string linkPackageFilename = System.IO.Path.Combine("Scripts", System.IO.Path.GetFileName(scriptFile));
                                                xmlWriter.WriteValue(linkPackageFilename);
                                                xmlWriter.WriteEndElement();
                                            }

                                            xmlWriter.WriteEndElement();
                                        }
                                    }
                                }
                            }
                        }

                        // builder scripts
                        {
                            Core.StringArray builderScripts = package.BuilderScripts;
                            if (null != builderScripts)
                            {
                                foreach (string scriptFile in builderScripts)
                                {
                                    xmlWriter.WriteStartElement("Compile");
                                    xmlWriter.WriteAttributeString("Include", scriptFile);
                                    {
                                        xmlWriter.WriteStartElement("Link");
                                        {
                                            string linkFilename = scriptFile.Replace(package.Directory, "");
                                            linkFilename = linkFilename.TrimStart(new char[] { System.IO.Path.DirectorySeparatorChar });
                                            xmlWriter.WriteValue(linkFilename);
                                            xmlWriter.WriteEndElement();
                                        }

                                        xmlWriter.WriteEndElement();
                                    }
                                }
                            }
                        }

                        xmlWriter.WriteEndElement();
                    }

                    // add dependent package source
                    int dependentPackageCount = Core.State.PackageInfo.Count;
                    // start from one as the first one is the main package
                    for (int packageIndex = 1; packageIndex < dependentPackageCount; ++packageIndex)
                    {
                        Core.PackageInformation dependentPackage = Core.State.PackageInfo[packageIndex];

                        Core.Log.DebugMessage("{0}: '{1}-{2}' @ '{3}'", packageIndex, dependentPackage.Name, dependentPackage.Version, dependentPackage.Root);

                        xmlWriter.WriteStartElement("ItemGroup");
                        {
                            // .cs file
                            xmlWriter.WriteStartElement("Compile");
                            xmlWriter.WriteAttributeString("Include", dependentPackage.ScriptFile);
                            {
                                xmlWriter.WriteStartElement("Link");
                                {
                                    string linkPackageFilename = System.IO.Path.Combine("DependentPackages", dependentPackage.Name + "-" + dependentPackage.Version);
                                    linkPackageFilename = System.IO.Path.Combine(linkPackageFilename, System.IO.Path.GetFileName(dependentPackage.ScriptFile));
                                    xmlWriter.WriteValue(linkPackageFilename);
                                    xmlWriter.WriteEndElement();
                                }

                                xmlWriter.WriteEndElement();
                            }

                            // .xml file
                            xmlWriter.WriteStartElement("None");
                            xmlWriter.WriteAttributeString("Include", dependentPackage.DependencyFile);
                            {
                                xmlWriter.WriteStartElement("Link");
                                {
                                    string linkPackageFilename = System.IO.Path.Combine("DependentPackages", dependentPackage.Name + "-" + dependentPackage.Version);
                                    linkPackageFilename = System.IO.Path.Combine(linkPackageFilename, System.IO.Path.GetFileName(dependentPackage.DependencyFile));
                                    xmlWriter.WriteValue(linkPackageFilename);
                                    xmlWriter.WriteEndElement();
                                }

                                xmlWriter.WriteEndElement();
                            }

                            // scripts
                            {
                                Core.StringArray scripts = dependentPackage.Scripts;
                                if (null != scripts)
                                {
                                    foreach (string scriptFile in scripts)
                                    {
                                        xmlWriter.WriteStartElement("Compile");
                                        xmlWriter.WriteAttributeString("Include", scriptFile);
                                        {
                                            xmlWriter.WriteStartElement("Link");
                                            {
                                                string prefix = System.IO.Path.Combine("DependentPackages", dependentPackage.Name + "-" + dependentPackage.Version);
                                                string linkFilename = scriptFile.Replace(dependentPackage.Directory, prefix);
                                                xmlWriter.WriteValue(linkFilename);
                                                xmlWriter.WriteEndElement();
                                            }

                                            xmlWriter.WriteEndElement();
                                        }
                                    }
                                }
                            }

                            // builder scripts
                            {
                                Core.StringArray builderScripts = dependentPackage.BuilderScripts;
                                if (null != builderScripts)
                                {
                                    foreach (string scriptFile in builderScripts)
                                    {
                                        xmlWriter.WriteStartElement("Compile");
                                        xmlWriter.WriteAttributeString("Include", scriptFile);
                                        {
                                            xmlWriter.WriteStartElement("Link");
                                            {
                                                string prefix = System.IO.Path.Combine("DependentPackages", dependentPackage.Name + "-" + dependentPackage.Version);
                                                string linkFilename = scriptFile.Replace(dependentPackage.Directory, prefix);
                                                xmlWriter.WriteValue(linkFilename);
                                                xmlWriter.WriteEndElement();
                                            }

                                            xmlWriter.WriteEndElement();
                                        }
                                    }
                                }
                            }

                            xmlWriter.WriteEndElement();
                        }
                    }

                    // referenced assembles
                    xmlWriter.WriteStartElement("ItemGroup");
                    {
                        System.Reflection.AssemblyName[] referencedAssemblies = System.Reflection.Assembly.GetExecutingAssembly().GetReferencedAssemblies();
                        foreach (System.Reflection.AssemblyName refAssembly in referencedAssemblies)
                        {
                            if (("Opus.Core" == refAssembly.Name) ||
                                ("System" == refAssembly.Name) ||
                                ("System.Xml" == refAssembly.Name))
                            {
                                System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(refAssembly);

                                xmlWriter.WriteStartElement("Reference");
                                xmlWriter.WriteAttributeString("Include", assembly.FullName);
                                {
                                    xmlWriter.WriteStartElement("SpecificVersion");
                                    {
                                        xmlWriter.WriteValue(false);
                                        xmlWriter.WriteEndElement();
                                    }

                                    xmlWriter.WriteStartElement("HintPath");
                                    {
                                        System.Uri assemblyLocationUri = new System.Uri(assembly.Location);
                                        System.Uri relativeAssemblyLocationUri = projectFilenameUri.MakeRelativeUri(assemblyLocationUri);

                                        Core.Log.DebugMessage("Relative path is '{0}'", relativeAssemblyLocationUri.ToString());
                                        xmlWriter.WriteString(relativeAssemblyLocationUri.ToString());
                                        xmlWriter.WriteEndElement();
                                    }

                                    xmlWriter.WriteEndElement();
                                }
                            }
                        }
                        xmlWriter.WriteEndElement();
                    }

                    // embedded resources
                    xmlWriter.WriteStartElement("ItemGroup");
                    {
                        foreach (string resourceFilePathName in resourceFilePathNames)
                        {
                            xmlWriter.WriteStartElement("EmbeddedResource");
                            {
                                xmlWriter.WriteAttributeString("Include", resourceFilePathName);
                                xmlWriter.WriteStartElement("Generator");
                                {
                                    xmlWriter.WriteString("ResXFileCodeGenerator");
                                    xmlWriter.WriteEndElement();
                                }
                                xmlWriter.WriteEndElement();
                            }
                            xmlWriter.WriteEndElement();
                        }
                    }

                    xmlWriter.WriteEndElement();
                }
                xmlWriter.Close();
            }
        }
    }
}