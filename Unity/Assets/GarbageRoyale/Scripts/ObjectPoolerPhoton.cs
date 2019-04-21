using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
	[System.Serializable]
	public class ObjectPoolPhotonItem
	{

		public string objectToPool;
		public int amountToPool;
		public bool shouldExpand = true;

		public ObjectPoolPhotonItem(string obj, int amt, bool exp = true)
		{
			objectToPool = obj;
			amountToPool = Mathf.Max(amt,2);
			shouldExpand = exp;
		}
	}

	public class ObjectPoolerPhoton : MonoBehaviour
	{
		public static ObjectPoolerPhoton SharedInstance;
		public List<ObjectPoolPhotonItem> itemsToPool;


		public List<List<GameObject>> pooledObjectsList;
		public List<GameObject> pooledObjects;
		private List<int> positions;

		void Awake()
		{

			SharedInstance = this;

			pooledObjectsList = new List<List<GameObject>>();
			pooledObjects = new List<GameObject>();
			positions = new List<int>();


			for (int i = 0; i < itemsToPool.Count; i++)
			{
				ObjectPoolItemToPooledObject(i);
			}

		}


		public GameObject GetPooledObject(int index)
		{

			int curSize = pooledObjectsList[index].Count;
			for (int i = positions[index] + 1; i < positions[index] + pooledObjectsList[index].Count; i++)
			{

				if (!pooledObjectsList[index][i % curSize].activeInHierarchy)
				{
					positions[index] = i % curSize;
					return pooledObjectsList[index][i % curSize];
				}
			}

			if (itemsToPool[index].shouldExpand)
			{

				GameObject obj = (GameObject)PhotonNetwork.Instantiate(itemsToPool[index].objectToPool,new Vector3(0,0,0), Quaternion.identity);
				obj.SetActive(false);
				obj.transform.parent = this.transform;
				pooledObjectsList[index].Add(obj);
				return obj;

			}
			return null;
		}

		public List<GameObject> GetAllPooledObjects(int index)
		{
			return pooledObjectsList[index];
		}


		public int AddObject(string GO, int amt = 3, bool exp = true)
		{
			ObjectPoolPhotonItem item = new ObjectPoolPhotonItem(GO, amt, exp);
			int currLen = itemsToPool.Count;
			itemsToPool.Add(item);
			ObjectPoolItemToPooledObject(currLen);
			return currLen;
		}


		void ObjectPoolItemToPooledObject(int index)
		{
			ObjectPoolPhotonItem item = itemsToPool[index];

			pooledObjects = new List<GameObject>();
			for (int i = 0; i < item.amountToPool; i++)
			{
				GameObject obj = (GameObject)PhotonNetwork.Instantiate(itemsToPool[index].objectToPool,new Vector3(0,0,0), Quaternion.identity);
				// obj.AddComponent<Item>();
				// obj.GetComponent<Item>().initItem(1);
				obj.SetActive(false);
				obj.transform.parent = this.transform;
				pooledObjects.Add(obj);
			}
			pooledObjectsList.Add(pooledObjects);
			positions.Add(0);

		}
	}
}