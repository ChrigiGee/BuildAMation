// <copyright file="CxxObjectFileCollection.cs" company="Mark Final">
//  Opus package
// </copyright>
// <summary>C package</summary>
// <author>Mark Final</author>
namespace C.Cxx
{
    /// <summary>
    /// C++ object file collection
    /// </summary>
    [Opus.Core.ModuleToolAssignment(typeof(ICxxCompilerTool))]
    public class ObjectFileCollection : ObjectFileCollectionBase
    {
        public void Add(ObjectFile objectFile)
        {
            this.list.Add(objectFile);
        }

        protected override System.Collections.Generic.List<Opus.Core.IModule> MakeChildModules(Opus.Core.LocationArray locationList)
        {
            var objectFileList = new System.Collections.Generic.List<Opus.Core.IModule>();

            foreach (var location in locationList)
            {
                var objectFile = new ObjectFile();
                objectFile.SourceFileLocation = location;
                objectFileList.Add(objectFile);
            }

            return objectFileList;
        }
    }
}