// Automatically generated by Opus v0.50
namespace ProxyTest
{
    class ProxiedObjectFile : C.ObjectFile
    {
        public ProxiedObjectFile()
        {
            this.SourceFile.Include(this.PackageLocation, "main.c");

            // note that the proxy is set AFTER the Include call, as the filename expansion is deferred
            this.ProxyPath.Assign("..", "..", "FakePackage");
        }
    }

    class ProxiedObjectFileCollection : C.ObjectFileCollection
    {
        public ProxiedObjectFileCollection()
        {
            this.Include(this.PackageLocation, "main.c");

            // note that the proxy is set AFTER the Include call, as the filename expansion is deferred
            this.ProxyPath.Assign("..", "..", "FakePackage");
        }
    }

    class ProxiedWildcardObjectFileCollection : C.ObjectFileCollection
    {
        public ProxiedWildcardObjectFileCollection()
        {
            this.Include(this.PackageLocation, "*.c");

            // note that the proxy is set AFTER the Include call, as the filename expansion is deferred
            this.ProxyPath.Assign("..", "..", "FakePackage");
        }
    }
}
