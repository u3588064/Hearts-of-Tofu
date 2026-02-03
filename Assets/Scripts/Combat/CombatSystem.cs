using UnityEngine;
using System.Collections;
using Quest2Wargame.Territory;
using Quest2Wargame.Faction;
using Quest2Wargame.Core;

namespace Quest2Wargame.Combat
{
    /// <summary>
    /// Handles combat between territories
    /// </summary>
    public class CombatSystem : MonoBehaviour
    {
        public static CombatSystem Instance { get; private set; }
        
        [Header("Combat Settings")]
        [SerializeField] private float attackTravelTime = GameConstants.ATTACK_TRAVEL_TIME;
        
        [Header("Visual Effects")]
        [SerializeField] private GameObject attackEffectPrefab;
        [SerializeField] private LineRenderer attackLinePrefab;
        
        // Events
        public System.Action<Territory.Territory, Territory.Territory, int> OnAttackStarted;
        public System.Action<Territory.Territory, Territory.Territory, AttackResult> OnAttackCompleted;
        public System.Action<Territory.Territory, FactionData> OnTerritoryConquered;
        
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
        /// Execute an attack from source to target territory
        /// </summary>
        /// <param name="source">Attacking territory</param>
        /// <param name="target">Target territory</param>
        /// <param name="attackPoints">Number of points to send</param>
        public void ExecuteAttack(Territory.Territory source, Territory.Territory target, int attackPoints)
        {
            if (!ValidateAttack(source, target, attackPoints))
            {
                Debug.LogWarning("Invalid attack!");
                return;
            }
            
            // Remove points from source
            source.RemovePoints(attackPoints);
            
            // Start attack animation/coroutine
            StartCoroutine(AttackCoroutine(source, target, attackPoints, source.Owner));
            
            OnAttackStarted?.Invoke(source, target, attackPoints);
        }
        
        /// <summary>
        /// Validate if an attack can be made
        /// </summary>
        private bool ValidateAttack(Territory.Territory source, Territory.Territory target, int attackPoints)
        {
            if (source == null || target == null) return false;
            if (!source.CanAttack(target)) return false;
            if (attackPoints < GameConstants.MIN_ATTACK_POINTS) return false;
            if (attackPoints > source.CurrentPoints) return false;
            
            return true;
        }
        
        /// <summary>
        /// Coroutine to handle attack animation and resolution
        /// </summary>
        private IEnumerator AttackCoroutine(Territory.Territory source, Territory.Territory target, 
            int attackPoints, FactionData attacker)
        {
            // Show attack visual
            if (attackLinePrefab != null)
            {
                var attackLine = Instantiate(attackLinePrefab);
                attackLine.SetPosition(0, source.transform.position);
                attackLine.SetPosition(1, target.transform.position);
                
                // Animate the line
                float elapsed = 0f;
                while (elapsed < attackTravelTime)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / attackTravelTime;
                    
                    Vector3 currentPos = Vector3.Lerp(source.transform.position, target.transform.position, t);
                    attackLine.SetPosition(1, currentPos);
                    
                    yield return null;
                }
                
                Destroy(attackLine.gameObject);
            }
            else
            {
                yield return new WaitForSeconds(attackTravelTime);
            }
            
            // Resolve combat
            AttackResult result = ResolveCombat(target, attackPoints, attacker);
            
            OnAttackCompleted?.Invoke(source, target, result);
            
            if (result.territoryConquered)
            {
                OnTerritoryConquered?.Invoke(target, attacker);
            }
        }
        
        /// <summary>
        /// Resolve combat - points cancel each other out
        /// </summary>
        private AttackResult ResolveCombat(Territory.Territory target, int attackPoints, FactionData attacker)
        {
            AttackResult result = new AttackResult();
            result.originalDefenderPoints = target.CurrentPoints;
            result.attackerPoints = attackPoints;
            
            // Points cancel out
            int defenderPoints = target.CurrentPoints;
            int remainingAttackPoints = attackPoints - defenderPoints;
            int remainingDefenderPoints = defenderPoints - attackPoints;
            
            if (remainingDefenderPoints > 0)
            {
                // Defender holds
                target.SetPoints(remainingDefenderPoints);
                result.territoryConquered = false;
                result.remainingPoints = remainingDefenderPoints;
            }
            else if (remainingAttackPoints > 0)
            {
                // Attacker conquers
                FactionData previousOwner = target.Owner;
                target.SetOwner(attacker);
                target.SetPoints(remainingAttackPoints);
                
                result.territoryConquered = true;
                result.remainingPoints = remainingAttackPoints;
                result.previousOwner = previousOwner;
                
                Debug.Log($"{attacker.factionName} conquered {target.TerritoryName}!");
                
                // Check if previous owner is now eliminated
                if (previousOwner != null)
                {
                    int remainingTerritories = TerritoryManager.Instance.CountTerritoriesByFaction(previousOwner);
                    FactionManager.Instance.CheckFactionElimination(previousOwner, remainingTerritories);
                }
            }
            else
            {
                // Tie - both sides depleted, territory stays with defender but at 0 points
                target.SetPoints(0);
                result.territoryConquered = false;
                result.remainingPoints = 0;
            }
            
            return result;
        }
        
        /// <summary>
        /// Calculate predicted result of an attack
        /// </summary>
        public AttackPrediction PredictAttack(Territory.Territory source, Territory.Territory target, int attackPoints)
        {
            var prediction = new AttackPrediction();
            prediction.canAttack = ValidateAttack(source, target, attackPoints);
            
            if (prediction.canAttack)
            {
                int defenderPoints = target.CurrentPoints;
                prediction.attackerPointsRemaining = Mathf.Max(0, attackPoints - defenderPoints);
                prediction.defenderPointsRemaining = Mathf.Max(0, defenderPoints - attackPoints);
                prediction.willConquer = attackPoints > defenderPoints;
            }
            
            return prediction;
        }
    }
    
    /// <summary>
    /// Result of a combat resolution
    /// </summary>
    public struct AttackResult
    {
        public bool territoryConquered;
        public int originalDefenderPoints;
        public int attackerPoints;
        public int remainingPoints;
        public FactionData previousOwner;
    }
    
    /// <summary>
    /// Predicted result of an attack
    /// </summary>
    public struct AttackPrediction
    {
        public bool canAttack;
        public bool willConquer;
        public int attackerPointsRemaining;
        public int defenderPointsRemaining;
    }
}
