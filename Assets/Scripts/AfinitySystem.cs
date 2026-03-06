using UnityEngine;
using VN.Data;

namespace VN.Runtime
{
    public class AffinitySystem : MonoBehaviour
    {
        private const int HighAffinityThreshold = 70;

        [SerializeField] private ProtagonistData protagonist;

        /// <summary>Applies affinity delta from a choice.</summary>
        public void ApplyChoiceAffinity(DialogueChoice choice)
        {
            if (choice.affinityTarget == null) return;

            protagonist.ModifyAffinity(choice.affinityTarget, choice.affinityDelta);

#if UNITY_EDITOR
            int current = protagonist.GetAffinity(choice.affinityTarget);
            Debug.Log($"[Affinity] {choice.affinityTarget.characterName}: {current}/100");
#endif
        }

        /// <summary>Returns true if affinity with a character meets the high threshold.</summary>
        public bool IsHighAffinity(CharacterData character)
        {
            return protagonist.GetAffinity(character) >= HighAffinityThreshold;
        }
    }
}
