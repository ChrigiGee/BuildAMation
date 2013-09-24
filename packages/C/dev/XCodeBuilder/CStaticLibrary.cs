// <copyright file="CStaticLibrary.cs" company="Mark Final">
//  Opus package
// </copyright>
// <summary>C package</summary>
// <author>Mark Final</author>
namespace XcodeBuilder
{
    public sealed partial class XcodeBuilder
    {
        public object Build(C.StaticLibrary moduleToBuild, out bool success)
        {
            var node = moduleToBuild.OwningNode;
            var moduleName = node.ModuleName;
            var target = node.Target;
            var baseTarget = (Opus.Core.BaseTarget)target;

            Opus.Core.Log.MessageAll("StaticLibrary {0}", moduleName);
            var options = moduleToBuild.Options as C.ArchiverOptionCollection;
            var outputPath = options.OutputPaths[C.OutputFileFlags.StaticLibrary];

            var fileRef = new PBXFileReference(moduleName, PBXFileReference.EType.StaticLibrary, outputPath, this.ProjectRootUri);
            this.Project.FileReferences.Add(fileRef);
            this.Project.ProductsGroup.Children.Add(fileRef);

            var data = new PBXNativeTarget(moduleName, PBXNativeTarget.EType.StaticLibrary);
            data.ProductReference = fileRef;
            this.Project.NativeTargets.Add(data);

            // build configuration target overrides to the project build configuration
            var buildConfiguration = this.Project.BuildConfigurations.Get(baseTarget.ConfigurationName('='), moduleName);
            var nativeTargetConfigurationList = this.Project.ConfigurationLists.Get(data);
            nativeTargetConfigurationList.AddUnique(buildConfiguration);
            if (null == data.BuildConfigurationList)
            {
                data.BuildConfigurationList = nativeTargetConfigurationList;
            }
            else
            {
                if (data.BuildConfigurationList != nativeTargetConfigurationList)
                {
                    throw new Opus.Core.Exception("Inconsistent build configuration lists");
                }
            }

            // fill out the build configuration
            XcodeProjectProcessor.ToXcodeProject.Execute(moduleToBuild.Options, this.Project, data, buildConfiguration, target);
            Opus.Core.Log.MessageAll("Options");
            foreach (var o in buildConfiguration.Options)
            {
                Opus.Core.Log.MessageAll("  {0} {1}", o.Key, o.Value);
            }

            // adding the group for the target
            var group = new PBXGroup(moduleName);
            group.SourceTree = "<group>";
            group.Path = moduleName;
            foreach (var source in node.Children)
            {
                if (source.Module is Opus.Core.IModuleCollection)
                {
                    Opus.Core.Log.MessageAll("Collective source; {0}", source.UniqueModuleName);
                    foreach (var source2 in source.Children)
                    {
                        Opus.Core.Log.MessageAll("\tsource; {0}", source2.UniqueModuleName);
                        var sourceData = source2.Data as PBXBuildFile;
                        Opus.Core.Log.MessageAll("\t{0}", sourceData.Name);
                        group.Children.Add(sourceData.FileReference);
                    }
                }
                else
                {
                    Opus.Core.Log.MessageAll("source; {0}", source.UniqueModuleName);
                    var sourceData = source.Data as PBXBuildFile;
                    group.Children.Add(sourceData.FileReference);
                }
            }
            this.Project.Groups.Add(group);
            this.Project.MainGroup.Children.Add(group);

            var sourcesBuildPhase = this.Project.SourceBuildPhases.Get("Sources", moduleName);
            data.BuildPhases.Add(sourcesBuildPhase);

            var copyFilesBuildPhase = this.Project.CopyFilesBuildPhases.Get("CopyFiles", moduleName);
            data.BuildPhases.Add(copyFilesBuildPhase);

            var frameworksBuildPhase = this.Project.FrameworksBuildPhases.Get("Frameworks", moduleName);
            data.BuildPhases.Add(frameworksBuildPhase);

            if (null != node.RequiredDependents)
            {
                // no link dependency
                foreach (var dependency in node.RequiredDependents)
                {
                    var dependentData = dependency.Data as PBXNativeTarget;
                    if (null == dependentData)
                    {
                        continue;
                    }

                    var targetDependency = new PBXTargetDependency(moduleName, dependentData);
                    this.Project.TargetDependencies.Add(targetDependency);

                    var containerItemProxy = new PBXContainerItemProxy(moduleName, dependentData, this.Project);
                    this.Project.ContainerItemProxies.Add(containerItemProxy);
                    targetDependency.TargetProxy = containerItemProxy;

                    data.Dependencies.Add(targetDependency);
                }
            }

            // find header files
            var fieldBindingFlags = System.Reflection.BindingFlags.Instance |
                                        System.Reflection.BindingFlags.Public |
                                            System.Reflection.BindingFlags.NonPublic;
            var fields = moduleToBuild.GetType().GetFields(fieldBindingFlags);
            foreach (var field in fields)
            {
                var headerFileAttributes = field.GetCustomAttributes(typeof(C.HeaderFilesAttribute), false);
                if (headerFileAttributes.Length > 0)
                {
                    var headerFileCollection = field.GetValue(moduleToBuild) as Opus.Core.FileCollection;
                    foreach (string headerPath in headerFileCollection)
                    {
                        var headerFileRef = new PBXFileReference(moduleName, PBXFileReference.EType.HeaderFile, headerPath, this.ProjectRootUri);
                        this.Project.FileReferences.Add(headerFileRef);
                        group.Children.Add(headerFileRef);
                    }
                }
            }

            success = true;
            return data;
        }
    }
}
