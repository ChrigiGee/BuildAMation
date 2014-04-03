﻿// <copyright file="IIdentifyExternalDependencies.cs" company="Mark Final">
//  Opus
// </copyright>
// <summary>Opus Core</summary>
// <author>Mark Final</author>
namespace Opus.Core
{
    public interface IIdentifyExternalDependencies
    {
        TypeArray IdentifyExternalDependencies(Core.Target target);
    }
}