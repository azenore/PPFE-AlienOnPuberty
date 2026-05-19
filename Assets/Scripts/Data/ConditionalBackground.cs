using System;
using System.Collections.Generic;
using UnityEngine;

namespace VN.Data
{
    /// <summary>
    /// Maps protagonist appearance combinations (hairId / eyeId) to background sprites.
    /// Leave hairId or eyeId empty to match any value for that attribute.
    /// Conditions are evaluated top-to-bottom — the first match wins.
    /// </summary>
    [CreateAssetMenu(menuName = "VN/Conditional Background", fileName = "NewConditionalBackground")]
    public class ConditionalBackground : ScriptableObject
    {
        [Serializable]
        public class BackgroundCondition
        {
            [Tooltip("Identifiant de coiffure requis. Laisser vide pour ignorer cette condition.")]
            public string hairId;

            [Tooltip("Identifiant de couleur des yeux requis. Laisser vide pour ignorer cette condition.")]
            public string eyeId;

            public Sprite background;
        }

        [Tooltip("Conditions évaluées dans l'ordre. La première correspondance est utilisée.")]
        public List<BackgroundCondition> conditions = new();

        [Tooltip("Fond utilisé si aucune condition ne correspond.")]
        public Sprite fallbackBackground;

        /// <summary>Returns the first matching background sprite, or the fallback if none match.</summary>
        public Sprite Evaluate(ProtagonistData protagonist)
        {
            if (protagonist == null) return fallbackBackground;

            foreach (var condition in conditions)
            {
                bool hairMatch = string.IsNullOrEmpty(condition.hairId) || condition.hairId == protagonist.hairId;
                bool eyeMatch  = string.IsNullOrEmpty(condition.eyeId)  || condition.eyeId  == protagonist.eyeId;

                if (hairMatch && eyeMatch)
                    return condition.background;
            }

            return fallbackBackground;
        }
    }
}
