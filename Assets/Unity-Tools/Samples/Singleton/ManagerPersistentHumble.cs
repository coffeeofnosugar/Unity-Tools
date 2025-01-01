using UnityEngine.Serialization;

namespace Tools.Singleton.Sample
{
    public class Manager : PersistentSingleton<Manager>
    {
        [FormerlySerializedAs("i")] public int index = 1000;
    }
}