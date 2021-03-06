#region License
// Copyright (c) 2010-2016, Mark Final
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
//
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// * Neither the name of BuildAMation nor the names of its
//   contributors may be used to endorse or promote products derived from
//   this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion // License
using System.Linq;
namespace Bam.Core
{
    /// <summary>
    /// Abstract concept of a module, the base class for all buildables in BAM. A hierarchy of classes in modules
    /// allows all modules to share similar features, and build specifics with each sub-class.
    /// </summary>
    public abstract class Module :
        IModuleExecution
    {
        /// <summary>
        /// Static cache of all modules created.
        /// </summary>
        static protected System.Collections.Generic.List<Module> AllModules = new System.Collections.Generic.List<Module>();

        /// <summary>
        /// Protected constructor (use Init function in general use to configure a module) for a new module.
        /// This defines the standard macros for all modules:
        /// 'packagedir'
        /// 'packagename'
        /// 'packagebuilddir'
        /// 'modulename'
        /// 'OutputName'
        /// 'config'
        /// </summary>
        // private so that the factory method must be used
        protected Module()
        {
            var graph = Graph.Instance;
            if (null == graph.BuildEnvironment)
            {
                throw new Exception("No build environment for module {0}", this.GetType().ToString());
            }

            graph.AddModule(this);
            this.Macros = new MacroList();
            // TODO: Can this be generalized to be a collection of files?
            this.GeneratedPaths = new System.Collections.Generic.Dictionary<PathKey, TokenizedString>();

            // add the package root
            var packageNameSpace = graph.CommonModuleType.Peek().Namespace;
            var packageDefinition = graph.Packages.Where(item => item.Name == packageNameSpace).FirstOrDefault();
            if (null == packageDefinition)
            {
                var includeTests = CommandLineProcessor.Evaluate(new Options.UseTests());
                if (includeTests && packageNameSpace.EndsWith(".tests"))
                {
                    packageNameSpace = packageNameSpace.Replace(".tests", string.Empty);
                    packageDefinition = graph.Packages.Where(item => item.Name == packageNameSpace).FirstOrDefault();
                }

                if (null == packageDefinition)
                {
                    throw new Exception("Unable to locate package for namespace '{0}'", packageNameSpace);
                }
            }
            this.PackageDefinition = packageDefinition;
            this.Macros.AddVerbatim("packagedir", packageDefinition.GetPackageDirectory());
            this.Macros.AddVerbatim("packagename", packageDefinition.Name);
            this.Macros.AddVerbatim("packagebuilddir", packageDefinition.GetBuildDirectory());
            this.Macros.AddVerbatim("modulename", this.GetType().Name);
            this.Macros.Add("OutputName", this.Macros["modulename"]);

            this.OwningRank = null;
            this.Tool = null;
            this.MetaData = null;
            this.BuildEnvironment = graph.BuildEnvironment;
            this.Macros.AddVerbatim("config", this.BuildEnvironment.Configuration.ToString());
            this.ReasonToExecute = ExecuteReasoning.Undefined();
        }

        /// <summary>
        /// Initialize the module. The base implementation does nothing, but subsequent sub-classing
        /// adds more specific details. Always invoke the base.Init.
        /// The parent module is present for any cases in which parentage is useful for the initialization of the child. 
        /// </summary>
        /// <param name="parent">Parent.</param>
        protected virtual void
        Init(
            Module parent)
        { }

        /// <summary>
        /// Utillity function to determine whether a specific module type can be created. Does it satisfy all requirements,
        /// such as platform and configuration filters.
        /// </summary>
        /// <returns><c>true</c> if can create the specified moduleType; otherwise, <c>false</c>.</returns>
        /// <param name="moduleType">Module type.</param>
        public static bool
        CanCreate(
            System.Type moduleType)
        {
            var platformFilters = moduleType.GetCustomAttributes(typeof(PlatformFilterAttribute), true) as PlatformFilterAttribute[];
            var configurationFilters = moduleType.GetCustomAttributes(typeof(ConfigurationFilterAttribute), true) as ConfigurationFilterAttribute[];
            if (platformFilters.Length > 0 && !platformFilters[0].Includes(Graph.Instance.BuildEnvironment.Platform))
            {
                Log.DebugMessage("Cannot create module of type {0} as it does not satisfy the platform filter", moduleType.ToString());
                return false;
            }
            if (configurationFilters.Length > 0 && !configurationFilters[0].Includes(Graph.Instance.BuildEnvironment.Configuration))
            {
                Log.DebugMessage("Cannot create module of type {0} as it does not satisfy the configuration filter", moduleType.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// Define the delegate that can be invoked between module construction and Init being called.
        /// </summary>
        public delegate void PreInitDelegate<T>(T module);

        /// <summary>
        /// Create the specified module type T, given an optional parent and pre-init callback.
        /// If a parent is provided, two macros are defined:
        /// 'parentmodulename' which is linked to the parent module's 'modulename' macro
        /// 'encapsulatedparentmodulename' which is linked to the parent's encapsulated module's 'modulename' macro
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="preInitCallback">Pre init callback.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T
        Create<T>(
            Module parent = null,
            PreInitDelegate<T> preInitCallback = null) where T : Module, new()
        {
            try
            {
                if (!CanCreate(typeof(T)))
                {
                    return null;
                }

                var module = new T();
                if (null != parent)
                {
                    module.Macros.Add("parentmodulename", parent.Macros["modulename"]);

                    var encapsulatingParent = parent.GetEncapsulatingReferencedModule();
                    module.Macros.Add("encapsulatedparentmodulename", encapsulatingParent.Macros["modulename"]);
                }
                if (preInitCallback != null)
                {
                    preInitCallback(module);
                }
                module.Init(parent);
                module.GetExecutionPolicy(Graph.Instance.Mode);
                AllModules.Add(module);
                return module;
            }
            catch (ModuleCreationException exception)
            {
                // persist the module type from the inner-most module creation call
                throw exception;
            }
            catch (System.Exception exception)
            {
                throw new ModuleCreationException(typeof(T), exception);
            }
        }

        /// <summary>
        /// Register a path against a particular key for the module. Useful for output paths that are referenced in dependents.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="path">Path.</param>
        protected void
        RegisterGeneratedFile(
            PathKey key,
            TokenizedString path)
        {
            if (this.GeneratedPaths.ContainsKey(key))
            {
                Log.DebugMessage("Key '{0}' already exists", key);
                return;
            }
            this.GeneratedPaths.Add(key, path);
        }

        /// <summary>
        /// Register an empty path against a given key.
        /// </summary>
        /// <param name="key">Key.</param>
        private void
        RegisterGeneratedFile(
            PathKey key)
        {
            this.RegisterGeneratedFile(key, null);
        }

        private void
        InternalDependsOn(
            Module module)
        {
            if (this == module)
            {
                throw new Exception("Circular reference. Module {0} cannot depend on itself", this.ToString());
            }
            if (this.DependentsList.Contains(module))
            {
                return;
            }
            this.DependentsList.Add(module);
            module.DependeesList.Add(this);
        }

        /// <summary>
        /// An axiom of Bam. If a module depends on another, that other must have completely been brought up-to-date
        /// before the first module can begin to build.
        /// </summary>
        /// <param name="module">Module to depend upon.</param>
        /// <param name="moreModules">A zero-or-longer list or further modules to depend upon</param>
        public void
        DependsOn(
            Module module,
            params Module[] moreModules)
        {
            this.InternalDependsOn(module);
            foreach (var m in moreModules)
            {
                this.InternalDependsOn(m);
            }
        }

        /// <summary>
        /// An axion of Bam. Depend upon a list of modules.
        /// </summary>
        /// <param name="modules">Modules.</param>
        public void
        DependsOn(
            System.Collections.Generic.IEnumerable<Module> modules)
        {
            this.DependentsList.AddRangeUnique(modules);
            foreach (var module in modules)
            {
                module.DependeesList.Add(this);
            }
        }

        private void
        InternalRequires(
            Module module)
        {
            if (this == module)
            {
                throw new Exception("Circular reference. Module {0} cannot require itself", this.ToString());
            }
            if (this.RequiredDependentsList.Contains(module))
            {
                return;
            }
            this.RequiredDependentsList.Add(module);
            module.RequiredDependeesList.Add(this);
        }

        /// <summary>
        /// An axiom of Bam. A module requires another module, ensures that both modules will be brought up-to-date.
        /// </summary>
        /// <param name="module">Module.</param>
        /// <param name="moreModules">A zero-or-longer list or further modules to depend upon</param>
        public void
        Requires(
            Module module,
            params Module[] moreModules)
        {
            this.InternalRequires(module);
            foreach (var m in moreModules)
            {
                this.InternalRequires(m);
            }
        }

        /// <summary>
        /// An axiom of Bam. Require a list of modules to exist.
        /// </summary>
        /// <param name="modules">Modules.</param>
        public void
        Requires(
            System.Collections.Generic.IEnumerable<Module> modules)
        {
            this.RequiredDependentsList.AddRangeUnique(modules);
            foreach (var module in modules)
            {
                module.RequiredDependeesList.Add(this);
            }
        }

        /// <summary>
        /// Get or set the Settings instance associated with the Tool for this Module. Can be null.
        /// </summary>
        /// <value>The settings.</value>
        public Settings Settings
        {
            get;
            set;
        }

        /// <summary>
        /// Get the package definition containing this module.
        /// </summary>
        /// <value>The package definition.</value>
        public PackageDefinition PackageDefinition
        {
            get;
            private set;
        }

        /// <summary>
        /// Delegate used for private-scope patching of Settings.
        /// </summary>
        public delegate void PrivatePatchDelegate(Settings settings);

        /// <summary>
        /// Add a private patch to the current module. Usually this takes the form of a lambda function.
        /// </summary>
        /// <param name="dlg">Dlg.</param>
        public void
        PrivatePatch(
            PrivatePatchDelegate dlg)
        {
            this.PrivatePatches.Add(dlg);
        }

        /// <summary>
        /// Delegate used for public-scope patching of Settings. Note that appliedTo is the module on which
        /// this delegate is being applied.
        /// </summary>
        public delegate void PublicPatchDelegate(Settings settings, Module appliedTo);

        /// <summary>
        /// Add a public patch to the current module. Usually this takes the form of a lambda function.
        /// </summary>
        /// <param name="dlg">Dlg.</param>
        public void
        PublicPatch(
            PublicPatchDelegate dlg)
        {
            this.PublicPatches.Add(dlg);
        }

        /// <summary>
        /// Instruct a module to use the public patches from module.
        /// </summary>
        /// <param name="module">Module.</param>
        public void
        UsePublicPatches(
            Module module)
        {
            this.ExternalPatches.Add(module.PublicPatches);
            this.ExternalPatches.AddRange(module.ExternalPatches);
        }

        /// <summary>
        /// Determine whether a module has any patches to be applied.
        /// </summary>
        /// <value><c>true</c> if this instance has patches; otherwise, <c>false</c>.</value>
        public bool HasPatches
        {
            get
            {
                return (this.PrivatePatches.Count() > 0) ||
                       (this.PublicPatches.Count() > 0) ||
                       (this.ExternalPatches.Count() > 0);
            }
        }

        /// <summary>
        /// Obtain a read-only list of modules it depends on.
        /// </summary>
        /// <value>The dependents.</value>
        public System.Collections.ObjectModel.ReadOnlyCollection<Module> Dependents
        {
            get
            {
                return this.DependentsList.ToReadOnlyCollection();
            }
        }

        /// <summary>
        /// Obtain a read-only list of modules that depend on it.
        /// </summary>
        /// <value>The dependees.</value>
        public System.Collections.ObjectModel.ReadOnlyCollection<Module> Dependees
        {
            get
            {
                return this.DependeesList.ToReadOnlyCollection();
            }
        }

        /// <summary>
        /// Obtain a read-only list of modules it requires to be up-to-date.
        /// </summary>
        /// <value>The requirements.</value>
        public System.Collections.ObjectModel.ReadOnlyCollection<Module> Requirements
        {
            get
            {
                return this.RequiredDependentsList.ToReadOnlyCollection();
            }
        }

        /// <summary>
        /// Obtain a read-only list of modules that require it.
        /// </summary>
        /// <value>The requirements.</value>
        public System.Collections.ObjectModel.ReadOnlyCollection<Module> Requirees
        {
            get
            {
                return this.RequiredDependeesList.ToReadOnlyCollection();
            }
        }

        /// <summary>
        /// Obtain a read-only list of dependents that are children of this module.
        /// </summary>
        /// <value>The children.</value>
        public System.Collections.ObjectModel.ReadOnlyCollection<Module> Children
        {
            get
            {
                return new System.Collections.ObjectModel.ReadOnlyCollection<Module>(this.DependentsList.Where(item => (item is IChildModule) && ((item as IChildModule).Parent == this)).ToList());
            }
        }

        private Array<Module> DependentsList = new Array<Module>();
        private Array<Module> DependeesList = new Array<Module>();

        private Array<Module> RequiredDependentsList = new Array<Module>();
        private Array<Module> RequiredDependeesList = new Array<Module>();

        private System.Collections.Generic.List<PrivatePatchDelegate> PrivatePatches = new System.Collections.Generic.List<PrivatePatchDelegate>();
        private System.Collections.Generic.List<PublicPatchDelegate> PublicPatches = new System.Collections.Generic.List<PublicPatchDelegate>();
        private System.Collections.Generic.List<System.Collections.Generic.List<PublicPatchDelegate>> ExternalPatches = new System.Collections.Generic.List<System.Collections.Generic.List<PublicPatchDelegate>>();

        /// <summary>
        /// Get the dictionary of keys and strings for all registered generated paths with the module.
        /// </summary>
        /// <value>The generated paths.</value>
        public System.Collections.Generic.Dictionary<PathKey, TokenizedString> GeneratedPaths
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets meta data on the module, which build mode packages use to associated extra data for builds.
        /// </summary>
        /// <value>The meta data.</value>
        public object MetaData
        {
            get;
            set;
        }

        /// <summary>
        /// Internal module execution function, invoked from IModuleExecution.
        /// </summary>
        /// <param name="context">Context.</param>
        protected abstract void
        ExecuteInternal(
            ExecutionContext context);

        void
        IModuleExecution.Execute(
            ExecutionContext context)
        {
            if (context.Evaluate)
            {
                if (null != this.EvaluationTask)
                {
                    this.EvaluationTask.Wait();
                }
                if (null == this.ReasonToExecute)
                {
                    Log.Message(context.ExplainLoggingLevel, "Module {0} is up-to-date", this.ToString());
                    return;
                }
                Log.Message(context.ExplainLoggingLevel, "Module {0} will change because {1}.", this.ToString(), this.ReasonToExecute.ToString());
            }
            this.ExecuteInternal(context);
        }

        /// <summary>
        /// Determine if the module is a top-level module, i.e. is from the package in which Bam was invoked.
        /// </summary>
        /// <value><c>true</c> if top level; otherwise, <c>false</c>.</value>
        public bool TopLevel
        {
            get
            {
                var isTopLevel = (0 == this.DependeesList.Count) && (0 == this.RequiredDependeesList.Count);
                return isTopLevel;
            }
        }

        /// <summary>
        /// Gets the macros associated with this Module.
        /// </summary>
        /// <value>The macros.</value>
        public MacroList Macros
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the ModuleCollection, which is associated with a rank in the DependencyGraph.
        /// </summary>
        /// <value>The owning rank.</value>
        public ModuleCollection OwningRank
        {
            get;
            set;
        }

        /// <summary>
        /// For the given build mode, perform the necessary actions to generate an execution policy.
        /// </summary>
        /// <param name="mode">Mode.</param>
        protected abstract void
        GetExecutionPolicy(
            string mode);

        private Module TheTool;
        /// <summary>
        /// Get or set the tool associated with the module.
        /// </summary>
        public Module Tool
        {
            get
            {
                return this.TheTool;
            }

            protected set
            {
                if ((null != value) && !(value is ITool))
                {
                    throw new Exception("Tool {0} does not implement {1}", value.GetType().ToString(), typeof(ITool).ToString());
                }
                this.TheTool = value;
            }
        }

        /// <summary>
        /// Apply any patches set on the module with the settings for its tool.
        /// </summary>
        public void
        ApplySettingsPatches()
        {
            this.ApplySettingsPatches(this.Settings, true);
        }

        /// <summary>
        /// Apply any patches set on the module with the settings for its tool.
        /// </summary>
        /// <param name="settings">Settings.</param>
        /// <param name="honourParents">If set to <c>true</c> honour parents takes private patches from any parent module
        /// and also invokes those if this module is a child.</param>
        public void
        ApplySettingsPatches(
            Settings settings,
            bool honourParents)
        {
            if (null == settings)
            {
                return;
            }
            // Note: first private patches, followed by public patches
            // TODO: they could override each other - anyway to check?
            var parentModule = (this is IChildModule) && honourParents ? (this as IChildModule).Parent : null;
            if (parentModule != null)
            {
                foreach (var patch in parentModule.PrivatePatches)
                {
                    patch(settings);
                }
            }
            foreach (var patch in this.PrivatePatches)
            {
                patch(settings);
            }
            if (parentModule != null)
            {
                foreach (var patch in parentModule.PublicPatches)
                {
                    patch(settings, this);
                }
            }
            foreach (var patch in this.PublicPatches)
            {
                patch(settings, this);
            }
            if (parentModule != null)
            {
                foreach (var patchList in parentModule.ExternalPatches)
                {
                    foreach (var patch in patchList)
                    {
                        patch(settings, this);
                    }
                }
            }
            foreach (var patchList in this.ExternalPatches)
            {
                foreach (var patch in patchList)
                {
                    patch(settings, this);
                }
            }
        }

        /// <summary>
        /// Determine the reason why the module should (re)build.
        /// </summary>
        /// <value>The reason to execute.</value>
        public ExecuteReasoning ReasonToExecute
        {
            get;
            protected set;
        }

        /// <summary>
        /// Get or set the async Task for execution.
        /// </summary>
        /// <value>The execution task.</value>
        public System.Threading.Tasks.Task ExecutionTask
        {
            get;
            set;
        }

        /// <summary>
        /// Get the async Task for evaluating the module for whether it is up-to-date.
        /// </summary>
        /// <value>The evaluation task.</value>
        public System.Threading.Tasks.Task
        EvaluationTask
        {
            get;
            protected set;
        }

        /// <summary>
        /// Evaluate the module to determine whether it requires a (re)build.
        /// </summary>
        public abstract void
        Evaluate();

        /// <summary>
        /// Get the Environment associated with this module. The same module in different environments will be different
        /// instances of a Module.
        /// </summary>
        /// <value>The build environment.</value>
        public Environment BuildEnvironment
        {
            get;
            private set;
        }

        /// <summary>
        /// A referenced module is an encapsulating module, and can be considered to be uniquely identifiable by name.
        /// An unreferenced module belongs, in some part, to another module and perhaps many of them exist within the
        /// dependency graph. For identification, walking up the hierarchy of dependees will eventually find a referenced
        /// module, and this is that module's encapsulating module.
        /// It is useful for logical grouping, such as build sub-folder names.
        /// Macros added to the module:
        /// 'encapsulatingmodulename'
        /// 'encapsulatingbuilddir'
        /// </summary>
        /// <returns>The encapsulating referenced module.</returns>
        public Module
        GetEncapsulatingReferencedModule()
        {
            if (Graph.Instance.IsReferencedModule(this))
            {
                return this;
            }
            if (this.DependeesList.Count > 1)
            {
                Log.DebugMessage("More than one dependee attached to {0}, so taking the first as the encapsulating module. This may be incorrect.", this.ToString());
            }
            if (this.RequiredDependeesList.Count > 1)
            {
                Log.DebugMessage("More than one requiree attached to {0}, so taking the first as the encapsulating module. This may be incorrect.", this.ToString());
            }
            Module encapsulating;
            if (0 == this.DependeesList.Count)
            {
                if (0 == this.RequiredDependeesList.Count)
                {
                    throw new Exception("No dependees or requirees attached to {0}. Cannot determine the encapsulating module", this.ToString());
                }
                encapsulating = this.RequiredDependeesList[0].GetEncapsulatingReferencedModule();
            }
            else
            {
                encapsulating = this.DependeesList[0].GetEncapsulatingReferencedModule();
            }
            this.Macros.AddVerbatim("encapsulatingmodulename", encapsulating.GetType().Name);
            this.Macros.Add("encapsulatingbuilddir", encapsulating.Macros["packagebuilddir"]);
            return encapsulating;
        }

        private void
        Complete()
        {
            var graph = Graph.Instance;
            var encapsulatingModule = this.GetEncapsulatingReferencedModule();
            // TODO: there may have to be a more general module type for something that is not built, as this affects modules referred to prebuilts too
            // note that this cannot be a class, as modules already are derived from another base class (generally)
            if (!(encapsulatingModule is PreBuiltTool))
            {
                this.Macros.Add("moduleoutputdir", graph.BuildModeMetaData.ModuleOutputDirectory(this, encapsulatingModule));
            }

            // modules that are encapsulated, have settings, and aren't a child (as their parent is also encapsulated, and thus gets this too), inherit the
            // public patches from the encapsulating module, since this is identical behavior to 'using public patches'
            if (encapsulatingModule != this)
            {
                if (this.Settings != null)
                {
                    if (!(this is IChildModule))
                    {
                        this.UsePublicPatches(encapsulatingModule);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures all modules have been 'completed', which ensures that everything is in place, ready for validation.
        /// </summary>
        static public void
        CompleteModules()
        {
            foreach (var module in AllModules.Reverse<Module>())
            {
                module.Complete();
            }
        }

        /// <summary>
        /// Make a path which is a placeholder, and will eventually be aliased.
        /// </summary>
        /// <returns>The placeholder path.</returns>
        public TokenizedString
        MakePlaceholderPath()
        {
            return TokenizedString.CreateUncached(string.Empty, this);
        }

        /// <summary>
        /// Create a TokenizedString associated with this module, using the MacroList in the module.
        /// </summary>
        /// <returns>The tokenized string.</returns>
        /// <param name="format">Format.</param>
        /// <param name="argv">Argv.</param>
        public TokenizedString
        CreateTokenizedString(
            string format,
            params TokenizedString[] argv)
        {
            if (0 == argv.Length)
            {
                return TokenizedString.Create(format, this);
            }
            var positionalTokens = new TokenizedStringArray(argv);
            return TokenizedString.Create(format, this, positionalTokens);
        }

        /// <summary>
        /// Static utility method to count all modules created. Useful for profiling.
        /// </summary>
        /// <value>The count.</value>
        public static int
        Count
        {
            get
            {
                return AllModules.Count;
            }
        }
    }
}
