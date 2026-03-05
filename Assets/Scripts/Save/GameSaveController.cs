using System.Collections.Generic;
using UnityEngine;
using VN.Data;
using VN.Save;

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

        private void OnApplicationQuit()
        {
            // Ne sauvegarde que si une partie est en cours
            if (!string.IsNullOrEmpty(chapterManager.CurrentChapterName))
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
                currentLineIndex = dialogueEngine.CurrentIndex,
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
        }

        /// <summary>Loads SaveData from disk and restores full game state. Returns false if no save exists.</summary>
        public bool LoadGame()
        {
            SaveData data = SaveSystem.Load();
            if (data == null) return false;

            // Restore protagonist
            protagonistData.playerName = data.protagonistName;
            protagonistData.hairColor = new Color(data.hairColorR, data.hairColorG, data.hairColorB);
            protagonistData.eyeColor = new Color(data.eyeColorR, data.eyeColorG, data.eyeColorB);

            // Restore affinities
            protagonistData.ResetAffinities();
            foreach (var entry in data.affinities)
            {
                CharacterData character = FindCharacterByName(entry.characterName);
                if (character != null)
                    protagonistData.SetAffinity(character, entry.value);
            }

            // Restore chapter and line position
            DialogueChapter chapter = FindChapterByName(data.currentChapterName);
            if (chapter != null)
                chapterManager.LoadChapterAtLine(chapter, data.currentLineIndex);
            else
                Debug.LogWarning($"[GameSaveController] Chapitre introuvable : {data.currentChapterName}");

            return true;
        }

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
