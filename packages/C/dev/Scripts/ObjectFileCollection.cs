// <copyright file="ObjectFileCollection.cs" company="Mark Final">
//  Opus package
// </copyright>
// <summary>C package</summary>
// <author>Mark Final</author>
namespace C
{
    /// <summary>
    /// C object file collection
    /// </summary>
    [Opus.Core.ModuleToolAssignment(typeof(ICompilerTool))]
    public class ObjectFileCollection : ObjectFileCollectionBase
    {
        public void Add(ObjectFile objectFile)
        {
            this.list.Add(objectFile);
        }

        protected override System.Collections.Generic.List<Opus.Core.IModule> MakeChildModules(Opus.Core.Array<Opus.Core.Location> locationList)
        {
            var objectFileList = new System.Collections.Generic.List<Opus.Core.IModule>();

            foreach (var location in locationList)
            {
                var objectFile = new ObjectFile();
                // TODO: the proxypath should have already been taken into account by now?
                objectFile.ProxyPath.Assign(this.ProxyPath);
                objectFile.SourceFile.AbsoluteLocation = location;
                objectFileList.Add(objectFile);
            }

            return objectFileList;
        }

#if false
        // deprecated
        public void Include(object owner, params string[] pathSegments)
        {
            var package = Opus.Core.PackageUtilities.GetOwningPackage(owner);
            if (null == package)
            {
                throw new Opus.Core.Exception("Unable to locate package '{0}'", owner.GetType().Namespace);
            }

            var packagePath = package.Identifier.Path;
            var proxyPath = (owner as Opus.Core.BaseModule).ProxyPath;
            if (null != proxyPath)
            {
                packagePath = proxyPath.Combine(package.Identifier.Location).CachedPath;
            }

            var filePaths = Opus.Core.File.GetFiles(packagePath, pathSegments);
            foreach (var path in filePaths)
            {
                var objectFile = new ObjectFile();
                objectFile.ProxyPath.Assign(this.ProxyPath);
                objectFile.SourceFile.AbsolutePath = path;
                this.list.Add(objectFile);
            }
        }

        // deprecated
        public void Exclude(object owner, params string[] pathSegments)
        {
            var package = Opus.Core.PackageUtilities.GetOwningPackage(owner);
            if (null == package)
            {
                throw new Opus.Core.Exception("Unable to locate package '{0}'", owner.GetType().Namespace);
            }

            var packagePath = package.Identifier.Path;
            var proxyPath = (owner as Opus.Core.BaseModule).ProxyPath;
            if (null != proxyPath)
            {
                packagePath = proxyPath.Combine(package.Identifier.Location).CachedPath;
            }

            var filePaths = Opus.Core.File.GetFiles(packagePath, pathSegments);
            var toRemove = new System.Collections.Generic.List<ObjectFile>();
            foreach (var path in filePaths)
            {
                foreach (var file in this.list)
                {
                    if (file.SourceFile.AbsolutePath == path)
                    {
                        toRemove.Add(file);
                    }
                }
            }

            foreach (var file in toRemove)
            {
                this.list.Remove(file);
            }
        }
#endif
    }
}