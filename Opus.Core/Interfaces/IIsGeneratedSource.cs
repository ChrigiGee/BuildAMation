﻿// <copyright file="IIsGeneratedSource.cs" company="Mark Final">
//  Opus
// </copyright>
// <summary>Opus Core</summary>
// <author>Mark Final</author>
namespace Opus.Core
{
    public interface IIsGeneratedSource
    {
        bool
        AutomaticallyHandledByBuilder(Opus.Core.Target target);
    }
}