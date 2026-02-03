using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Quest2Wargame.Territory;
using Quest2Wargame.Faction;
using Quest2Wargame.Combat;
using Quest2Wargame.Core;

namespace Quest2Wargame.AI
{
    /// <summary>
    /// Simple AI controller for non-player factions
    /// </summary>
    public class SimpleAI : MonoBehaviour
    {
        [Header("AI Settings")]
        [SerializeField] private float decisionInterval = 3f;
        [SerializeField] private float aggressiveness = 0.6f; // 0-1, higher = more aggressive
        [SerializeField] private float reserveRatio = 0.3f; // Percentage of points to keep for defense
        
        private FactionData faction;
        private float decisionTimer;
        private bool isActive;
        
        public FactionData Faction => faction;
        
        /// <summary>
        /// Initialize AI with faction
        /// </summary>
        public void Initialize(FactionData factionData)
        {
            faction = factionData;
            isActive = true;
            decisionTimer = Random.Range(1f, decisionInterval); // Stagger AI decisions
        }
        
        /// <summary>
        /// Pause/Resume AI
        /// </summary>
        public void SetActive(bool active)
        {
            isActive = active;
        }
        
        private void Update()
        {
            if (!isActive || faction == null || faction.isEliminated)
                return;
            
            decisionTimer -= Time.deltaTime;
            if (decisionTimer <= 0)
            {
                MakeDecision();
                decisionTimer = decisionInterval + Random.Range(-0.5f, 0.5f);
            }
        }
        
        /// <summary>
        /// Main AI decision loop
        /// </summary>
        private void MakeDecision()
        {
            var myTerritories = TerritoryManager.Instance.GetTerritoriesByFaction(faction);
            if (myTerritories.Count == 0)
                return;
            
            // Find best attack opportunity
            AttackOpportunity bestAttack = FindBestAttack(myTerritories);
            
            if (bestAttack.isValid && Random.value < aggressiveness)
            {
                ExecuteAttack(bestAttack);
            }
        }
        
        /// <summary>
        /// Find the best attack opportunity
        /// </summary>
        private AttackOpportunity FindBestAttack(List<Territory.Territory> myTerritories)
        {
            AttackOpportunity best = new AttackOpportunity();
            float bestScore = float.MinValue;
            
            foreach (var territory in myTerritories)
            {
                int availablePoints = CalculateAvailableAttackPoints(territory);
                if (availablePoints < GameConstants.MIN_ATTACK_POINTS)
                    continue;
                
                var targets = TerritoryManager.Instance.GetAttackableTargets(territory);
                foreach (var target in targets)
                {
                    float score = EvaluateTarget(territory, target, availablePoints);
                    
                    if (score > bestScore)
                    {
                        bestScore = score;
                        best.source = territory;
                        best.target = target;
                        best.points = CalculateAttackPoints(territory, target, availablePoints);
                        best.isValid = true;
                    }
                }
            }
            
            return best;
        }
        
        /// <summary>
        /// Calculate how many points can be used for attack (keeping reserve)
        /// </summary>
        private int CalculateAvailableAttackPoints(Territory.Territory territory)
        {
            int reserve = Mathf.CeilToInt(territory.MaxPoints * reserveRatio);
            return Mathf.Max(0, territory.CurrentPoints - reserve);
        }
        
        /// <summary>
        /// Calculate optimal attack points for a target
        /// </summary>
        private int CalculateAttackPoints(Territory.Territory source, Territory.Territory target, int available)
        {
            // Attack with enough to conquer plus some overflow
            int neededToConquer = target.CurrentPoints + 10;
            return Mathf.Min(available, neededToConquer);
        }
        
        /// <summary>
        /// Evaluate how good a target is to attack
        /// </summary>
        private float EvaluateTarget(Territory.Territory source, Territory.Territory target, int availablePoints)
        {
            float score = 0f;
            
            // Prefer targets with fewer points (easier to take)
            float pointRatio = (float)availablePoints / Mathf.Max(1, target.CurrentPoints);
            score += pointRatio * 50f;
            
            // Can we actually conquer it?
            if (availablePoints > target.CurrentPoints)
            {
                score += 100f;
            }
            
            // Prefer neutral territories (no faction) over enemy factions
            if (target.IsNeutral)
            {
                score += 30f;
            }
            
            // Prefer territories with high max points (strategic value)
            score += target.MaxPoints * 0.2f;
            
            // Slight randomness
            score += Random.Range(-10f, 10f);
            
            return score;
        }
        
        /// <summary>
        /// Execute the attack
        /// </summary>
        private void ExecuteAttack(AttackOpportunity attack)
        {
            if (attack.source == null || attack.target == null)
                return;
            
            Debug.Log($"AI {faction.factionName} attacks {attack.target.TerritoryName} from {attack.source.TerritoryName} with {attack.points} points");
            
            CombatSystem.Instance.ExecuteAttack(attack.source, attack.target, attack.points);
        }
        
        /// <summary>
        /// Data structure for attack opportunities
        /// </summary>
        private struct AttackOpportunity
        {
            public bool isValid;
            public Territory.Territory source;
            public Territory.Territory target;
            public int points;
        }
    }
}
