// <copyright file="PackageIdentifier.cs" company="Mark Final">
//  Opus
// </copyright>
// <summary>Opus Core</summary>
// <author>Mark Final</author>
namespace Opus.Core
{
    public class PackageIdentifier : System.IComparable
    {
        public PackageIdentifier(string name, string version)
        {
            if (null == name)
            {
                throw new Exception("Invalid name");
            }
            if (null == version)
            {
                throw new Exception("Invalid version");
            }

            this.Name = name;
            this.Version = version;
            this.Root = this.LocateRoot();
            this.PlatformFilter = EPlatform.All;
            this.Location = DirectoryLocation.Get(this.Path);
        }

        public string Name
        {
            get;
            private set;
        }

        public string Version
        {
            get;
            private set;
        }

        public DirectoryLocation Root
        {
            get;
            private set;
        }

        public EPlatform PlatformFilter
        {
            get;
            set;
        }

        public string DefinitionPathName
        {
            get
            {
                var scriptFile = System.IO.Path.Combine(this.Path, this.Name + ".xml");
                return scriptFile;
            }
        }

        public PackageDefinitionFile Definition
        {
            get;
            set;
        }

        public string ScriptPathName
        {
            get
            {
                var scriptFile = System.IO.Path.Combine(this.Path, this.Name + ".cs");
                return scriptFile;
            }
        }

        public string CompilationDefinition
        {
            get
            {
                var compilationDefine = System.String.Format("OPUSPACKAGE_{0}_{1}", this.Name, this.Version.Replace('.', '_').Replace('-', '_')).ToUpper();
                return compilationDefine;
            }
        }

        public bool MatchName(PackageIdentifier identifier, bool ignoreCase)
        {
            return this.MatchName(identifier.Name, ignoreCase);
        }

        public bool MatchName(string name, bool ignoreCase)
        {
            var match = (0 == System.String.Compare(this.Name, name, ignoreCase));
            return match;
        }

        public int MatchVersion(PackageIdentifier identifier, bool ignoreCase)
        {
            return this.MatchVersion(identifier.Version, ignoreCase);
        }

        public int MatchVersion(string version, bool ignoreCase)
        {
            int compare = System.String.Compare(this.Version, version, ignoreCase);
            return compare;
        }

        public bool Match(PackageIdentifier identifier, bool ignoreCase)
        {
            var match = this.MatchName(identifier, ignoreCase) && (0 == this.MatchVersion(identifier, ignoreCase));
            return match;
        }

        public bool Match(string name, string version, bool ignoreCase)
        {
            var match = this.MatchName(name, ignoreCase) && (0 == this.MatchVersion(version, ignoreCase));
            return match;
        }

        public bool ConvertVersionToDouble(out double version)
        {
            return double.TryParse(this.Version, out version);
        }

        public string ToString(string separator)
        {
            var identifierString = System.String.Format("{0}{1}{2}", this.Name, separator, this.Version);
            return identifierString;
        }

        public override string ToString()
        {
            return this.ToString("-");
        }

        public DirectoryLocation Location
        {
            get;
            private set;
        }

        public string Path
        {
            get
            {
                var rootedPath = System.IO.Path.Combine(this.Root.AbsolutePath, this.Name);
                rootedPath = System.IO.Path.Combine(rootedPath, this.Version);
                return rootedPath;
            }
        }

        int System.IComparable.CompareTo(object obj)
        {
            var objAs = obj as PackageIdentifier;
            string left = this.ToString("-");
            string right = objAs.ToString("-");
            int compared = left.CompareTo(right);
            return compared;
        }

        private DirectoryLocation LocateRoot()
        {
            var rootContainingPackage = false;
            foreach (var root in State.PackageRoots)
            {
                var packageDirectory = System.IO.Path.Combine(root.AbsolutePath, this.Name);
                if (System.IO.Directory.Exists(packageDirectory))
                {
                    rootContainingPackage = true;
                }

                var versionDirectory = System.IO.Path.Combine(packageDirectory, this.Version);
                if (System.IO.Directory.Exists(versionDirectory))
                {
                    return root;
                }
            }

            if (!rootContainingPackage)
            {
                var message = new System.Text.StringBuilder();
                message.AppendFormat("Unable to locate package '{0}' in any registered package roots:\n", this.ToString("-"));
                foreach (var root in State.PackageRoots)
                {
                    message.AppendFormat("{0}\n", root);
                }
                throw new Exception(message.ToString());
            }
            else
            {
                return null;
            }
        }

        public bool IsDefaultVersion
        {
            set;
            get;
        }
    }
}