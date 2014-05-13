// <copyright file="CObjectFileCollection.cs" company="Mark Final">
//  Opus package
// </copyright>
// <summary>C package</summary>
// <author>Mark Final</author>
namespace MakeFileBuilder
{
    public sealed partial class MakeFileBuilder
    {
        public object Build(C.ObjectFileCollectionBase moduleToBuild, out bool success)
        {
            var objectFileCollectionModule = moduleToBuild as Opus.Core.BaseModule;
            var node = objectFileCollectionModule.OwningNode;

            var dependents = new MakeFileVariableDictionary();
            var childDataArray = new Opus.Core.Array<MakeFileData>();
            foreach (var childNode in node.Children)
            {
                var data = childNode.Data as MakeFileData;
                if (!data.VariableDictionary.ContainsKey(C.ObjectFile.ObjectFileLocationKey))
                {
                    throw new Opus.Core.Exception("MakeFile Variable for '{0}' is missing", childNode.UniqueModuleName);
                }

                childDataArray.Add(data);
                dependents.Add(C.ObjectFile.ObjectFileLocationKey, data.VariableDictionary[C.ObjectFile.ObjectFileLocationKey]);
            }
            if (null != node.ExternalDependents)
            {
                foreach (var dependentNode in node.ExternalDependents)
                {
                    if (null == dependentNode.Data)
                    {
                        continue;
                    }
                    var data = dependentNode.Data as MakeFileData;
                    foreach (var makeVariable in data.VariableDictionary)
                    {
                        dependents.Add(makeVariable.Key, makeVariable.Value);
                    }
                }
            }

            var makeFilePath = MakeFileBuilder.GetMakeFilePathName(node);
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(makeFilePath));

            var makeFile = new MakeFile(node, this.topLevelMakeFilePath);

            // no output paths because this rule has no recipe
            var rule = new MakeFileRule(moduleToBuild, C.ObjectFile.ObjectFileLocationKey, node.UniqueModuleName, null, dependents, null, null);
            if (null == node.Parent)
            {
                // phony target
                rule.TargetIsPhony = true;
            }
            makeFile.RuleArray.Add(rule);

            using (var makeFileWriter = new System.IO.StreamWriter(makeFilePath))
            {
                makeFile.Write(makeFileWriter);
            }

            var returnData = new MakeFileData(makeFilePath, makeFile.ExportedTargets, makeFile.ExportedVariables, null);
            success = true;
            return returnData;
        }
    }
}