using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class DataHandler : MonoBehaviour
{


    public static Data data;
    // Start is called before the first frame update
    void Start()
    {

        string filePath = Application.persistentDataPath + "/example.json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Debug.Log("Loaded JSON: " + json);

            data = JsonUtility.FromJson<Data>(json);

            if (data != null)
            {
                Debug.Log("JSON deserialized successfully.");
                Debug.Log("Number of TranslatedContents: " + data.TranslatedContents.Count);
            }
            else
            {
                Debug.LogError("Failed to deserialize JSON into Data object.");
            }
        }
        else
        {
            Debug.LogError("File does not exist at path: " + filePath);
        }

        GameMaster.Instance.CreateLanguageButtons();
    }

}
