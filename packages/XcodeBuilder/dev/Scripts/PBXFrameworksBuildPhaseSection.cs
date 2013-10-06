// <copyright file="PBXFrameworksBuildPhaseSection.cs" company="Mark Final">
//  Opus package
// </copyright>
// <summary>XcodeBuilder package</summary>
// <author>Mark Final</author>
namespace XcodeBuilder
{
    public sealed class PBXFrameworksBuildPhaseSection : IWriteableNode, System.Collections.IEnumerable
    {
        public PBXFrameworksBuildPhaseSection()
        {
            this.FrameworksBuildPhases = new System.Collections.Generic.List<PBXFrameworksBuildPhase>();
        }

        public void Add(PBXFrameworksBuildPhase buildPhase)
        {
            lock (this.FrameworksBuildPhases)
            {
                this.FrameworksBuildPhases.Add(buildPhase);
            }
        }

        public PBXFrameworksBuildPhase Get(string name, string moduleName)
        {
            lock (this.FrameworksBuildPhases)
            {
                foreach (var buildPhase in this.FrameworksBuildPhases)
                {
                    if ((buildPhase.Name == name) && (buildPhase.ModuleName == moduleName))
                    {
                        return buildPhase;
                    }
                }

                var newBuildPhase = new PBXFrameworksBuildPhase(name, moduleName);
                this.FrameworksBuildPhases.Add(newBuildPhase);
                return newBuildPhase;
            }
        }

        private System.Collections.Generic.List<PBXFrameworksBuildPhase> FrameworksBuildPhases
        {
            get;
            set;
        }

#region IWriteableNode implementation
        void IWriteableNode.Write (System.IO.TextWriter writer)
        {
            if (this.FrameworksBuildPhases.Count == 0)
            {
                return;
            }

            var orderedList = new System.Collections.Generic.List<PBXFrameworksBuildPhase>(this.FrameworksBuildPhases);
            orderedList.Sort(
                delegate(PBXFrameworksBuildPhase p1, PBXFrameworksBuildPhase p2)
                {
                    return p1.UUID.CompareTo(p2.UUID);
                }
            );

            writer.WriteLine("");
            writer.WriteLine("/* Begin PBXFrameworksBuildPhase section */");
            foreach (var buildPhase in orderedList)
            {
                (buildPhase as IWriteableNode).Write(writer);
            }
            writer.WriteLine("/* End PBXFrameworksBuildPhase section */");
        }
#endregion

#region IEnumerable implementation

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return this.FrameworksBuildPhases.GetEnumerator();
        }

#endregion
    }
}
