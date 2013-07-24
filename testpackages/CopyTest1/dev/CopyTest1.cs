// Automatically generated by Opus v0.50
namespace CopyTest1
{
    class CopySingleFileTest : FileUtilities.CopyFile
    {
        public CopySingleFileTest()
        {
            var dataDir = this.Locations["PackageDir"].ChildDirectory("data");
            this.Include(dataDir, "testfile.txt");
            this.UpdateOptions += delegate(Opus.Core.IModule module, Opus.Core.Target target) {
                var options = module.Options as FileUtilities.ICopyFileOptions;
                if (null != options)
                {
                    options.DestinationDirectory = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "CopyTest");
                }
           };
        }
    }

    class CopyMultipleFileTest : FileUtilities.CopyFileCollection
    {
        public CopyMultipleFileTest()
        {
            var dataDir = this.Locations["PackageDir"].ChildDirectory("data");
            this.Include(dataDir, "*");
        }
    }

    class CopyDirectoryTest : FileUtilities.CopyDirectory
    {
        public CopyDirectoryTest(Opus.Core.Target target)
        {
            this.Include(this.Locations["PackageDir"], target, "data");
        }
    }
}
