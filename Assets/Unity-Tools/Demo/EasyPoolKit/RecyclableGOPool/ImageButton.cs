using UnityEngine;
using UnityEngine.UI;

namespace Tools.EasyPoolKit.Demo
{
    public class ImageButton : RecyclableMonoBehaviour
    {
        [SerializeField] private Button button;

        public override void OnObjectInit()
        {
            base.OnObjectInit();
            button.onClick.AddListener(OnClick);
        }

        public override void OnObjectSpawn()
        {
            base.OnObjectSpawn();
            Debug.Log($"ImageButton Spawned {name}");
        }

        public override void OnObjectDespawn()
        {
            base.OnObjectDespawn();
            Debug.Log($"ImageButton Despawn {name}");
        }

        private void OnClick()
        {
            Debug.Log($"Button Clicked {name}");
        }
    }
}