using UnityEngine;
using System;

namespace Quest2Wargame.Faction
{
    /// <summary>
    /// Represents a faction/player in the game
    /// </summary>
    [Serializable]
    public class FactionData
    {
        public string factionId;
        public string factionName;
        public Color factionColor;
        public bool isPlayer;
        public bool isEliminated;
        
        public FactionData(string id, string name, Color color, bool player = false)
        {
            factionId = id;
            factionName = name;
            factionColor = color;
            isPlayer = player;
            isEliminated = false;
        }
    }
}
