using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tools.EasyPoolKit.Editor
{
    [CustomEditor(typeof(SimpleGOPoolKit))]
    public class SimpleGOPoolManagerInspector : UnityEditor.Editor
    {
        private readonly HashSet<string> _poolIdsSet = new HashSet<string>();

        public override void OnInspectorGUI()
        {
            if (!EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("只在运行时可用。", MessageType.Info);
                return;
            }
            
            var poolKit = target as SimpleGOPoolKit;

            if (poolKit)
            {
                EditorGUILayout.LabelField("SimpleGOPoolKit");
                
                var poolsInfo = poolKit.GetPoolsInfo();
                
                EditorGUILayout.LabelField("GameObject Pool Count", poolsInfo.Count.ToString());
                
                foreach (var poolInfo in poolsInfo)
                {
                    DrawGameObjectPool(poolInfo);
                }
            }

            Repaint();  // 每帧都会调用OnInspectorGUI，所以这里的Repaint会导致每帧都会刷新Inspector
        }

        private void DrawGameObjectPool(RecyclablePoolInfo poolInfo)
        {
            var isLastOpened = _poolIdsSet.Contains(poolInfo.PoolId);
            var isCurOpened = EditorGUILayout.Foldout(isLastOpened, poolInfo.PoolId);

            if (isCurOpened != isLastOpened)
            {
                if (isCurOpened)
                {
                    _poolIdsSet.Add(poolInfo.PoolId);
                }
                else
                {
                    _poolIdsSet.Remove(poolInfo.PoolId);
                }
            }
            
            if (isCurOpened)
            {
                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUILayout.LabelField("PoolId", poolInfo.PoolId);
                    EditorGUILayout.LabelField("ReferenceType", poolInfo.ReferenceType.ToString());
                    EditorGUILayout.LabelField("InitCreateCount", poolInfo.InitCreateCount.HasValue ? poolInfo.InitCreateCount.Value.ToString() : "-");
                    EditorGUILayout.LabelField("ReachMaxLimitType", poolInfo.ReachMaxLimitType.ToString());
                    if (poolInfo.ReachMaxLimitType != PoolReachMaxLimitType.Default)
                    {
                        EditorGUILayout.LabelField("MaxSpawnCount", poolInfo.MaxSpawnCount.HasValue ? poolInfo.MaxSpawnCount.Value.ToString() : "-");
                    }
                    EditorGUILayout.LabelField("DespawnDestroyType", poolInfo.DespawnDestroyType.ToString());
                    if (poolInfo.DespawnDestroyType == PoolDespawnDestroyType.DestroyToLimit)
                    {
                        EditorGUILayout.LabelField("MaxDespawnCount", poolInfo.MaxDespawnCount.HasValue ? poolInfo.MaxDespawnCount.Value.ToString() : "-");
                    }

                    EditorGUILayout.LabelField("ClearType", poolInfo.ClearType.ToString());
                    EditorGUILayout.LabelField("AutoClearTime", poolInfo.AutoClearTime.ToString());
                    EditorGUILayout.LabelField("IfIgnoreTimeScale", poolInfo.IsIgnoreTimeScale.ToString());
                    EditorGUILayout.LabelField("CachedObjectCount", poolInfo.CachedObjectCount.ToString());
                    EditorGUILayout.LabelField("UsedObjectCount", poolInfo.UsedObjectCount.ToString());
                    EditorGUILayout.LabelField("TotalObjectCount", poolInfo.TotalObjectCount.ToString());

                    if (GUILayout.Button("ClearUnusedObjects(Safe)"))
                    {
                        if (poolInfo.ExtraInfo is SimpleGameObjectPool pool)
                        {
                            pool.ClearUnusedObjects();
                        }
                    }
                    
                    if (GUILayout.Button("ClearPool(Unsafe)"))
                    {
                        if (poolInfo.ExtraInfo is SimpleGameObjectPool pool)
                        {
                            pool.ClearAll();
                        }
                    }
                    
                    //TODO Draw all Objects
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.Separator();
            }
        }
    }
}