
namespace CardGame.Abstracts.Managers
{
    public interface SaveLoadService
    {
        public void SaveDataProcess(string key, object value);
        public T LoadDataProcess<T>(string key);
        public void SaveUnityObjectProcess(string key, UnityEngine.Object value);
        public T LoadUnityObjectProcess<T>(string key) where T : UnityEngine.Object;
        public bool HasKeyAvailable(string key);
        public void DeleteData(string name);
    }
}