using System;
using UnityEngine;

namespace Tools.SaveLoad.Sample
{
    [SelectionBase]
    public class Player : MonoBehaviour, IBind<PlayerData>
    {
        [SerializeField] private PlayerData data;


        public int HashCode { get; set; }
        public void Bind(PlayerData data)
        {
            this.data = data;
            this.data.HashCode = GetHashCode();
            transform.position = data.Position;
            transform.rotation = data.Rotation;
        }

        public PlayerData GetData()
        {
            return new PlayerData()
            {
                Position = transform.position,
                Rotation = transform.rotation,
            };
        }
    }
}