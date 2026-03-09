using System;
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
        [SerializeField] private ProtagonistData protagonistDataAsset;
        [SerializeField] private ChapterManager chapterManager;
        [SerializeField] private DialogueEngine dialogueEngine;

        [Tooltip("Tous les DialogueChapter du jeu, dans l'ordre.")]
        [SerializeField] private List<DialogueChapter> allChapters;

        /// <summary>Fired each time a save is successfully written to disk.</summary>
        public event Action OnSaved;

        // Copie runtime — évite de polluer l'asset ScriptableObject en éditeur
        public ProtagonistData ProtagonistData { get; private set; }

        private Dictionary<string, CharacterData> _characterCache;

        private void Awake()
        {
            ProtagonistData = protagonistDataAsset.CreateRuntimeCopy();
        }

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

        private void OnApplicationQuit() => TrySave();

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
                protagonistName = ProtagonistData.playerName,
                hairColorR = ProtagonistData.hairColor.r,
                hairColorG = ProtagonistData.hairColor.g,
                hairColorB = ProtagonistData.hairColor.b,
                eyeColorR = ProtagonistData.eyeColor.r,
                eyeColorG = ProtagonistData.eyeColor.g,
                eyeColorB = ProtagonistData.eyeColor.b,
                currentChapterName = chapterManager.CurrentChapterName,
                currentLineIndex = dialogueEngine.LastDisplayedIndex,
                lastCharacterOnScreenName = dialogueEngine.CurrentCharacter?.name ?? string.Empty,
            };

            foreach (var (character, value) in ProtagonistData.GetAllAffinities())
            {
                data.affinities.Add(new AffinitySaveEntry
                {
                    characterName = character.name,
                    value = value,
                });
            }

            SaveSystem.Save(data);
            Debug.Log($"[Save] Chapitre : {data.currentChapterName} | Ligne : {data.currentLineIndex} | Perso : {data.lastCharacterOnScreenName}");
            OnSaved?.Invoke();
        }

        /// <summary>Returns true if a save file exists on disk.</summary>
        public bool HasSave() => SaveSystem.HasSave();

        /// <summary>Loads SaveData from disk and restores full game state. Returns false if no save exists.</summary>
        public bool LoadGame()
        {
            SaveData data = SaveSystem.Load();
            if (data == null)
            {
                Debug.LogWarning("[Save] Aucune sauvegarde trouvée.");
                return false;
            }

            Debug.Log($"[Save] Chargement — Chapitre : {data.currentChapterName} | Ligne : {data.currentLineIndex} | Perso : {data.lastCharacterOnScreenName}");

            ProtagonistData.playerName = data.protagonistName;
            ProtagonistData.hairColor = new Color(data.hairColorR, data.hairColorG, data.hairColorB);
            ProtagonistData.eyeColor = new Color(data.eyeColorR, data.eyeColorG, data.eyeColorB);

            ProtagonistData.ResetAffinities();
            foreach (var entry in data.affinities)
            {
                CharacterData character = FindCharacterByName(entry.characterName);
                if (character != null)
                    ProtagonistData.SetAffinity(character, entry.value);
            }

            DialogueChapter chapter = FindChapterByName(data.currentChapterName);
            if (chapter == null)
            {
                Debug.LogError($"[Save] Chapitre introuvable : '{data.currentChapterName}'. Vérifie la liste allChapters dans l'Inspector.");
                return false;
            }

            // Restore le personnage visible AVANT de charger le nœud
            if (!string.IsNullOrEmpty(data.lastCharacterOnScreenName))
            {
                CharacterData savedCharacter = FindCharacterByName(data.lastCharacterOnScreenName);
                dialogueEngine.RestoreCharacter(savedCharacter);
            }

            chapterManager.LoadChapterAtLine(chapter, data.currentLineIndex);
            return true;
        }

        [ContextMenu("Debug — Forcer sauvegarde")]
        private void ForceSave() => SaveGame();

        [ContextMenu("Debug — Supprimer sauvegarde")]
        private void DeleteSave() => SaveSystem.DeleteSave();

        [ContextMenu("Debug — Afficher chemin")]
        private void PrintPath() => Debug.Log(SaveSystem.GetSavePath());

        private DialogueChapter FindChapterByName(string assetName)
            => allChapters.Find(c => c.name == assetName);

        private void BuildCharacterCache()
        {
            _characterCache = new Dictionary<string, CharacterData>();
            foreach (var chapter in allChapters)
                foreach (var node in chapter.nodes)
                {
                    RegisterCharacter(node.characterOnScreen);
                    RegisterCharacter(node.line?.speaker);
                }
        }

        private void RegisterCharacter(CharacterData character)
        {
            if (character != null && !_characterCache.ContainsKey(character.name))
                _characterCache[character.name] = character;
        }

        private CharacterData FindCharacterByName(string assetName)
        {
            if (_characterCache == null) BuildCharacterCache();
            _characterCache.TryGetValue(assetName, out CharacterData result);
            return result;
        }
    }
}
