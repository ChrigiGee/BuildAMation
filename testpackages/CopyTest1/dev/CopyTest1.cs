// Automatically generated by Opus v0.50
namespace CopyTest1
{
    class CopySingleFileTest : FileUtilities.CopyFile
    {
        public CopySingleFileTest()
        {
            this.SetRelativePath(this, "data", "testfile.txt");
            this.UpdateOptions += delegate(Opus.Core.IModule module, Opus.Core.Target target) {
           };
        }
    }
}
