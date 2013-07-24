// Automatically generated by Opus v0.20
namespace WPFTest
{
    // Define module classes here
    class WPFExecutable : CSharp.WindowsExecutable
    {
        public WPFExecutable()
        {
            var sourceDir = this.Locations["PackageDir"].ChildDirectory("source");
            this.applicationDefinition.Include(sourceDir, "App.xaml");
            this.page.Include(sourceDir, "MainWindow.xaml");

            this.UpdateOptions += new Opus.Core.UpdateOptionCollectionDelegate(WPFExecutable_UpdateOptions);
        }

        void WPFExecutable_UpdateOptions(Opus.Core.IModule module, Opus.Core.Target target)
        {
            var options = module.Options as CSharp.IOptions;
            options.References.Add("System.dll");
            options.References.Add("System.Data.dll");
            options.References.Add("System.Xaml.dll");
            options.References.Add("WindowsBase.dll");
            options.References.Add("PresentationCore.dll");
            options.References.Add("PresentationFramework.dll");
        }

        [CSharp.ApplicationDefinition]
        Opus.Core.File applicationDefinition = new Opus.Core.File();

        [CSharp.Pages]
        Opus.Core.File page = new Opus.Core.File();
    }
}
