using System.Collections.Generic;
using UnityEngine;
using VN.Data;
using VN.Runtime;

namespace VN.UI
{
    public class ChoiceMenuController : MonoBehaviour
    {
        [SerializeField] private DialogueEngine engine;
        [SerializeField] private GameObject choiceMenu;
        [SerializeField] private ChoiceButtonView choiceButtonPrefab;
        [SerializeField] private Transform buttonsContainer;

        // Pool de boutons réutilisés — plus de Instantiate/Destroy à chaque choix
        private readonly List<ChoiceButtonView> _pool = new();

        private void OnEnable()
        {
            engine.OnChoiceReady += ShowChoices;
            engine.OnLineReady += HandleLineReady;
            engine.OnChapterFinished += HandleChapterFinished;
        }

        private void OnDisable()
        {
            engine.OnChoiceReady -= ShowChoices;
            engine.OnLineReady -= HandleLineReady;
            engine.OnChapterFinished -= HandleChapterFinished;
        }

        private void HandleLineReady(DialogueLine _) => HideChoices();
        private void HandleChapterFinished(DialogueChapter _) => HideChoices();

        private void ShowChoices(List<DialogueChoice> choices)
        {
            HideChoices();
            choiceMenu.SetActive(true);

            // Instancie uniquement si le pool n'a pas assez de boutons
            while (_pool.Count < choices.Count)
                _pool.Add(Instantiate(choiceButtonPrefab, buttonsContainer));

            for (int i = 0; i < _pool.Count; i++)
            {
                bool active = i < choices.Count;
                _pool[i].gameObject.SetActive(active);
                if (active)
                    _pool[i].Setup(choices[i], engine);
            }
        }

        private void HideChoices()
        {
            foreach (var button in _pool)
                button.gameObject.SetActive(false);

            choiceMenu.SetActive(false);
        }
    }
}
