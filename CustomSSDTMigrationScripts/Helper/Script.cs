using System;
using System.IO;
using System.Security.Cryptography;

namespace CustomSSDTMigrationScripts
{
    public class Script
    {
        public string UniqueScriptId { get; set; }

        public string OrderCriteria { get; set; }

        public string Name { get; set; }

        public string MigrationType { get; set; }

        public string FullFilePath { get; set; }

        public string ScriptHash
        {
            get
            {
                var hashBase64 = string.Empty;
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(FullFilePath))
                    {
                        var hashBinary = md5.ComputeHash(stream);
                        hashBase64 = Convert.ToBase64String(hashBinary);
                    }
                }

                return hashBase64;
            }
        }
    }
}
