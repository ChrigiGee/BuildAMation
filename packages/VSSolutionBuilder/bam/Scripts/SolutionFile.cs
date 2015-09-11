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
namespace VSSolutionBuilder
{
    public sealed class SolutionFile
    {
        public
        SolutionFile(
            string pathName)
        {
            this.PathName = pathName;
            this.ProjectDictionary = new System.Collections.Generic.Dictionary<string, IProject>();
            this.ProjectConfigurations = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<IProject>>();
        }

        public string PathName
        {
            get;
            private set;
        }

        public System.Collections.Generic.Dictionary<string, IProject> ProjectDictionary
        {
            get;
            private set;
        }

        //TODO: not sure if this is necessary
        public System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<IProject>> ProjectConfigurations
        {
            get;
            private set;
        }

        public void
        ResolveSourceFileConfigurationExclusions()
        {
            foreach (var project in this.ProjectDictionary)
            {
                var projectData = project.Value;
                // TODO: convert to var
                foreach (ProjectFile sourceFile in projectData.SourceFiles)
                {
                    // TODO: convert to var
                    foreach (ProjectConfiguration configuration in projectData.Configurations)
                    {
                        lock (sourceFile.FileConfigurations)
                        {
                            if (!sourceFile.FileConfigurations.Contains(configuration.Name))
                            {
                                var tool = new ProjectTool("VCCLCompilerTool");
                                var fileConfiguration = new ProjectFileConfiguration(configuration, tool, true);
                                sourceFile.FileConfigurations.Add(fileConfiguration);
                            }
                        }
                    }
                }
            }
        }

        private static string
        WorkaroundMSBuildBug(
            string key)
        {
            // http://connect.microsoft.com/VisualStudio/feedback/details/503935/msbuild-inconsistent-platform-for-any-cpu-between-solution-and-project
            return key.Replace("AnyCPU", "Any CPU");
        }

        public void
        Serialize()
        {
            // serialize each vcproj
            foreach (var project in this.ProjectDictionary)
            {
                project.Value.Serialize();
            }

            var solutionType = Bam.Core.State.Get("VSSolutionBuilder", "SolutionType") as System.Type;
            var SolutionInstance = System.Activator.CreateInstance(solutionType);

            // serialize the sln
            using (System.IO.TextWriter textWriter = new System.IO.StreamWriter(this.PathName))
            {
                var HeaderProperty = solutionType.GetProperty("Header");
                textWriter.Write(HeaderProperty.GetGetMethod().Invoke(SolutionInstance, null));

                var solutionLocationUri = new System.Uri(this.PathName, System.UriKind.RelativeOrAbsolute);

                var solutionFolders = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<System.Guid>>();
                var solutionFolderGuids = new System.Collections.Generic.Dictionary<string, System.Guid>();

                // projects
                foreach (var project in this.ProjectDictionary)
                {
                    var p = project.Value;
                    if (null != p.GroupName)
                    {
                        if (!solutionFolders.ContainsKey(p.GroupName))
                        {
                            solutionFolders.Add(p.GroupName, new System.Collections.Generic.List<System.Guid>());
                            solutionFolderGuids.Add(p.GroupName, new DeterministicGuid(p.GroupName).Guid);
                        }

                        solutionFolders[p.GroupName].Add(p.Guid);
                    }

                    var projectLocationUri = new System.Uri(project.Value.PathName, System.UriKind.RelativeOrAbsolute);
                    var relativeProjectLocationUri = solutionLocationUri.MakeRelativeUri(projectLocationUri);

                    var GuidProperty = solutionType.GetProperty("ProjectGuid");
                    var projectTypeGuid = (System.Guid)GuidProperty.GetGetMethod().Invoke(SolutionInstance, null);
                    textWriter.WriteLine("Project(\"{0}\") = \"{1}\", \"{2}\", \"{3}\"",
                                         projectTypeGuid.ToString("B").ToUpper(),
                                         project.Value.Name,
                                         relativeProjectLocationUri.ToString(),
                                         project.Value.Guid.ToString("B").ToUpper());

                    var ProjectExtensionProperty = solutionType.GetProperty("ProjectExtension");
                    var projectExtension = ProjectExtensionProperty.GetGetMethod().Invoke(SolutionInstance, null) as string;
                    if (projectExtension.Equals(".vcproj") && (project.Value.DependentProjects.Count > 0))
                    {
                        textWriter.WriteLine("\tProjectSection(ProjectDependencies) = postProject");
                        foreach (var dependentProject in project.Value.DependentProjects)
                        {
                            textWriter.WriteLine("\t\t{0} = {0}", dependentProject.Guid.ToString("B").ToUpper());
                        }
                        textWriter.WriteLine("\tEndProjectSection");
                    }

                    textWriter.WriteLine("EndProject");
                }

                var SolutionFolderGuidProperty = solutionType.GetProperty("SolutionFolderGuid");
                var solutionFolderTypeGuid = (System.Guid)SolutionFolderGuidProperty.GetGetMethod().Invoke(SolutionInstance, null);

                // solution folders
                if ((solutionFolders.Count > 0) && (0 != solutionFolderTypeGuid.CompareTo(System.Guid.Empty)))
                {
                    foreach (var folder in solutionFolders)
                    {
                        textWriter.WriteLine("Project(\"{0}\") = \"{1}\", \"{1}\", \"{2}\"",
                                             solutionFolderTypeGuid.ToString("B").ToUpper(), folder.Key, solutionFolderGuids[folder.Key].ToString("B").ToUpper());
                        textWriter.WriteLine("EndProject");
                    }
                }

                // global sections
                textWriter.WriteLine("Global");
                {
                    // solution configuration (presolution)
                    textWriter.WriteLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
                    foreach (var configuration in this.ProjectConfigurations)
                    {
                        var key = configuration.Key;
                        key = WorkaroundMSBuildBug(key);
                        textWriter.WriteLine("\t\t{0} = {1}", key, key); // TODO: fixme - should this be repeated?
                    }
                    textWriter.WriteLine("\tEndGlobalSection");

                    // project configuration platforms (post solution)
                    textWriter.WriteLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
                    foreach (var project in this.ProjectDictionary)
                    {
                        // TODO: fixme (WHY?)
                        var projectData = project.Value;

                        // TODO: change to var
                        foreach (ProjectConfiguration configurations in projectData.Configurations)
                        {
                            var configName = configurations.Name;
                            configName = WorkaroundMSBuildBug(configName);
                            // TODO: the second .Name should be from the solution configurations
                            textWriter.WriteLine("\t\t{0}.{1}.{2} = {3}", projectData.Guid.ToString("B").ToUpper(), configName, "ActiveCfg", configName);
                            textWriter.WriteLine("\t\t{0}.{1}.{2}.{3} = {4}", projectData.Guid.ToString("B").ToUpper(), configName, "Build", 0, configName);
                        }
                    }
                    textWriter.WriteLine("\tEndGlobalSection");

                    // solution properties (presolution)
                    textWriter.WriteLine("\tGlobalSection(SolutionProperties) = preSolution");
                    textWriter.WriteLine("\t\tHideSolutionNode = FALSE");
                    textWriter.WriteLine("\tEndGlobalSection");

                    // solution folders
                    if ((solutionFolders.Count > 0) && (0 != solutionFolderTypeGuid.CompareTo(System.Guid.Empty)))
                    {
                        textWriter.WriteLine("\tGlobalSection(NestedProjects) = preSolution");
                        foreach (var folder in solutionFolders)
                        {
                            foreach (var projectGuid in folder.Value)
                            {
                                textWriter.WriteLine("\t\t{0} = {1}", projectGuid.ToString("B").ToUpper(), solutionFolderGuids[folder.Key].ToString("B").ToUpper());
                            }
                        }
                        textWriter.WriteLine("\tEndGlobalSection");
                    }
                }
                textWriter.WriteLine("EndGlobal");
            }

            SolutionInstance = null;

#if false
            Bam.Core.Log.Info("Successfully created Visual Studio solution file for package '{0}'\n\t{1}", Bam.Core.State.PackageInfo[0].Name, this.PathName);
#endif
        }
    }
}