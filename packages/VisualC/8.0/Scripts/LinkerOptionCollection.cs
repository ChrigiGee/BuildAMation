// <copyright file="LinkerOptionCollection.cs" company="Mark Final">
//  Opus package
// </copyright>
// <summary>VisualC package</summary>
// <author>Mark Final</author>
namespace VisualC
{
    public sealed partial class LinkerOptionCollection : VisualCCommon.LinkerOptionCollection
    {
        public LinkerOptionCollection()
            : base()
        {}

        public LinkerOptionCollection(Opus.Core.DependencyNode node)
            : base(node)
        {
        }
    }
}