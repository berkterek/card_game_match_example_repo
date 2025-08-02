using CardGame.Abstracts.DataAccessLayers;
using CardGame.Abstracts.Managers;
using CardGame.DataAccessLayers;

public class SaveLoadManager : SaveLoadService
{
    static SaveLoadManager _instance;
    readonly IDataSaveLoadDal _dataSaveLoadDal;

    private SaveLoadManager(IDataSaveLoadDal dataSaveLoadDal)
    {
        _dataSaveLoadDal = dataSaveLoadDal;
    }

    public static SaveLoadManager Singleton()
    {
        if (_instance == null)
        {
            _instance = new SaveLoadManager(new PlayerPrefSaveLoadDal());
        }

        return _instance;
    }

    public void SaveDataProcess(string key, object value)
    {
        _dataSaveLoadDal.SaveData(key, value);
    }

    public T LoadDataProcess<T>(string key)
    {
        var value = _dataSaveLoadDal.LoadData<T>(key);
        return value;
    }

    public void SaveUnityObjectProcess(string key, UnityEngine.Object value)
    {
        _dataSaveLoadDal.SaveUnityObject(key, value);
    }

    public T LoadUnityObjectProcess<T>(string key) where T : UnityEngine.Object
    {
        var value = _dataSaveLoadDal.LoadUnityObject<T>(key);
        return value;
    }

    public bool HasKeyAvailable(string key)
    {
        return _dataSaveLoadDal.HasKey(key);
    }

    public void DeleteData(string name)
    {
        _dataSaveLoadDal.DeleteData(name);
    }
}
