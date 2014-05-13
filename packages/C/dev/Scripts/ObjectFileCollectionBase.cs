// <copyright file="ObjectFileCollectionBase.cs" company="Mark Final">
//  Opus package
// </copyright>
// <summary>C package</summary>
// <author>Mark Final</author>
namespace C
{
    public abstract class ObjectFileCollectionBase : Opus.Core.BaseModule, Opus.Core.IModuleCollection
    {
        protected Opus.Core.Array<ObjectFile> list = new Opus.Core.Array<ObjectFile>();

        protected Opus.Core.LocationArray Includes
        {
            get;
            set;
        }

        protected Opus.Core.LocationArray Excludes
        {
            get;
            set;
        }

        public void Include(Opus.Core.Location baseLocation, string pattern)
        {
            if (null == this.Includes)
            {
                this.Includes = new Opus.Core.LocationArray();
            }
            this.Includes.Add(new Opus.Core.ScaffoldLocation(baseLocation, pattern, Opus.Core.ScaffoldLocation.ETypeHint.File));
        }

        public void Exclude(Opus.Core.Location baseLocation, string pattern)
        {
            if (null == this.Excludes)
            {
                this.Excludes = new Opus.Core.LocationArray();
            }
            this.Excludes.Add(new Opus.Core.ScaffoldLocation(baseLocation, pattern, Opus.Core.ScaffoldLocation.ETypeHint.File));
        }

        private Opus.Core.LocationArray EvaluatePaths()
        {
            if (null == this.Includes)
            {
                return null;
            }

            var includePathList = new Opus.Core.LocationArray();
            foreach (var include in this.Includes)
            {
                includePathList.AddRangeUnique(include.GetLocations());
            }
            if (null == this.Excludes)
            {
                return includePathList;
            }

            var excludePathList = new Opus.Core.LocationArray();
            foreach (var exclude in this.Excludes)
            {
                excludePathList.AddRangeUnique(exclude.GetLocations());
            }

            var complement = includePathList.Complement(excludePathList) as Opus.Core.LocationArray;
            return complement;
        }

        protected virtual System.Collections.Generic.List<Opus.Core.IModule> MakeChildModules(Opus.Core.LocationArray locationList)
        {
            throw new Opus.Core.Exception("Derived classes should implement this function");
        }

        Opus.Core.ModuleCollection Opus.Core.INestedDependents.GetNestedDependents(Opus.Core.Target target)
        {
            var collection = new Opus.Core.ModuleCollection();

            // add in modules that were inserted other than by an Include() call
            foreach (var module in this.list)
            {
                collection.Add(module);
            }

            var locationList = this.EvaluatePaths();
            if (null == locationList)
            {
                return collection;
            }

            var childModules = this.MakeChildModules(locationList);
            if (null != this.DeferredUpdates)
            {
                foreach (var objectFile in childModules)
                {
                    var objectFileDeferredLocation = (objectFile as ObjectFile).SourceFileLocation;
                    if (this.DeferredUpdates.ContainsKey(objectFileDeferredLocation))
                    {
                        foreach (var updateDelegate in this.DeferredUpdates[objectFileDeferredLocation])
                        {
                            objectFile.UpdateOptions += updateDelegate;
                        }
                    }

                    collection.Add(objectFile);
                }
            }
            else
            {
                foreach (var objectFile in childModules)
                {
                    collection.Add(objectFile as Opus.Core.IModule);
                }
            }
            return collection;
        }

        private System.Collections.Generic.Dictionary<Opus.Core.Location, Opus.Core.UpdateOptionCollectionDelegateArray> DeferredUpdates
        {
            get;
            set;
        }

        public void RegisterUpdateOptions(Opus.Core.UpdateOptionCollectionDelegateArray delegateArray,
                                          Opus.Core.Location baseLocation,
                                          string pattern)
        {
            if (null == this.DeferredUpdates)
            {
                this.DeferredUpdates = new System.Collections.Generic.Dictionary<Opus.Core.Location, Opus.Core.UpdateOptionCollectionDelegateArray>(new Opus.Core.LocationComparer());
            }

            this.DeferredUpdates[new Opus.Core.ScaffoldLocation(baseLocation, pattern, Opus.Core.ScaffoldLocation.ETypeHint.File)] = delegateArray;
        }
    }
}