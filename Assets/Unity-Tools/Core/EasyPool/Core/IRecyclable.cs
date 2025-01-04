﻿namespace Tools.EasyPoolKit
{
    public interface IRecyclable
    {
        RecycleObjectType ObjectType { get; }
        
        string PoolId { get; set; }

        int ObjectId { get; set; }

        string Name { get; set; }

        float UsedTime { get; set; }

        void OnObjectInit();

        void OnObjectDestroyInit();

        void OnObjectSpawn();

        void OnObjectDespawn();

        void OnObjectUpdate(float deltaTime);
    }
}