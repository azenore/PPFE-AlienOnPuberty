using UnityEngine;
using VN.Data;

namespace VN.Runtime
{
    public class AffinitySystem : MonoBehaviour
    {
        private const int HighAffinityThreshold = 70;

        [SerializeField] private ProtagonistData protagonist;

        /// <summary>Applies affinity delta from a dialogue choice.</summary>
        public void ApplyChoiceAffinity(DialogueChoice choice)
        {
            if (choice.affinityTarget == null) return;
            protagonist.ModifyAffinity(choice.affinityTarget, choice.affinityDelta);

#if UNITY_EDITOR
            Debug.Log($"[Affinity] {choice.affinityTarget.characterName}: {protagonist.GetAffinity(choice.affinityTarget)}/100");
#endif
        }

        /// <summary>Applies a raw affinity delta to a character. Used by phone choices.</summary>
        public void ApplyDelta(CharacterData character, int delta)
        {
            if (character == null) return;
            protagonist.ModifyAffinity(character, delta);

#if UNITY_EDITOR
            Debug.Log($"[Affinity] {character.characterName}: {protagonist.GetAffinity(character)}/100");
#endif
        }

        /// <summary>Returns true if affinity with a character meets the high threshold.</summary>
        public bool IsHighAffinity(CharacterData character)
        {
            return protagonist.GetAffinity(character) >= HighAffinityThreshold;
        }
    }
}
