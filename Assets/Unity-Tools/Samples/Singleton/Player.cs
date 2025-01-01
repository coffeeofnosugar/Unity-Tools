using UnityEngine;

namespace Tools.Singleton.Sample
{
    public class Player : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log(Manager.Instance.index);
        }
    }
}