using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace GarbageRoyale.Scripts.Localization
{
    public class LocalizationSave
    {
        public static void NewLocalizationSave(string fileName)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            string path = Application.persistentDataPath + "/localization.cache";
            FileStream stream = new FileStream(path, FileMode.Create);

            LocalizationState localState = new LocalizationState(fileName);

            formatter.Serialize(stream, localState);
            stream.Close();
            Debug.Log(Application.persistentDataPath);
        }

        public static LocalizationState LoadData()
        {
            string path = Application.persistentDataPath + "/localization.cache";
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                LocalizationState data = formatter.Deserialize(stream) as LocalizationState;
                stream.Close();
                Debug.Log(data.LocalizationFileName);
                return data;
            }
            else
            {
                Debug.Log("Error not found!");
                return null;
            }
        }
    }
}