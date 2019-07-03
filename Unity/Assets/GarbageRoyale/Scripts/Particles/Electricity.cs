using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GarbageRoyale.Scripts;
using GarbageRoyale.Scripts.PrefabPlayer;
using Photon.Pun;
using UnityEngine;

public class Electricity : MonoBehaviour
{
    [SerializeField]
    private BoxCollider bx;

    private float timeToBurn;
    private GameController gc;
    
    private IEnumerator[] coroutine;

    private void Start()
    {
        timeToBurn = 50.0f;
        gc = GameObject.Find("Controller").GetComponent<GameController>();
        coroutine = new IEnumerator[10];
    }

    private void Update()
    {
        if (timeToBurn > 0.0f)
            {
                timeToBurn -= Time.deltaTime;
            }
            else
            {
                bx.enabled = false;
                gameObject.SetActive(false);
            }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (other.name.StartsWith("Player"))
        {
            int id = other.GetComponent<ExposerPlayer>().PlayerIndex;

            coroutine[id] = DealDamage(id);
            StartCoroutine(coroutine[id]);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (other.name.StartsWith("Player"))
        {
            int id = other.GetComponent<ExposerPlayer>().PlayerIndex;
            StopCoroutine(coroutine[id]);
            gc.playersActions[id].isTrap = false;
        }
    }

    private IEnumerator DealDamage(int id)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            yield return null;
        }

        while (true)
        {
            gc.players[id].PlayerStats.takeDamage(1f);
            gc.playersActions[id].isTrap = !gc.playersActions[id].isTrap;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
