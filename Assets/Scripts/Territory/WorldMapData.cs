using UnityEngine;
using System;
using System.Collections.Generic;

namespace Quest2Wargame.Territory
{
    /// <summary>
    /// Definition for a single territory in the world map
    /// </summary>
    [Serializable]
    public class TerritoryDefinition
    {
        public string territoryId;
        public string territoryName;
        public string regionName;
        public Vector3 position;
        public Vector3 scale = Vector3.one;
        public int startingPoints = 20;
        public int maxPoints = 100;
        public List<string> neighborIds = new List<string>();
    }
    
    /// <summary>
    /// ScriptableObject containing world map data
    /// </summary>
    [CreateAssetMenu(fileName = "WorldMapData", menuName = "Quest2Wargame/World Map Data")]
    public class WorldMapData : ScriptableObject
    {
        [Header("Map Info")]
        public string mapName = "World Map";
        
        [Header("Territories")]
        public List<TerritoryDefinition> regions = new List<TerritoryDefinition>();
        
        /// <summary>
        /// Create default world map with major regions
        /// </summary>
        public void CreateDefaultWorldMap()
        {
            regions.Clear();
            
            // North America
            AddTerritory("alaska", "阿拉斯加", "北美", new Vector3(-3f, 0, 2f), new[] { "canada", "siberia" });
            AddTerritory("canada", "加拿大", "北美", new Vector3(-2f, 0, 2f), new[] { "alaska", "usa_west", "usa_east", "greenland" });
            AddTerritory("greenland", "格陵兰", "北美", new Vector3(-0.5f, 0, 2.5f), new[] { "canada", "north_europe" });
            AddTerritory("usa_west", "美国西部", "北美", new Vector3(-2.5f, 0, 1f), new[] { "canada", "usa_east", "mexico" });
            AddTerritory("usa_east", "美国东部", "北美", new Vector3(-1.5f, 0, 1f), new[] { "canada", "usa_west", "mexico", "caribbean" });
            AddTerritory("mexico", "墨西哥", "北美", new Vector3(-2f, 0, 0.2f), new[] { "usa_west", "usa_east", "central_america" });
            AddTerritory("central_america", "中美洲", "北美", new Vector3(-1.5f, 0, -0.3f), new[] { "mexico", "caribbean", "colombia" });
            AddTerritory("caribbean", "加勒比", "北美", new Vector3(-1f, 0, 0.2f), new[] { "usa_east", "central_america", "colombia" });
            
            // South America
            AddTerritory("colombia", "哥伦比亚", "南美", new Vector3(-1.3f, 0, -0.8f), new[] { "central_america", "caribbean", "brazil", "andes" });
            AddTerritory("brazil", "巴西", "南美", new Vector3(-0.8f, 0, -1.5f), new[] { "colombia", "andes", "south_cone" });
            AddTerritory("andes", "安第斯", "南美", new Vector3(-1.5f, 0, -1.5f), new[] { "colombia", "brazil", "south_cone" });
            AddTerritory("south_cone", "南锥体", "南美", new Vector3(-1.2f, 0, -2.2f), new[] { "brazil", "andes" });
            
            // Europe
            AddTerritory("north_europe", "北欧", "欧洲", new Vector3(0.5f, 0, 2.2f), new[] { "greenland", "west_europe", "central_europe", "russia_west" });
            AddTerritory("west_europe", "西欧", "欧洲", new Vector3(0.3f, 0, 1.5f), new[] { "north_europe", "iberia", "central_europe", "north_africa" });
            AddTerritory("iberia", "伊比利亚", "欧洲", new Vector3(-0.2f, 0, 1.2f), new[] { "west_europe", "north_africa" });
            AddTerritory("central_europe", "中欧", "欧洲", new Vector3(0.7f, 0, 1.5f), new[] { "north_europe", "west_europe", "italy", "east_europe", "balkans" });
            AddTerritory("italy", "意大利", "欧洲", new Vector3(0.6f, 0, 1.2f), new[] { "central_europe", "balkans", "north_africa" });
            AddTerritory("east_europe", "东欧", "欧洲", new Vector3(1f, 0, 1.7f), new[] { "central_europe", "balkans", "russia_west" });
            AddTerritory("balkans", "巴尔干", "欧洲", new Vector3(0.9f, 0, 1.2f), new[] { "central_europe", "italy", "east_europe", "middle_east" });
            AddTerritory("russia_west", "俄罗斯西部", "欧洲", new Vector3(1.5f, 0, 2f), new[] { "north_europe", "east_europe", "central_asia", "siberia" });
            
            // Asia
            AddTerritory("siberia", "西伯利亚", "亚洲", new Vector3(2.5f, 0, 2.3f), new[] { "alaska", "russia_west", "central_asia", "china_north", "far_east" });
            AddTerritory("central_asia", "中亚", "亚洲", new Vector3(2f, 0, 1.2f), new[] { "russia_west", "siberia", "middle_east", "south_asia", "china_north" });
            AddTerritory("middle_east", "中东", "亚洲", new Vector3(1.3f, 0, 0.8f), new[] { "balkans", "central_asia", "south_asia", "north_africa", "east_africa" });
            AddTerritory("south_asia", "南亚", "亚洲", new Vector3(2f, 0, 0.3f), new[] { "central_asia", "middle_east", "southeast_asia" });
            AddTerritory("china_north", "中国北部", "亚洲", new Vector3(2.8f, 0, 1f), new[] { "siberia", "central_asia", "south_asia", "southeast_asia", "japan_korea", "far_east" });
            AddTerritory("far_east", "远东", "亚洲", new Vector3(3.3f, 0, 1.8f), new[] { "siberia", "china_north", "japan_korea" });
            AddTerritory("japan_korea", "日韩", "亚洲", new Vector3(3.5f, 0, 1.2f), new[] { "far_east", "china_north" });
            AddTerritory("southeast_asia", "东南亚", "亚洲", new Vector3(2.8f, 0, -0.2f), new[] { "south_asia", "china_north", "australia" });
            
            // Africa
            AddTerritory("north_africa", "北非", "非洲", new Vector3(0.5f, 0, 0.5f), new[] { "iberia", "west_europe", "italy", "middle_east", "west_africa", "east_africa" });
            AddTerritory("west_africa", "西非", "非洲", new Vector3(0.2f, 0, -0.3f), new[] { "north_africa", "east_africa", "south_africa" });
            AddTerritory("east_africa", "东非", "非洲", new Vector3(1f, 0, -0.5f), new[] { "north_africa", "middle_east", "west_africa", "south_africa" });
            AddTerritory("south_africa", "南非", "非洲", new Vector3(0.7f, 0, -1.5f), new[] { "west_africa", "east_africa" });
            
            // Oceania
            AddTerritory("australia", "澳大利亚", "大洋洲", new Vector3(3.2f, 0, -1.2f), new[] { "southeast_asia", "pacific" });
            AddTerritory("pacific", "太平洋岛屿", "大洋洲", new Vector3(3.8f, 0, -0.5f), new[] { "australia", "japan_korea" });
            
            Debug.Log($"Created default world map with {regions.Count} territories");
        }
        
        private void AddTerritory(string id, string name, string region, Vector3 pos, string[] neighbors)
        {
            regions.Add(new TerritoryDefinition
            {
                territoryId = id,
                territoryName = name,
                regionName = region,
                position = pos,
                scale = new Vector3(0.3f, 0.1f, 0.3f),
                startingPoints = 20,
                maxPoints = 100,
                neighborIds = new List<string>(neighbors)
            });
        }
    }
}
