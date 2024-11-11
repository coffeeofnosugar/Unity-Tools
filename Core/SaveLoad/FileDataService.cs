using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Tools.SaveLoad
{
    public interface IDataService
    {
        void Save(GameData data, bool overwrite = true);
        GameData Load(string name);
        void Delete(string name);
        void DeleteAll();
        IEnumerable<string> ListSaves();
    }

    public class FileDataService : IDataService
    {
        private readonly ISerializer serializer;
        private readonly string dataPath;
        private readonly string fileExtension;
        
        private string GetPathToFile(string fileName) => Path.Combine(dataPath, string.Concat(fileName, ".", fileExtension));
        
        /// 定义实例化方法
        public FileDataService(ISerializer serializer)
        {
            this.serializer = serializer;
            this.dataPath = Path.Combine(Application.persistentDataPath, "GameData");
            this.fileExtension = "json";

            if (!File.Exists(dataPath))
                Directory.CreateDirectory(dataPath);
        }
        
        public void Save(GameData data, bool overwrite = true)
        {
            string fileLocation = GetPathToFile(data.Name);

            if (!overwrite && File.Exists(fileLocation))
                throw new IOException($"文件'{data.Name}.{fileExtension}'已经存在");
            
            File.WriteAllText(fileLocation, serializer.Serialize(data));
        }

        public GameData Load(string name)
        {
            string fileLocation = GetPathToFile(name);

            if (!File.Exists(fileLocation))
                throw new ArgumentException($"文件'{name}.{fileExtension}'不存在");

            return serializer.Deserialize<GameData>(File.ReadAllText(fileLocation));
        }

        public void Delete(string name)
        {
            string fileLocation = GetPathToFile(name);

            if (!File.Exists(fileLocation))
                throw new ArgumentException($"文件'{name}.{fileExtension}'不存在");

            File.Delete(fileLocation);
        }

        public void DeleteAll()
        {
            foreach (var file in Directory.GetFiles(dataPath))
            {
                File.Delete(file);
            }
        }

        public IEnumerable<string> ListSaves()
        {
            foreach (string path in Directory.EnumerateFiles(dataPath))
            {
                if (Path.GetExtension(path) == fileExtension)
                {
                    yield return Path.GetFileNameWithoutExtension(path);
                }
            }
        }
    }
}