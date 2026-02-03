using UnityEngine;
using System;
using System.Collections.Generic;
using Quest2Wargame.Faction;
using Quest2Wargame.Core;

namespace Quest2Wargame.Territory
{
    /// <summary>
    /// Represents a single territory on the world map
    /// </summary>
    public class Territory : MonoBehaviour
    {
        [Header("Territory Info")]
        [SerializeField] private string territoryId;
        [SerializeField] private string territoryName;
        [SerializeField] private string regionName;
        
        [Header("Points Configuration")]
        [SerializeField] private int currentPoints;
        [SerializeField] private int maxPoints = GameConstants.DEFAULT_MAX_POINTS;
        [SerializeField] private float pointsPerSecond = GameConstants.DEFAULT_POINTS_PER_SECOND;
        
        [Header("Connections")]
        [SerializeField] private List<Territory> neighbors = new List<Territory>();
        
        [Header("Visuals")]
        [SerializeField] private MeshRenderer territoryRenderer;
        [SerializeField] private TextMesh pointsText;
        
        // Current owner
        private FactionData owner;
        private Material territoryMaterial;
        
        // Properties
        public string TerritoryId => territoryId;
        public string TerritoryName => territoryName;
        public string RegionName => regionName;
        public int CurrentPoints => currentPoints;
        public int MaxPoints => maxPoints;
        public FactionData Owner => owner;
        public List<Territory> Neighbors => neighbors;
        public bool IsNeutral => owner == null;
        
        // Events
        public Action<Territory, FactionData, FactionData> OnOwnerChanged;
        public Action<Territory, int> OnPointsChanged;
        
        private void Awake()
        {
            if (territoryRenderer != null)
            {
                territoryMaterial = new Material(territoryRenderer.material);
                territoryRenderer.material = territoryMaterial;
            }
        }
        
        private void Update()
        {
            // Auto-generate points over time
            if (owner != null && currentPoints < maxPoints)
            {
                AutoGeneratePoints();
            }
        }
        
        private float pointAccumulator = 0f;
        
        private void AutoGeneratePoints()
        {
            pointAccumulator += pointsPerSecond * Time.deltaTime;
            
            if (pointAccumulator >= 1f)
            {
                int pointsToAdd = Mathf.FloorToInt(pointAccumulator);
                pointAccumulator -= pointsToAdd;
                
                AddPoints(pointsToAdd);
            }
        }
        
        /// <summary>
        /// Initialize territory with starting values
        /// </summary>
        public void Initialize(string id, string name, string region, int startingPoints = 0)
        {
            territoryId = id;
            territoryName = name;
            regionName = region;
            currentPoints = startingPoints > 0 ? startingPoints : GameConstants.DEFAULT_STARTING_POINTS;
            UpdateVisuals();
        }
        
        /// <summary>
        /// Set the owner of this territory
        /// </summary>
        public void SetOwner(FactionData newOwner)
        {
            var previousOwner = owner;
            owner = newOwner;
            
            UpdateVisuals();
            OnOwnerChanged?.Invoke(this, previousOwner, newOwner);
        }
        
        /// <summary>
        /// Add points to this territory
        /// </summary>
        public void AddPoints(int amount)
        {
            int previousPoints = currentPoints;
            currentPoints = Mathf.Clamp(currentPoints + amount, 0, maxPoints);
            
            if (currentPoints != previousPoints)
            {
                UpdatePointsDisplay();
                OnPointsChanged?.Invoke(this, currentPoints);
            }
        }
        
        /// <summary>
        /// Remove points from this territory
        /// </summary>
        /// <returns>Actual points removed</returns>
        public int RemovePoints(int amount)
        {
            int actualRemoved = Mathf.Min(currentPoints, amount);
            currentPoints -= actualRemoved;
            
            UpdatePointsDisplay();
            OnPointsChanged?.Invoke(this, currentPoints);
            
            return actualRemoved;
        }
        
        /// <summary>
        /// Set points directly (used when capturing territory)
        /// </summary>
        public void SetPoints(int amount)
        {
            currentPoints = Mathf.Clamp(amount, 0, maxPoints);
            UpdatePointsDisplay();
            OnPointsChanged?.Invoke(this, currentPoints);
        }
        
        /// <summary>
        /// Check if this territory can attack another
        /// </summary>
        public bool CanAttack(Territory target)
        {
            if (target == null || target == this) return false;
            if (owner == null) return false;
            if (target.Owner == owner) return false;
            if (currentPoints < GameConstants.MIN_ATTACK_POINTS) return false;
            if (!neighbors.Contains(target)) return false;
            
            return true;
        }
        
        /// <summary>
        /// Add a neighbor connection
        /// </summary>
        public void AddNeighbor(Territory neighbor)
        {
            if (neighbor != null && !neighbors.Contains(neighbor))
            {
                neighbors.Add(neighbor);
            }
        }
        
        /// <summary>
        /// Update visual representation
        /// </summary>
        private void UpdateVisuals()
        {
            if (territoryMaterial != null)
            {
                if (owner != null)
                {
                    territoryMaterial.color = owner.factionColor;
                }
                else
                {
                    territoryMaterial.color = Color.gray; // Neutral color
                }
            }
            
            UpdatePointsDisplay();
        }
        
        /// <summary>
        /// Update points text display
        /// </summary>
        private void UpdatePointsDisplay()
        {
            if (pointsText != null)
            {
                pointsText.text = currentPoints.ToString();
            }
        }
        
        /// <summary>
        /// Visual feedback when selected
        /// </summary>
        public void SetSelected(bool selected)
        {
            if (territoryMaterial != null)
            {
                if (selected)
                {
                    territoryMaterial.SetFloat("_OutlineWidth", 0.02f);
                }
                else
                {
                    territoryMaterial.SetFloat("_OutlineWidth", 0f);
                }
            }
        }
        
        /// <summary>
        /// Visual feedback when targeted for attack
        /// </summary>
        public void SetTargeted(bool targeted)
        {
            if (territoryMaterial != null)
            {
                if (targeted)
                {
                    territoryMaterial.SetColor("_OutlineColor", Color.red);
                    territoryMaterial.SetFloat("_OutlineWidth", 0.03f);
                }
                else
                {
                    territoryMaterial.SetFloat("_OutlineWidth", 0f);
                }
            }
        }
    }
}
