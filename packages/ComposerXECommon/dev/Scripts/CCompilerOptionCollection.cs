// <copyright file="CCompilerOptionCollection.cs" company="Mark Final">
//  Opus package
// </copyright>
// <summary>ComposerXECommon package</summary>
// <author>Mark Final</author>
namespace ComposerXECommon
{
    // Not sealed since the C++ compiler inherits from it
    public partial class CCompilerOptionCollection : C.CompilerOptionCollection, C.ICCompilerOptions, ICCompilerOptions
    {
        protected override void SetDefaultOptionValues(Opus.Core.DependencyNode node)
        {
            ICCompilerOptions compilerInterface = this as ICCompilerOptions;
            compilerInterface.AllWarnings = true;
            compilerInterface.StrictDiagnostics = true;
            compilerInterface.EnableRemarks = true;

            base.SetDefaultOptionValues(node);

            // there is too much of a headache with include paths to enable this!
            (this as C.ICCompilerOptions).IgnoreStandardIncludePaths = false;

            Opus.Core.Target target = node.Target;
            compilerInterface.SixtyFourBit = Opus.Core.OSUtilities.Is64Bit(target);

            if (target.HasConfiguration(Opus.Core.EConfiguration.Debug))
            {
                compilerInterface.StrictAliasing = false;
                compilerInterface.InlineFunctions = false;
            }
            else
            {
                compilerInterface.StrictAliasing = true;
                compilerInterface.InlineFunctions = true;
            }

            compilerInterface.PositionIndependentCode = false;

            C.ICompilerTool compilerTool = target.Toolset.Tool(typeof(C.ICompilerTool)) as C.ICompilerTool;
            (this as C.ICCompilerOptions).SystemIncludePaths.AddRange(compilerTool.IncludePaths((Opus.Core.BaseTarget)target));

            (this as C.ICCompilerOptions).TargetLanguage = C.ETargetLanguage.C;
        }

        public CCompilerOptionCollection(Opus.Core.DependencyNode node)
            : base(node)
        {
        }
    }
}
