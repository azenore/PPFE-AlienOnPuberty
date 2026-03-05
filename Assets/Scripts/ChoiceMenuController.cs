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

        private readonly List<ChoiceButtonView> _activeButtons = new();

        private void OnEnable()
        {
            engine.OnChoiceReady += ShowChoices;
            engine.OnChapterFinished += _ => HideChoices();
            engine.OnLineReady += _ => HideChoices();
        }

        private void OnDisable()
        {
            engine.OnChoiceReady -= ShowChoices;
            engine.OnChapterFinished -= _ => HideChoices();
            engine.OnLineReady -= _ => HideChoices();
        }

        private void ShowChoices(List<DialogueChoice> choices)
        {
            HideChoices();
            choiceMenu.SetActive(true);

            foreach (var choice in choices)
            {
                var button = Instantiate(choiceButtonPrefab, buttonsContainer);
                button.Setup(choice, engine);
                _activeButtons.Add(button);
            }
        }

        private void HideChoices()
        {
            foreach (var button in _activeButtons)
                Destroy(button.gameObject);

            _activeButtons.Clear();
            choiceMenu.SetActive(false);
        }
    }
}
