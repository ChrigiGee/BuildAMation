// <copyright file="CObjectFileCollection.cs" company="Mark Final">
//  Opus package
// </copyright>
// <summary>C package</summary>
// <author>Mark Final</author>
namespace VSSolutionBuilder
{
    public sealed partial class VSSolutionBuilder
    {
        public object Build(C.ObjectFileCollectionBase objectFileCollection, out bool success)
        {
            Opus.Core.DependencyNode node = objectFileCollection.OwningNode;
            if (null == node.Parent || (node.Parent.Module.GetType().BaseType.BaseType == typeof(C.ObjectFileCollection) && null == node.Parent.Parent))
            {
                // utility project
                success = true;
                return null;
            }

            Opus.Core.Target target = node.Target;
            string moduleName = node.ModuleName;

            IProject projectData = null;
            // TODO: want to remove this
            lock (this.solutionFile.ProjectDictionary)
            {
                if (this.solutionFile.ProjectDictionary.ContainsKey(moduleName))
                {
                    projectData = this.solutionFile.ProjectDictionary[moduleName];
                }
                else
                {
                    System.Type solutionType = Opus.Core.State.Get("VSSolutionBuilder", "SolutionType") as System.Type;
                    object SolutionInstance = System.Activator.CreateInstance(solutionType);
                    System.Reflection.PropertyInfo ProjectExtensionProperty = solutionType.GetProperty("ProjectExtension");
                    string projectExtension = ProjectExtensionProperty.GetGetMethod().Invoke(SolutionInstance, null) as string;

                    string projectPathName = System.IO.Path.Combine(node.GetModuleBuildDirectory(), moduleName);
                    projectPathName += projectExtension;

                    System.Type projectType = VSSolutionBuilder.GetProjectClassType();
                    projectData = System.Activator.CreateInstance(projectType, new object[] { moduleName, projectPathName, node.Package.Identifier, objectFileCollection.ProxyPath }) as IProject;

                    this.solutionFile.ProjectDictionary.Add(moduleName, projectData);
                }
            }

            {
                string platformName = VSSolutionBuilder.GetPlatformNameFromTarget(target);
                if (!projectData.Platforms.Contains(platformName))
                {
                    projectData.Platforms.Add(platformName);
                }
            }

            string configurationName = VSSolutionBuilder.GetConfigurationNameFromTarget(target);

            // TODO: want to remove this
            lock (this.solutionFile.ProjectConfigurations)
            {
                if (!this.solutionFile.ProjectConfigurations.ContainsKey(configurationName))
                {
                    this.solutionFile.ProjectConfigurations.Add(configurationName, new System.Collections.Generic.List<IProject>());
                }
            }
            this.solutionFile.ProjectConfigurations[configurationName].Add(projectData);

            ProjectConfiguration configuration;
            lock (projectData.Configurations)
            {
                if (!projectData.Configurations.Contains(configurationName))
                {
                    C.ICCompilerOptions compilerOptions = objectFileCollection.Options as C.ICCompilerOptions;
                    C.IToolchainOptions toolchainOptions = compilerOptions.ToolchainOptionCollection as C.IToolchainOptions;
                    EProjectCharacterSet characterSet;
                    switch (toolchainOptions.CharacterSet)
                    {
                        case C.ECharacterSet.NotSet:
                            characterSet = EProjectCharacterSet.NotSet;
                            break;

                        case C.ECharacterSet.Unicode:
                            characterSet = EProjectCharacterSet.UniCode;
                            break;

                        case C.ECharacterSet.MultiByte:
                            characterSet = EProjectCharacterSet.MultiByte;
                            break;

                        default:
                            characterSet = EProjectCharacterSet.Undefined;
                            break;
                    }
                    configuration = new ProjectConfiguration(configurationName, projectData);
                    configuration.CharacterSet = characterSet;

                    projectData.Configurations.Add(target, configuration);
                }
                else
                {
                    configuration = projectData.Configurations[configurationName];
                    configuration.CharacterSet = (EProjectCharacterSet)((objectFileCollection.Options as C.ICCompilerOptions).ToolchainOptionCollection as C.IToolchainOptions).CharacterSet;
                    projectData.Configurations.AddExistingForTarget(target, configuration);
                }

                C.CompilerOptionCollection options = objectFileCollection.Options as C.CompilerOptionCollection;
                configuration.IntermediateDirectory = options.OutputDirectoryPath;
            }

            string toolName = "VCCLCompilerTool";
            ProjectTool vcCLCompilerTool = configuration.GetTool(toolName);
            if (null == vcCLCompilerTool)
            {
                vcCLCompilerTool = new ProjectTool(toolName);
                configuration.AddToolIfMissing(vcCLCompilerTool);

                if (objectFileCollection.Options is VisualStudioProcessor.IVisualStudioSupport)
                {
                    VisualStudioProcessor.IVisualStudioSupport visualStudioProjectOption = objectFileCollection.Options as VisualStudioProcessor.IVisualStudioSupport;
                    VisualStudioProcessor.ToolAttributeDictionary settingsDictionary = visualStudioProjectOption.ToVisualStudioProjectAttributes(target);

                    foreach (System.Collections.Generic.KeyValuePair<string, string> setting in settingsDictionary)
                    {
                        vcCLCompilerTool[setting.Key] = setting.Value;
                    }
                }
                else
                {
                    throw new Opus.Core.Exception("Compiler options does not support VisualStudio project translation");
                }
            }

            success = true;
            return projectData;
        }
    }
}