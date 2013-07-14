// <copyright file="LinkerUtilities.cs" company="Mark Final">
//  Opus package
// </copyright>
// <summary>C package</summary>
// <author>Mark Final</author>
namespace C
{
    public static class LinkerUtilities
    {
        public static void AppendLibrariesToCommandLine(Opus.Core.StringArray commandLineBuilder,
                                                        ILinkerTool linkerTool,
                                                        ILinkerOptions linkerOptions,
                                                        Opus.Core.StringArray otherLibraryPaths)
        {
            var includeStandardLibraries = linkerOptions.DoNotAutoIncludeStandardLibraries && linkerOptions.StandardLibraries.Count > 0;
            var includeOtherLibraries = (null != otherLibraryPaths) && (otherLibraryPaths.Count > 0);
            if (includeStandardLibraries ||
                (linkerOptions.Libraries.Count > 0) ||
                includeOtherLibraries)
            {
                commandLineBuilder.Add(linkerTool.StartLibraryList);

                if (includeOtherLibraries)
                {
                    commandLineBuilder.AddRange(otherLibraryPaths);
                }

                // TODO: replace with 'var'
                foreach (string libraryPath in linkerOptions.Libraries)
                {
                    if (libraryPath.Contains(" "))
                    {
                        commandLineBuilder.Add(System.String.Format("\"{0}\"", libraryPath));
                    }
                    else
                    {
                        commandLineBuilder.Add(libraryPath);
                    }
                }

                if (includeStandardLibraries)
                {
                    // TODO: replace with 'var'
                    foreach (string standardLibraryPath in linkerOptions.StandardLibraries)
                    {
                        if (standardLibraryPath.Contains(" "))
                        {
                            commandLineBuilder.Add(System.String.Format("\"{0}\"", standardLibraryPath));
                        }
                        else
                        {
                            commandLineBuilder.Add(standardLibraryPath);
                        }
                    }
                }

                commandLineBuilder.Add(linkerTool.EndLibraryList);
            }
        }
    }
}
