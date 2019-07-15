using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace GarbageRoyale.Scripts
{
    public class SendScores
    {
        public static IEnumerator SendScore(string userId, int score)
        {
            WWWForm form = new WWWForm();
            form.AddField("userid", userId);
            form.AddField("addScore", score);
            var www = UnityWebRequest.Post("https://www.garbage-royale.heolia.eu/services/account/updateScore.php", form);
            www.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
            www.SendWebRequest();
            yield return new WaitForSeconds(0.5f);
            if (www.responseCode == 202)
            {
                Debug.Log(" / Score mis à jour pour le joueur : " + userId); 
            }
            else if (www.responseCode == 406)
            {
                Debug.Log("Erreur : Vérifiez les paramètres envoyés"); 
            }
            else
            {
               Debug.Log("Erreur : Serveur indisponible " + www.responseCode + " / " + www.downloadHandler.text); 
            }
        }
    }
}