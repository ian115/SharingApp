using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices.ComTypes;

public static class SaveSystem
{
    
    public static void SaveData(JSONReader jsonReader)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Path.Combine(Application.persistentDataPath, "userdata.ian"); // Set a persistent path

        // Create the file.
        // We use "using" since it will automatically call stream.close()
        using (FileStream stream = new FileStream(path, FileMode.Create)) 
        {
            // Set the data to serialize
            UserData data = new UserData(jsonReader);


            formatter.Serialize(stream, data);
        }
    }


    public static UserData LoadData()
    {
        // Path combine is safer for OS.
        string path = Path.Combine(Application.persistentDataPath, "userdata.ian"); // Set a persistent path

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream stream = new FileStream(path, FileMode.Open))
            {

                // We need to cast it so it knows which data we are looking for.
                UserData data = (UserData)formatter.Deserialize(stream);

                return data;
            }
        }
        else
        {
            #if UNITY_EDITOR
            Debug.LogError("[IGNORABLE ASSERT] Save file not found. Path: " + path);
            #endif
            return null;
        }

    }


}
