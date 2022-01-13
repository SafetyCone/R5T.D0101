using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

using R5T.T0044;
using R5T.T0097;


namespace R5T.D0101.I001
{
    public static class IFileSystemOperatorExtensions
    {
        public static Project[] LoadProjectEntriesFromJsonFile(this IFileSystemOperator _,
            string jsonFilePath)
        {
            var output = JsonFileHelper.LoadFromFile<Project[]>(jsonFilePath);
            return output;
        }

        public static void WriteToJsonFile(this IFileSystemOperator _,
            string jsonFilePath,
            IEnumerable<Project> projects,
            bool overwrite = IOHelper.DefaultOverwriteValue)
        {
            JsonFileHelper.WriteToFile(jsonFilePath, projects, overwrite: overwrite);
        }
    }
}
