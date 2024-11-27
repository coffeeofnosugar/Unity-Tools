#if UNITY_EDITOR
using UnityEditor;
#endif
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tools.Inventory
{
    public class IconGenerator : MonoBehaviour
    {
        [FolderPath]
        public string path;
        
        [Button(ButtonSizes.Gigantic)]
        private void TakeScreenshot()
        {
            var fullPath = System.IO.Path.Combine(path, "Icons.png");
            
            var camera = GetComponent<Camera>();
            
            RenderTexture rt = new RenderTexture(256, 256, 24);
            camera.targetTexture = rt;
            Texture2D screenShot = new Texture2D(256, 256, TextureFormat.RGB24, false);
            camera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
            camera.targetTexture = null;
            RenderTexture.active = null;

            if (Application.isEditor)
                DestroyImmediate(rt);
            else
                Destroy(rt);
            
            byte[] bytes = screenShot.EncodeToPNG();
            System.IO.File.WriteAllBytes(fullPath, bytes);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
    }
}