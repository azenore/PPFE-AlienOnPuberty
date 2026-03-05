using UnityEngine;
using UnityEngine.InputSystem;
using VN.Runtime;

namespace VN.UI
{
    public class AvanceGame : MonoBehaviour
    {
        [SerializeField] private DialogueEngine engine;

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
                engine.Advance();
        }
    }
}
