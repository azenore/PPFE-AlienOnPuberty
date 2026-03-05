using System.Collections.Generic;
using UnityEngine;
using VN.Data;
using VN.Save;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VN.Runtime
{
    public class GameSaveController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ProtagonistData protagonistData;
        [SerializeField] private ChapterManager chapterManager;
        [SerializeField] private DialogueEngine dialogueEngine;

        [Tooltip("Tous les DialogueChapter du jeu, dans l'ordre.")]
        [SerializeField] private List<DialogueChapter> allChapters;

        private void OnEnable()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
        }

#if UNITY_EDITOR
        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
                TrySave();
        }
#endif

        private void OnApplicationQuit()
        {
            TrySave();
        }

        private void TrySave()
        {
            if (string.IsNullOrEmpty(chapterManager.CurrentChapterName)) return;
            SaveGame();
        }

        /// <summary>Builds a SaveData snapshot from current game state and writes it to disk.</summary>
        public void SaveGame()
        {
            SaveData data = new SaveData
            {
                protagonistName = protagonistData.playerName,
                hairColorR = protagonistData.hairColor.r,
                hairColorG = protagonistData.hairColor.g,
                hairColorB = protagonistData.hairColor.b,
                eyeColorR = protagonistData.eyeColor.r,
                eyeColorG = protagonistData.eyeColor.g,
                eyeColorB = protagonistData.eyeColor.b,
                currentChapterName = chapterManager.CurrentChapterName,
                currentLineIndex = dialogueEngine.LastDisplayedIndex,
            };

            foreach (var (character, value) in protagonistData.GetAllAffinities())
            {
                data.affinities.Add(new AffinitySaveEntry
                {
                    characterName = character.name,
                    value = value,
                });
            }

            SaveSystem.Save(data);
            Debug.Log($"[Save] Chapitre : {data.currentChapterName} | Ligne : {data.currentLineIndex}");
        }

        /// <summary>Loads SaveData from disk and restores full game state. Returns false if no save exists.</summary>
        public bool LoadGame()
        {
            SaveData data = SaveSystem.Load();
            if (data == null)
            {
                Debug.LogWarning("[Save] Aucune sauvegarde trouvée.");
                return false;
            }

            Debug.Log($"[Save] Chargement — Chapitre : {data.currentChapterName} | Ligne : {data.currentLineIndex}");

            protagonistData.playerName = data.protagonistName;
            protagonistData.hairColor = new Color(data.hairColorR, data.hairColorG, data.hairColorB);
            protagonistData.eyeColor = new Color(data.eyeColorR, data.eyeColorG, data.eyeColorB);

            protagonistData.ResetAffinities();
            foreach (var entry in data.affinities)
            {
                CharacterData character = FindCharacterByName(entry.characterName);
                if (character != null)
                    protagonistData.SetAffinity(character, entry.value);
            }

            DialogueChapter chapter = FindChapterByName(data.currentChapterName);
            if (chapter == null)
            {
                Debug.LogError($"[Save] Chapitre introuvable : '{data.currentChapterName}'. Vérifie la liste allChapters dans l'Inspector.");
                return false;
            }

            chapterManager.LoadChapterAtLine(chapter, data.currentLineIndex);
            return true;
        }

        [ContextMenu("Debug — Forcer sauvegarde")]
        private void ForceSave() => SaveGame();

        [ContextMenu("Debug — Supprimer sauvegarde")]
        private void DeleteSave() => SaveSystem.DeleteSave();

        [ContextMenu("Debug — Afficher chemin")]
        private void PrintPath() => Debug.Log(System.IO.Path.Combine(Application.persistentDataPath, "save.json"));

        private DialogueChapter FindChapterByName(string assetName)
            => allChapters.Find(c => c.name == assetName);

        private CharacterData FindCharacterByName(string assetName)
        {
            foreach (var chapter in allChapters)
                foreach (var node in chapter.nodes)
                {
                    if (node.characterOnScreen?.name == assetName) return node.characterOnScreen;
                    if (node.line?.speaker?.name == assetName) return node.line.speaker;
                }
            return null;
        }
    }
}
