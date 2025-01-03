using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.EasyPoolKit.Demo
{
    public class SimpleGOPoolManager : MonoBehaviour
    {
        public GameObject cubePrefab;
        [ReadOnly] public List<GameObject> cubes = new List<GameObject>();
        
        [Space]
        public GameObject imagePrefab;
        public Transform parent;
        [ReadOnly] public List<GameObject> images = new List<GameObject>();
        
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                var newImage = SimpleGOPoolKit.Instance.SimpleSpawn(imagePrefab);
                newImage.transform.SetParent(parent);
                images.Add(newImage);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                if (images.Count > 0)
                {
                    var lastImage = images[^1];
                    SimpleGOPoolKit.Instance.Despawn(lastImage);
                    images.RemoveAt(images.Count - 1);
                }
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                SimpleGOPoolKit.Instance.ClearAllPools(true);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                var newImage = SimpleGOPoolKit.Instance.SimpleSpawn(cubePrefab);
                newImage.transform.SetParent(null);
                cubes.Add(newImage);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (cubes.Count > 0)
                {
                    var lastCube = cubes[^1];
                    SimpleGOPoolKit.Instance.Despawn(lastCube);
                    cubes.RemoveAt(cubes.Count - 1);
                }
            }
        }
    }
}
