using System.Collections.Generic;
using UnityEngine;
using VN.Data;
using VN.Runtime;

namespace VN.UI
{
    public class PhoneChoiceMenuController : MonoBehaviour
    {
        [SerializeField] private PhoneEngine phoneEngine;
        [SerializeField] private AffinitySystem affinitySystem;
        [SerializeField] private GameObject choiceMenu;
        [SerializeField] private PhoneChoiceButtonView choiceButtonPrefab;
        [SerializeField] private Transform buttonsContainer;

        private readonly List<PhoneChoiceButtonView> _pool = new();

        private void OnEnable()
        {
            phoneEngine.OnChoiceReady += ShowChoices;
            phoneEngine.OnMessageReady += HandleMessageReady;
        }

        private void OnDisable()
        {
            phoneEngine.OnChoiceReady -= ShowChoices;
            phoneEngine.OnMessageReady -= HandleMessageReady;
        }

        private void HandleMessageReady(PhoneMessage _) => HideChoices();

        private void ShowChoices(List<PhoneChoice> choices)
        {
            HideChoices();
            choiceMenu.SetActive(true);

            while (_pool.Count < choices.Count)
                _pool.Add(Instantiate(choiceButtonPrefab, buttonsContainer));

            for (int i = 0; i < _pool.Count; i++)
            {
                bool active = i < choices.Count;
                _pool[i].gameObject.SetActive(active);
                if (active)
                    _pool[i].Setup(choices[i], phoneEngine, affinitySystem);
            }
        }

        private void HideChoices()
        {
            foreach (var btn in _pool)
                btn.gameObject.SetActive(false);
            choiceMenu.SetActive(false);
        }
    }
}
