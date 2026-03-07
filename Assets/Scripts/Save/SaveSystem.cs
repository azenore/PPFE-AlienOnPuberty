using System.IO;
using UnityEngine;
using VN.Save;

namespace VN.Runtime
{
    public static class SaveSystem
    {
        private const string SaveFileName = "save.json";
        private const string SaveFolderName = "save";

        // Racine du jeu (dossier de l'exe en build, racine du projet en Editor)
        private static string SaveDirectory =>
            Path.Combine(Path.GetDirectoryName(Application.dataPath), SaveFolderName);

        private static string SavePath =>
            Path.Combine(SaveDirectory, SaveFileName);

        /// <summary>Serializes and writes SaveData to disk.</summary>
        public static void Save(SaveData data)
        {
            Directory.CreateDirectory(SaveDirectory);
            string json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(SavePath, json);
            Debug.Log($"[SaveSystem] Sauvegardé dans {SavePath}");
        }

        /// <summary>Reads and deserializes SaveData from disk. Returns null if no save exists.</summary>
        public static SaveData Load()
        {
            if (!File.Exists(SavePath)) return null;
            string json = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<SaveData>(json);
        }

        /// <summary>Returns true if a save file exists on disk.</summary>
        public static bool HasSave() => File.Exists(SavePath);

        /// <summary>Deletes the save file from disk.</summary>
        public static void DeleteSave()
        {
            if (File.Exists(SavePath))
                File.Delete(SavePath);
        }
    }
}
