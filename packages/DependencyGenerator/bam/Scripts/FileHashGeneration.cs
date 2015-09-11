#region License
// Copyright (c) 2010-2015, Mark Final
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
#if BAM_ENABLE_FILE_HASHING
namespace DependencyGenerator
{
    public class FileHashGeneration
    {
        private static System.Threading.Thread hashingThread = null;
        private static System.Threading.ManualResetEvent completedEvent = new System.Threading.ManualResetEvent(false);
        private static System.Collections.Generic.Dictionary<string, int> fileHashDictionary = new System.Collections.Generic.Dictionary<string, int>();
        private static System.Collections.Generic.Dictionary<string, int> loadedFileHashDictionary = null;

        internal static DependencyQueue<string> FileProcessQueue
        {
            get;
            private set;
        }

        static string FileHashPathName
        {
            get
            {
                string hashFile = System.IO.Path.Combine(Bam.Core.State.BuildRoot, Bam.Core.State.PackageInfo[0].FullName);
                hashFile = System.IO.Path.Combine(hashFile, "sourceFileHash");
                return hashFile;
            }
        }

        static
        FileHashGeneration()
        {
            FileProcessQueue = new DependencyQueue<string>();

            System.Threading.ParameterizedThreadStart threadStart = new System.Threading.ParameterizedThreadStart(ProcessFileQueue);
            hashingThread = new System.Threading.Thread(threadStart);
            hashingThread.Start(FileProcessQueue);

            if (System.IO.File.Exists(FileHashPathName))
            {
                System.Collections.Generic.Dictionary<string, int> dictionary = null;
                using (System.IO.FileStream fileStream = new System.IO.FileStream(FileHashPathName, System.IO.FileMode.Open))
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    dictionary = formatter.Deserialize(fileStream) as System.Collections.Generic.Dictionary<string, int>;
                }

                loadedFileHashDictionary = dictionary;
            }
        }

        public static bool
        HaveFileHashesChanged(
            Bam.Core.StringArray paths)
        {
            if (null == loadedFileHashDictionary)
            {
                Bam.Core.Log.DebugMessage("No loaded dictionary - change everything");
                return true;
            }

            foreach (string path in paths)
            {
                int savedHash;
                bool hasValue = loadedFileHashDictionary.TryGetValue(path, out savedHash);
                if (!hasValue)
                {
                    Bam.Core.Log.DebugMessage("No hash for '{0}'", path);
                    return true;
                }

                GenerateFileHash(path);

                int hash = fileHashDictionary[path];
                bool hashHasChanged = (hash != savedHash);
                if (hashHasChanged)
                {
                    Bam.Core.Log.DebugMessage("Hash has changed '{0}': was {1}, now {2}", path, savedHash, hash);
                    return true;
                }
            }

            Bam.Core.Log.DebugMessage("All hashes identical");
            return false;
        }

        private static void
        GenerateFileHash(
            string entry)
        {
            // has the hash already  been generated?
            if (fileHashDictionary.ContainsKey(entry))
            {
                return;
            }

            string fileContents = string.Empty;
            using (System.IO.TextReader reader = new System.IO.StreamReader(entry))
            {
                fileContents = reader.ReadToEnd();
            }

            int hash = fileContents.GetHashCode();
            fileHashDictionary[entry] = hash;
        }

        internal static void
        ProcessFileQueue(
            object obj)
        {
            // wait for the build to start
            System.Threading.WaitHandle.WaitAll(new System.Threading.WaitHandle[] { Bam.Core.State.BuildStartedEvent }, -1);
            Bam.Core.BuildManager buildManager = Bam.Core.State.BuildManager;
            buildManager.AdditionalThreadCompletionEvents.Add(completedEvent);

            DependencyQueue<string> data = obj as DependencyQueue<string>;
            for (; ; )
            {
                // wake up when there is data to be processed
                int waitResult = System.Threading.WaitHandle.WaitAny(new System.Threading.WaitHandle[] { data.IsAlive, buildManager.Finished }, -1);

                if (1 == waitResult)
                {
                    break;
                }

                // do work while it's available
                do
                {
                    string entry = data.Dequeue();
                    GenerateFileHash(entry);
                }
                while (System.Threading.WaitHandle.WaitAll(new System.Threading.WaitHandle[] { data.IsAlive }, 0));
            }

            if (fileHashDictionary.Count > 0)
            {
                if (null != loadedFileHashDictionary)
                {
                    // merge NEW dictionary onto OLD
                    foreach (string key in fileHashDictionary.Keys)
                    {
                        loadedFileHashDictionary[key] = fileHashDictionary[key];
                    }
                }
                else
                {
                    loadedFileHashDictionary = fileHashDictionary;
                }

                string hashPath = FileHashPathName;
                string hashDirectory = System.IO.Path.GetDirectoryName(hashPath);
                if (!System.IO.Directory.Exists(hashDirectory))
                {
                    System.IO.Directory.CreateDirectory(hashDirectory);
                }

                // save the combined, and updated, dictionaries
                using (System.IO.FileStream fileStream = new System.IO.FileStream(hashPath, System.IO.FileMode.Create))
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    formatter.Serialize(fileStream, loadedFileHashDictionary);
                }
            }

            // signal complete so that the BuildManager no longer waits
            completedEvent.Set();
        }
    }
}
#endif // BAM_ENABLE_FILE_HASHING