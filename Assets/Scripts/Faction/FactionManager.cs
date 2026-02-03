using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Quest2Wargame.Faction
{
    /// <summary>
    /// Manages all factions in the game
    /// </summary>
    public class FactionManager : MonoBehaviour
    {
        public static FactionManager Instance { get; private set; }
        
        [Header("Faction Colors")]
        [SerializeField] private Color[] factionColors = new Color[]
        {
            new Color(0.2f, 0.4f, 0.8f, 1f),    // Blue - Player
            new Color(0.8f, 0.2f, 0.2f, 1f),    // Red
            new Color(0.2f, 0.7f, 0.3f, 1f),    // Green
            new Color(0.9f, 0.7f, 0.1f, 1f),    // Yellow
            new Color(0.6f, 0.2f, 0.6f, 1f),    // Purple
            new Color(0.9f, 0.5f, 0.1f, 1f)     // Orange
        };
        
        [SerializeField] private string[] factionNames = new string[]
        {
            "联盟", "帝国", "共和", "联邦", "王国", "自由军"
        };
        
        private List<FactionData> factions = new List<FactionData>();
        
        public List<FactionData> Factions => factions;
        public FactionData PlayerFaction => factions.FirstOrDefault(f => f.isPlayer);
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Initialize factions for a new game
        /// </summary>
        /// <param name="factionCount">Number of factions (2-6)</param>
        public void InitializeFactions(int factionCount)
        {
            factions.Clear();
            factionCount = Mathf.Clamp(factionCount, 2, 6);
            
            for (int i = 0; i < factionCount; i++)
            {
                var faction = new FactionData(
                    $"faction_{i}",
                    factionNames[i],
                    factionColors[i],
                    i == 0 // First faction is player
                );
                factions.Add(faction);
            }
            
            Debug.Log($"Initialized {factionCount} factions");
        }
        
        /// <summary>
        /// Get faction by ID
        /// </summary>
        public FactionData GetFaction(string factionId)
        {
            return factions.FirstOrDefault(f => f.factionId == factionId);
        }
        
        /// <summary>
        /// Get faction by index
        /// </summary>
        public FactionData GetFactionByIndex(int index)
        {
            if (index >= 0 && index < factions.Count)
                return factions[index];
            return null;
        }
        
        /// <summary>
        /// Check if a faction is eliminated (owns no territories)
        /// </summary>
        public void CheckFactionElimination(FactionData faction, int territoriesOwned)
        {
            if (territoriesOwned == 0 && !faction.isEliminated)
            {
                faction.isEliminated = true;
                Debug.Log($"Faction {faction.factionName} has been eliminated!");
                OnFactionEliminated?.Invoke(faction);
            }
        }
        
        /// <summary>
        /// Get list of active (non-eliminated) factions
        /// </summary>
        public List<FactionData> GetActiveFactions()
        {
            return factions.Where(f => !f.isEliminated).ToList();
        }
        
        /// <summary>
        /// Check if only one faction remains (victory condition)
        /// </summary>
        public bool CheckVictoryCondition(out FactionData winner)
        {
            var activeFactions = GetActiveFactions();
            if (activeFactions.Count == 1)
            {
                winner = activeFactions[0];
                return true;
            }
            winner = null;
            return false;
        }
        
        // Events
        public System.Action<FactionData> OnFactionEliminated;
    }
}
