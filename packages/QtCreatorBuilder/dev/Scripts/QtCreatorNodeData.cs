// <copyright file="QtCreatorNodeData.cs" company="Mark Final">
//  Opus package
// </copyright>
// <summary>QtCreator package</summary>
// <author>Mark Final</author>

// Automatically generated by Opus v0.20
namespace QtCreatorBuilder
{
    public class NodeData
    {
        private System.Collections.Generic.Dictionary<string, Opus.Core.StringArray> Dictionary = new System.Collections.Generic.Dictionary<string, Opus.Core.StringArray>();
        private System.Collections.Generic.Dictionary<string, Opus.Core.StringArray> SingleValueDictionary = new System.Collections.Generic.Dictionary<string, Opus.Core.StringArray>();

        public bool Contains(string VariableName)
        {
            bool contains = this.Dictionary.ContainsKey(VariableName) || this.SingleValueDictionary.ContainsKey(VariableName);
            return contains;
        }

        public Opus.Core.StringArray this[string VariableName]
        {
            get
            {
                if (!this.Dictionary.ContainsKey(VariableName) && !this.SingleValueDictionary.ContainsKey(VariableName))
                {
                    throw new Opus.Core.Exception(System.String.Format("Unable to locate variable '{0}'", VariableName));
                }

                if (this.Dictionary.ContainsKey(VariableName))
                {
                    return this.Dictionary[VariableName];
                }

                return this.SingleValueDictionary[VariableName];
            }
        }

        public void AddVariable(string VariableName, string Value)
        {
            Dictionary.Add(VariableName, new Opus.Core.StringArray(Value));
        }

        public void AddUniqueVariable(string VariableName, Opus.Core.StringArray Value)
        {
            SingleValueDictionary.Add(VariableName, Value);
        }

        public void Merge(NodeData data)
        {
            foreach (System.Collections.Generic.KeyValuePair<string, Opus.Core.StringArray> entry in data.Dictionary)
            {
                if (this.Dictionary.ContainsKey(entry.Key))
                {
                    this.Dictionary[entry.Key].AddRange(entry.Value);
                }
                else
                {
                    this.Dictionary.Add(entry.Key, entry.Value);
                }
            }

            foreach (System.Collections.Generic.KeyValuePair<string, Opus.Core.StringArray> entry in data.SingleValueDictionary)
            {
                if (this.SingleValueDictionary.ContainsKey(entry.Key))
                {
                    throw new Opus.Core.Exception(System.String.Format("Repeated entry for '{0}'", entry.Key));
                }

                this.SingleValueDictionary.Add(entry.Key, entry.Value);
            }
        }
    }
}
