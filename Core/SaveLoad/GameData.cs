using UnityEngine;
using UnityEngine.Serialization;

namespace Tools.SaveLoad
{
    public interface ISaveable  {
        int HashCode { get; set; }
    }
    
    public interface IBind<TData> where TData : ISaveable {
        int HashCode { get; set; }
        void Bind(TData data);
        TData GetData();
    }
    
    [System.Serializable]
    public class GameData
    {
        public string Name;
        public string CurrentSceneName;
        public PlayerData PlayerData;
        public InventoryData InventoryData;
    }

    [System.Serializable]
    public class PlayerData : ISaveable
    {
        public int HashCode { get; set; }
        public Vector3 Position;
        public Quaternion Rotation;
    }

    [System.Serializable]
    public class InventoryData : ISaveable
    {
        public int HashCode { get; set; }
    }
}