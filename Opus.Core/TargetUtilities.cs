﻿// <copyright file="TargetUtilities.cs" company="Mark Final">
//  Opus
// </copyright>
// <summary>Opus Core</summary>
// <author>Mark Final</author>
namespace Opus.Core
{
    public static class TargetUtilities
    {
        /// <summary>
        /// For a given Target, identify whether the provided filters for platform, configuration and toolchain are a match or not.
        /// </summary>
        /// <param name="target">The Target to evaluate.</param>
        /// <param name="filterInterface">The filters to look for.</param>
        /// <returns>True if the Target matches the filters, false otherwise.</returns>
        public static bool MatchFilters(Target target, ITargetFilters filterInterface)
        {
            var baseTarget = (BaseTarget)target;
            if (!baseTarget.HasPlatform(filterInterface.Platform))
            {
                return false;
            }
            if (!baseTarget.HasConfiguration(filterInterface.Configuration))
            {
                return false;
            }
            if (null == filterInterface.ToolsetTypes)
            {
                return true;
            }

            foreach (var toolsetType in filterInterface.ToolsetTypes)
            {
                if (target.HasToolsetType(toolsetType))
                {
                    Log.DebugMessage("Target filter '{0}' matches target '{1}'", filterInterface.ToString(), target.ToString());
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determine a standard name for a directory for this Target.
        /// </summary>
        /// <param name="target">Target to get the directory name for.</param>
        /// <returns>The Target's directory name.</returns>
        public static string DirectoryName(Target target)
        {
            if (null == target.Toolset)
            {
                throw new Exception("Getting the directory name for a null Toolset is not supported");
            }
            var versionString = target.Toolset.Version((BaseTarget)target);
            var builder = new System.Text.StringBuilder();
            builder.AppendFormat("{0}{1}", target.ToString(), versionString);
            return builder.ToString().ToLower();
        }
    }
}