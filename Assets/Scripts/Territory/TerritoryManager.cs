using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Quest2Wargame.Faction;
using Quest2Wargame.Core;

namespace Quest2Wargame.Territory
{
    /// <summary>
    /// Manages all territories and the world map
    /// </summary>
    public class TerritoryManager : MonoBehaviour
    {
        public static TerritoryManager Instance { get; private set; }
        
        [Header("Map Configuration")]
        [SerializeField] private Transform mapParent;
        [SerializeField] private GameObject territoryPrefab;
        
        [Header("World Map Data")]
        [SerializeField] private WorldMapData worldMapData;
        
        private Dictionary<string, Territory> territories = new Dictionary<string, Territory>();
        
        public Dictionary<string, Territory> Territories => territories;
        
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
        /// Generate the world map from data
        /// </summary>
        public void GenerateMap()
        {
            if (worldMapData == null)
            {
                Debug.LogError("WorldMapData not assigned!");
                return;
            }
            
            ClearMap();
            
            foreach (var regionData in worldMapData.regions)
            {
                CreateTerritory(regionData);
            }
            
            // Setup neighbor connections
            SetupNeighborConnections();
            
            Debug.Log($"Generated map with {territories.Count} territories");
        }
        
        /// <summary>
        /// Create a single territory from data
        /// </summary>
        private void CreateTerritory(TerritoryDefinition data)
        {
            if (territoryPrefab == null)
            {
                Debug.LogError("Territory prefab not assigned!");
                return;
            }
            
            GameObject territoryObj = Instantiate(territoryPrefab, mapParent);
            territoryObj.name = data.territoryName;
            territoryObj.transform.localPosition = data.position;
            territoryObj.transform.localScale = data.scale;
            
            Territory territory = territoryObj.GetComponent<Territory>();
            if (territory != null)
            {
                territory.Initialize(data.territoryId, data.territoryName, data.regionName, data.startingPoints);
                territories[data.territoryId] = territory;
            }
        }
        
        /// <summary>
        /// Setup neighbor connections between territories
        /// </summary>
        private void SetupNeighborConnections()
        {
            foreach (var regionData in worldMapData.regions)
            {
                if (!territories.TryGetValue(regionData.territoryId, out Territory territory))
                    continue;
                
                foreach (string neighborId in regionData.neighborIds)
                {
                    if (territories.TryGetValue(neighborId, out Territory neighbor))
                    {
                        territory.AddNeighbor(neighbor);
                    }
                }
            }
        }
        
        /// <summary>
        /// Clear all territories
        /// </summary>
        public void ClearMap()
        {
            foreach (var territory in territories.Values)
            {
                if (territory != null)
                {
                    Destroy(territory.gameObject);
                }
            }
            territories.Clear();
        }
        
        /// <summary>
        /// Assign initial territories to factions
        /// </summary>
        public void AssignInitialTerritories(List<FactionData> factions)
        {
            var territoryList = territories.Values.ToList();
            int territoriesPerFaction = territoryList.Count / factions.Count;
            int index = 0;
            
            foreach (var faction in factions)
            {
                int assigned = 0;
                while (assigned < territoriesPerFaction && index < territoryList.Count)
                {
                    territoryList[index].SetOwner(faction);
                    index++;
                    assigned++;
                }
            }
            
            // Remaining territories stay neutral or assign to random factions
            while (index < territoryList.Count)
            {
                // Leave as neutral or assign to random faction
                territoryList[index].SetOwner(null);
                index++;
            }
            
            Debug.Log($"Assigned territories to {factions.Count} factions");
        }
        
        /// <summary>
        /// Get territory by ID
        /// </summary>
        public Territory GetTerritory(string territoryId)
        {
            territories.TryGetValue(territoryId, out Territory territory);
            return territory;
        }
        
        /// <summary>
        /// Get all territories owned by a faction
        /// </summary>
        public List<Territory> GetTerritoriesByFaction(FactionData faction)
        {
            return territories.Values.Where(t => t.Owner == faction).ToList();
        }
        
        /// <summary>
        /// Count territories owned by a faction
        /// </summary>
        public int CountTerritoriesByFaction(FactionData faction)
        {
            return territories.Values.Count(t => t.Owner == faction);
        }
        
        /// <summary>
        /// Get all territories that can attack from a source
        /// </summary>
        public List<Territory> GetAttackableTargets(Territory source)
        {
            if (source == null || source.Owner == null)
                return new List<Territory>();
            
            return source.Neighbors.Where(n => source.CanAttack(n)).ToList();
        }
    }
}
