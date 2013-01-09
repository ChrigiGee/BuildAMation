﻿// <copyright file="CreatePackageAtAction.cs" company="Mark Final">
//  Opus
// </copyright>
// <summary>Opus main application.</summary>
// <author>Mark Final</author>

[assembly: Opus.Core.RegisterAction(typeof(Opus.CreatePackageAtAction))]

namespace Opus
{
    [Core.TriggerAction]
    internal class CreatePackageAtAction : Core.IActionWithArguments
    {
        public string CommandLineSwitch
        {
            get
            {
                return "-createpackageat";
            }
        }

        public string Description
        {
            get
            {
                return "Create a new package at the specified path.";
            }
        }

        void Opus.Core.IActionWithArguments.AssignArguments(string arguments)
        {
            this.PackagePath = arguments;
        }

        private string PackagePath
        {
            get;
            set;
        }

        public bool Execute()
        {
            bool isWellDefined;
            Core.PackageIdentifier id = Core.PackageUtilities.IsPackageDirectory(this.PackagePath,
                                                                                 out isWellDefined);
            if ((null != id) || isWellDefined)
            {
                throw new Core.Exception(System.String.Format("Package already present at '{0}'", this.PackagePath), false);
            }

            string PackageDirectory = this.PackagePath;
            if (System.IO.Directory.Exists(PackageDirectory))
            {
                Core.Log.Info("Package directory already exists at '{0}'", PackageDirectory);
                return false;
            }
            else
            {
                System.IO.Directory.CreateDirectory(PackageDirectory);
            }

            id = Core.PackageUtilities.IsPackageDirectory(this.PackagePath, out isWellDefined);

            // Xml file for dependencies
            Core.PackageDefinitionFile packageDefinition = new Core.PackageDefinitionFile(id.DefinitionPathName, true);
            if (null == packageDefinition)
            {
                throw new Core.Exception(System.String.Format("Package definition file '%s' could not be created", packageDefinition), false);
            }

            if (Core.State.PackageCreationDependents != null)
            {
                Core.Log.DebugMessage("Adding dependent packages:");
                foreach (string dependentPackage in Core.State.PackageCreationDependents)
                {
                    Core.Log.DebugMessage("\t'{0}'", dependentPackage);
                    string[] packageNameAndVersion = dependentPackage.Split('-');
                    if (packageNameAndVersion.Length != 2)
                    {
                        throw new Core.Exception(System.String.Format("Ill-formed package name-version pair, '{0}'", packageNameAndVersion), false);
                    }

                    Core.PackageIdentifier idToAdd = new Core.PackageIdentifier(packageNameAndVersion[0], packageNameAndVersion[1]);
                    packageDefinition.AddRequiredPackage(idToAdd);
                }
            }
            packageDefinition.OpusAssemblies.Add("Opus.Core");
            {
                Core.DotNetAssemblyDescription system = new Core.DotNetAssemblyDescription("System");
                system.RequiredTargetFramework = "2.0.50727";
                packageDefinition.DotNetAssemblies.Add(system);

                Core.DotNetAssemblyDescription systemXml = new Core.DotNetAssemblyDescription("System.Xml");
                systemXml.RequiredTargetFramework = "2.0.50727";
                packageDefinition.DotNetAssemblies.Add(systemXml);
            }
            packageDefinition.Write();
            id.Definition = packageDefinition;

            // Script file
            string packageScriptPathname = id.ScriptPathName;
            using (System.IO.TextWriter scriptWriter = new System.IO.StreamWriter(packageScriptPathname))
            {
                scriptWriter.WriteLine("// Automatically generated by Opus v" + Core.State.OpusVersionString);
                scriptWriter.WriteLine("namespace " + id.Name);
                scriptWriter.WriteLine("{");
                scriptWriter.WriteLine("    // Add modules here");
                scriptWriter.WriteLine("}");
                scriptWriter.Close();
            }

            Core.Log.Info("Successfully created package '{0}' in '{1}'", id.ToString("-"), id.Root);

            return true;
        }

        #region ICloneable Members

        object System.ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}