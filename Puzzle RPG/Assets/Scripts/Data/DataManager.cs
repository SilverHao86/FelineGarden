using UnityEngine;

//#if DEBUG
//Debug.Log("DEBUG");
//#endif
//#if DEVELOPMENT_BUILD
//Debug.Log("DEVELOPMENT_BUILD");
//#endif

class DataManager
{
    private Data currentData; // might not need
    private const int TIME_REQUIRED_TO_SAVE = 10;
    public DataManager()
    {
        SaveManager.Init();
    }

    public void EndGame(float timer = 0.0f)
    {
        currentData.timePlayed = timer;
        if (currentData.timePlayed > TIME_REQUIRED_TO_SAVE)
        {
            Save(currentData, "fin");
        }
    }
    public void Save(Data data, string fileName = "")
    {
        SaveManager.NewSave(data, fileName);
    }
    private Data Load(string fileName = "")
    {
        Data data = SaveManager.Load(fileName);
        if(!data)
        {
            Debug.Log("No data with filename \""+fileName+"\"");
            return null;
        }
        return data;
    }
}
