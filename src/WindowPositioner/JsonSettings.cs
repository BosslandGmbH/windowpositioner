using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace WindowPositioner
{
    public abstract class JsonSettings : IDisposable
    {
        protected JsonSettings(string path)
        {
            FilePath = path;
            Load();
        }

        [JsonIgnore]
        public string FilePath { get; set; }

        private static string AssemblyPath => Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        public static string SettingsPath => Path.Combine(AssemblyPath, "Settings");

        #region Implementation of IDisposable

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Save();
        }

        #endregion

        public void Save()
        {
            SaveAs(FilePath);
        }

        public void SaveAs(string file)
        {
            try
            {
                // So basically, if we need to create the file, also create the directory structure.
                if (!File.Exists(file))
                {
                    var dir = Path.GetDirectoryName(file);
                    if (dir != null && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }

                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                // Write the serialized shit to file.
                File.WriteAllText(file, json);
            }
            catch
            {
            }
        }

        protected void Load()
        {
            LoadFrom(FilePath);
        }

        protected void LoadFrom(string file)
        {
            // First, we need to setup the default values, in case things changed.
            foreach (var propertyInfo in GetType().GetProperties())
            {
                if (!propertyInfo.CanWrite)
                    continue;

                var attrs = propertyInfo.GetCustomAttributes(typeof (DefaultValueAttribute), true);

                if (!attrs.Any())
                    continue;

                foreach (DefaultValueAttribute a in attrs)
                    propertyInfo.SetValue(this, a.Value, null);
            }

            // If the file exists, load it up.
            if (File.Exists(file))
            {
                var fileText = File.ReadAllText(file);
                JsonConvert.PopulateObject(fileText, this);
            }

            // We always save, since the data could have changed.
            Save();
        }

        public static string GetSettingsFilePath(params string[] subPathParts)
        {
            var paths = new List<string>();
            paths.Add(SettingsPath);
            paths.AddRange(subPathParts);
            return Path.Combine(paths.ToArray());
        }
    }
}