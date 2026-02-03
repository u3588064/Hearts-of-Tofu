using UnityEngine;

namespace Quest2Wargame.Core
{
    /// <summary>
    /// Game state enumeration for managing game flow
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Setup,
        Playing,
        Paused,
        Victory
    }

    /// <summary>
    /// Game configuration constants
    /// </summary>
    public static class GameConstants
    {
        // Territory settings
        public const float DEFAULT_POINTS_PER_SECOND = 1f;
        public const int DEFAULT_MAX_POINTS = 100;
        public const int DEFAULT_STARTING_POINTS = 20;
        
        // Combat settings
        public const float ATTACK_TRAVEL_TIME = 1.5f;
        public const int MIN_ATTACK_POINTS = 5;
        
        // Faction settings
        public const int MIN_FACTIONS = 2;
        public const int MAX_FACTIONS = 6;
        
        // VR settings
        public const float PINCH_THRESHOLD = 0.7f;
        public const float SELECTION_DISTANCE = 0.1f;
    }
}
