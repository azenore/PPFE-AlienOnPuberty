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
        [SerializeField] private PhoneEngine phoneEngine;

        [Tooltip("Tous les DialogueChapter du jeu, dans l'ordre.")]
        [SerializeField] private List<DialogueChapter> allChapters;

        [Tooltip("Tous les PhoneChapter du jeu.")]
        [SerializeField] private List<PhoneChapter> allPhoneChapters;

        /// <summary>Fired each time a save is successfully written to disk.</summary>
        public event Action OnSaved;

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
            bool hasDialogue = !string.IsNullOrEmpty(chapterManager.CurrentChapterName);
            bool hasPhone = chapterManager.IsInPhoneChapter &&
                            !string.IsNullOrEmpty(chapterManager.CurrentPhoneChapterName);

            if (!hasDialogue && !hasPhone) return;
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

            if (chapterManager.IsInPhoneChapter)
            {
                data.currentPhoneChapterName = chapterManager.CurrentPhoneChapterName;
                data.currentPhoneMessageIndex = phoneEngine.LastRevealedIndex;
                data.currentPhoneChoiceMessageIndex = phoneEngine.ChoiceMessageIndex;
                data.currentPhoneChoiceIndex = phoneEngine.SelectedChoiceIndex;
            }

            foreach (var (character, value) in ProtagonistData.GetAllAffinities())
            {
                data.affinities.Add(new AffinitySaveEntry
                {
                    characterName = character.name,
                    value = value,
                });
            }

            SaveSystem.Save(data);
            Debug.Log($"[Save] Chapitre : {data.currentChapterName} | Phone : {data.currentPhoneChapterName} | Message : {data.currentPhoneMessageIndex} | Choix : {data.currentPhoneChoiceMessageIndex}/{data.currentPhoneChoiceIndex}");
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

            // Restaure le phone chapter en priorité s'il est défini
            if (!string.IsNullOrEmpty(data.currentPhoneChapterName))
            {
                PhoneChapter phoneChapter = FindPhoneChapterByName(data.currentPhoneChapterName);
                if (phoneChapter == null)
                {
                    Debug.LogError($"[Save] PhoneChapter introuvable : '{data.currentPhoneChapterName}'.");
                    return false;
                }

                // Mémorise silencieusement le dialogue chapter sans déclencher le moteur
                if (!string.IsNullOrEmpty(data.currentChapterName))
                {
                    DialogueChapter dialogueCh = FindChapterByName(data.currentChapterName);
                    if (dialogueCh != null)
                        chapterManager.SetCurrentChapterSilent(dialogueCh);
                }

                // Si un choix avait été fait, on restaure via RestoreAfterChoice
                if (data.currentPhoneChoiceIndex >= 0 && data.currentPhoneChoiceMessageIndex >= 0)
                {
                    Debug.Log($"[Save] Restore phone avec choix → message {data.currentPhoneChoiceMessageIndex}, choix {data.currentPhoneChoiceIndex}");
                    chapterManager.LoadPhoneChapterAfterChoice(
                        phoneChapter,
                        data.currentPhoneChoiceMessageIndex,
                        data.currentPhoneChoiceIndex
                    );
                }
                else
                {
                    Debug.Log($"[Save] Restore phone → {data.currentPhoneChapterName} | Message : {data.currentPhoneMessageIndex}");
                    chapterManager.LoadPhoneChapterAtMessage(phoneChapter, data.currentPhoneMessageIndex);
                }

                return true;
            }

            // Sinon restaure le chapitre dialogue normalement
            DialogueChapter chapter = FindChapterByName(data.currentChapterName);
            if (chapter == null)
            {
                Debug.LogError($"[Save] Chapitre introuvable : '{data.currentChapterName}'.");
                return false;
            }

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

        private PhoneChapter FindPhoneChapterByName(string assetName)
            => allPhoneChapters.Find(c => c.name == assetName);

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
