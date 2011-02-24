// <copyright file="MocFile.cs" company="Mark Final">
//  Opus package
// </copyright>
// <summary>Qt package</summary>
// <author>Mark Final</author>
namespace MakeFileBuilder
{
    public partial class MakeFileBuilder
    {
        public object Build(Qt.MocFile mocFile, Opus.Core.DependencyNode node, out System.Boolean success)
        {
            Opus.Core.Target target = node.Target;
            Qt.MocTool tool = new Qt.MocTool();
            Qt.MocOptionCollection toolOptions = mocFile.Options as Qt.MocOptionCollection;
            string toolExePath = tool.Executable(target);

            string sourceFilePath = mocFile.SourceFile.AbsolutePath;
            if (!System.IO.File.Exists(sourceFilePath))
            {
                throw new Opus.Core.Exception(System.String.Format("Moc source file '{0}' does not exist", sourceFilePath));
            }

            Opus.Core.StringArray inputFiles = new Opus.Core.StringArray();
            inputFiles.Add(sourceFilePath);

            Opus.Core.StringArray outputFiles = new Opus.Core.StringArray();
            node.FilterOutputPaths(Qt.OutputFileFlags.MocGeneratedSourceFile, outputFiles);

            System.Text.StringBuilder commandLineBuilder = new System.Text.StringBuilder();
            Opus.Core.DirectoryCollection directoriesToCreate = null;
            if (toolOptions is CommandLineProcessor.ICommandLineSupport)
            {
                CommandLineProcessor.ICommandLineSupport commandLineOption = toolOptions as CommandLineProcessor.ICommandLineSupport;
                commandLineOption.ToCommandLineArguments(commandLineBuilder, target);

                directoriesToCreate = commandLineOption.DirectoriesToCreate();
            }
            else
            {
                throw new Opus.Core.Exception("Moc options does not support command line translation");
            }

            commandLineBuilder.AppendFormat("\"{0}\"", sourceFilePath);

            Opus.Core.StringArray recipes = new Opus.Core.StringArray();
            recipes.Add("\"" + toolExePath + "\" " + commandLineBuilder.ToString());

            string makeFilePath = MakeFileBuilder.GetMakeFilePathName(node);
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(makeFilePath));
            Opus.Core.Log.DebugMessage("Makefile : '{0}'", makeFilePath);

#if true
            MakeFile makeFile = new MakeFile(node, this.topLevelMakeFilePath);

            MakeFileRule rule = new MakeFileRule(mocFile.Options.OutputPaths, Qt.OutputFileFlags.MocGeneratedSourceFile, node.UniqueModuleName, directoriesToCreate, inputFiles, recipes);
            makeFile.RuleArray.Add(rule);

            using (System.IO.TextWriter makeFileWriter = new System.IO.StreamWriter(makeFilePath))
            {
                makeFile.Write(makeFileWriter);
            }

            success = true;

            MakeFileData returnData = new MakeFileData(makeFilePath, makeFile.ExportedTargets, makeFile.ExportedVariables, tool.EnvironmentPaths(target));
            return returnData;
#else
            MakeFileBuilderRecipe recipe = new MakeFileBuilderRecipe(node, inputFiles, null, commandLines, this.topLevelMakeFilePath);

            string makeFileTargetName = null;
            string makeFileVariableName = null;
            using (System.IO.TextWriter makeFileWriter = new System.IO.StreamWriter(makeFile))
            {
                recipe.Write(makeFileWriter, Qt.MocOutputPathFlag.MocGeneratedSourceFile);
                makeFileTargetName = recipe.TargetName;
                makeFileVariableName = recipe.VariableName;
            }

            success = true;

            MakeFileData returnData = new MakeFileData(makeFilePath, makeFileTargetName, makeFileVariableName, tool.EnvironmentPaths(target));
            return returnData;
#endif
        }
    }
}