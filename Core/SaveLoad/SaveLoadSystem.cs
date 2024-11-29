using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tools.SaveLoad
{
    public class SaveLoadSystem : PersistentSingleton<SaveLoadSystem>
    {
        [SerializeField] private GameData gameData;
        
        IDataService dataService;
        
        protected override void Awake()
        {
            base.Awake();
            dataService = new FileDataService(new JsonSerializer());    // 实例化接口
        }

        private void Start()
        {
            NewGame();
        }

        private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Menu") return;
            
            // Bind<Tools.SaveLoad.Sample.Player, PlayerData>(gameData.PlayerData);
        }

        private void GetGameData()
        {
            // gameData.PlayerData = GetData<Tools.SaveLoad.Sample.Player, PlayerData>();
        }

        private void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entity = FindObjectOfType<T>();
            // var entity = FindFirstObjectByType<T>();
            if (entity != null && data != null)
                entity.Bind(data);
        }
        
        private void Bind<T, TData>(List<TData> datas) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entities = FindObjectsOfType<T>();
            // var entities = FindObjectsByType<T>(FindObjectsSortMode.None);

            foreach (var entity in entities)
            {
                var data = datas.Find(d => d.HashCode == entity.HashCode);
                if (data == null)
                {
                    data = new TData() { HashCode = entity.HashCode };
                    datas.Add(data);
                }
                entity.Bind(data);
            }
        }
        
        private TData GetData<T, TData>() where T : MonoBehaviour, IBind<TData> where TData : class, ISaveable
        {
            var entity = FindObjectOfType<T>();
            // var entity = FindFirstObjectByType<T>();
            return entity != null ? entity.GetData() : null;
        }

        [Button]
        public void NewGame()
        {
            gameData = new GameData()
            {
                Name = "New Game",
                CurrentSceneName = "SaveLoad"
            };
            SceneManager.LoadScene(gameData.CurrentSceneName);
        }

        [Button]
        public void SaveGame()
        {
            GetGameData();
            dataService.Save(gameData);
        }

        [Button]
        public void LoadGame(string gameName)
        {
            gameData = dataService.Load(gameName);

            if (string.IsNullOrWhiteSpace(gameData.CurrentSceneName))
                gameData.CurrentSceneName = "Demo";
            
            SceneManager.LoadScene(gameData.CurrentSceneName);
        }
        
        [Button]
        public void ReLoadGame() => LoadGame(gameData.Name);
        
        [Button]
        public void DeleteGame(string gameName) => dataService.Delete(gameName);
    }
}