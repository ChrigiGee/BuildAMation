// Automatically generated by Opus v0.20
namespace WPFTest
{
    // Define module classes here
    class WPFExecutable : CSharp.WindowsExecutable
    {
        public WPFExecutable()
        {
            this.applicationDefinitions.AddRelativePaths(this, "source", "*.xaml");

            this.UpdateOptions += new Opus.Core.UpdateOptionCollectionDelegate(WPFExecutable_UpdateOptions);
        }

        void WPFExecutable_UpdateOptions(Opus.Core.IModule module, Opus.Core.Target target)
        {
            CSharp.IOptions options = module.Options as CSharp.IOptions;
            string referenceAssemblyDir = DotNetFramework.DotNet.ToolsPath;
            string wpfDir = System.IO.Path.Combine(referenceAssemblyDir, "WPF");

            options.References.Add(System.IO.Path.Combine(referenceAssemblyDir, "System.Xaml.dll"));
            options.References.Add(System.IO.Path.Combine(wpfDir, "PresentationCore.dll"));
            options.References.Add(System.IO.Path.Combine(wpfDir, "PresentationFramework.dll"));
            options.References.Add(System.IO.Path.Combine(wpfDir, "WindowsBase.dll"));
        }

        [CSharp.ApplicationDefinitions]
        Opus.Core.FileCollection applicationDefinitions = new Opus.Core.FileCollection();
    }
}
