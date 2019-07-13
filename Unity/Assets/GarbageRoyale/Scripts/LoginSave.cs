using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class LoginSave
    {
        public static void SaveMailLogin(string email)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            
            string path = Application.persistentDataPath + "/cache.login";
            //Debug.Log(path);
            FileStream stream = new FileStream(path, FileMode.Create);
            
            DataLogin dataState = new DataLogin(email);
            
            formatter.Serialize(stream, dataState);
            stream.Close();
        }

        public static string LoadMailLogin()
        {
            string path = Application.persistentDataPath + "/cache.login";
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                DataLogin data = formatter.Deserialize(stream) as DataLogin;
                stream.Close();
                
                return data.Email;
            }
            else
            {
                Debug.Log("Error not found!");
                return null;
            }
        }
    }
}