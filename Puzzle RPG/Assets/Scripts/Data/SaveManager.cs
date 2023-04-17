using UnityEngine;
using System.IO;

public static class SaveManager
{
    private static readonly string SAVE_FILEPATH = Application.dataPath + "/Saves/";
    //private static string SAVE_FILEPATH_FORMATTED; //Directory.GetCurrentDirectory() + "\\Saves\\"; // correct formatting
    private static readonly string FILE_FORMAT = ".txt";
    private static DirectoryInfo SAVE_DIRECTORY; // Looking for use, but not much exists rn

    /// <summary>
    /// Initializes save manager. 
    /// Finds location of saved files. 
    /// Creates directory if one does not exist. 
    /// </summary>
    public static void Init()
    {
        SAVE_DIRECTORY = new DirectoryInfo(SAVE_FILEPATH);

        // Looks for save directory and creates it if one doesn't exist
       if (!SAVE_DIRECTORY.Exists) //!Directory.Exists(SAVE_FILEPATH)
        {
            SAVE_DIRECTORY.Create(); //SAVE_DIRECTORY = Directory.CreateDirectory(SAVE_FILEPATH);
        }
    }

    public static void Save(Data data, string fileName)
    {
        
        string filePath = SAVE_FILEPATH + fileName + FILE_FORMAT;
        File.WriteAllText(filePath, Convert(data));
    }
    public static void NewSave(Data data, string fileName = "save")
    {
        int saveID = 0;
        string filePath = SAVE_FILEPATH + fileName + saveID + FILE_FORMAT;
        // Find a file that doesn't exist
        while (File.Exists(filePath))
        {
            saveID++;
            filePath = SAVE_FILEPATH + fileName + saveID + FILE_FORMAT;
        }
        File.WriteAllText(filePath, Convert(data));
    }
    public static Data Load(string fileName)
    {
        string filePath = SAVE_FILEPATH + fileName + FILE_FORMAT;
        if (File.Exists(filePath))
        {
            string loadedData = File.ReadAllText(filePath);
            return Convert(loadedData);
        }
        return null;
    }
    public static Data LoadNewData(string fileName)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(SAVE_FILEPATH);
        FileInfo[] saveFiles = dirInfo.GetFiles();
        FileInfo latestSaveFile = null;

        foreach (FileInfo saveFile in saveFiles)
        {
            if(latestSaveFile == null)
            {
                latestSaveFile = saveFile;
            }

            if(saveFile.LastWriteTime > latestSaveFile.LastWriteTime)
            {
                latestSaveFile = saveFile;
            }
        }

        if(latestSaveFile == null) { return null; }
        
        string filePath = SAVE_FILEPATH + latestSaveFile.FullName;
        string loadedData = File.ReadAllText(filePath);
        return Convert(loadedData);
    }

    // Helpers - could probably be moved to DataManager
    private static string Convert(Data dataObject)
    {
        return JsonUtility.ToJson(dataObject);
    }
    private static Data Convert(string dataObject)
    {
        Data data = new Data();
        JsonUtility.FromJsonOverwrite(dataObject, data);
        return data;
    }
}