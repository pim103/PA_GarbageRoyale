using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class SaveState
    {
        public static void SaveStateGame(bool isInMenu)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            string path = Application.persistentDataPath + "/savestate.menu";
            FileStream stream = new FileStream(path, FileMode.Create);

            DataState dataState = new DataState(PhotonNetwork.AuthValues.UserId, PhotonNetwork.NickName,
                PhotonNetwork.LocalPlayer.CustomProperties["role"].ToString(), isInMenu, !isInMenu);

            formatter.Serialize(stream, dataState);
            stream.Close();
        }

        public static DataState LoadData()
        {
            string path = Application.persistentDataPath + "/savestate.menu";
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                DataState data = formatter.Deserialize(stream) as DataState;
                stream.Close();

                return data;
            }
            else
            {
                Debug.Log("Error not found!");
                return null;
            }
        }

        public static void DeleteData()
        {
            string path = Application.persistentDataPath + "/savestate.menu";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}