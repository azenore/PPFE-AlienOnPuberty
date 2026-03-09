using System.IO;
using UnityEngine;
using VN.Settings;

namespace VN.Runtime
{
    public static class SettingsSystem
    {
        private const string SettingsFileName = "settings.json";
        private const string SaveFolderName = "save";

        private static string SaveDirectory =>
            Path.Combine(Path.GetDirectoryName(Application.dataPath), SaveFolderName);

        private static string SettingsPath =>
            Path.Combine(SaveDirectory, SettingsFileName);

        /// <summary>Writes settings to disk.</summary>
        public static void Save(SettingsData data)
        {
            Directory.CreateDirectory(SaveDirectory);
            string json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(SettingsPath, json);
        }

        /// <summary>Reads settings from disk. Returns defaults if no file exists.</summary>
        public static SettingsData Load()
        {
            if (!File.Exists(SettingsPath))
                return new SettingsData();
            string json = File.ReadAllText(SettingsPath);
            return JsonUtility.FromJson<SettingsData>(json);
        }
    }
}
