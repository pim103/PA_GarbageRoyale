using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
	public class PlayerMovement : MonoBehaviour {

		public float speed = 4.0f;
		public float gravity = -9.8f;

		private CharacterController _charCont;
		private MazeConstructor gen;
		
		private GameController gameControl;
		
		private bool mine;

		// Use this for initialization
		void Start () {
			_charCont = GetComponent<CharacterController> ();
			gen = GameObject.Find("Controller").GetComponent<MazeConstructor>();
			gameControl = GameObject.Find("Controller").GetComponent<GameController>();
		}
	
		// Update is called once per frame
		void Update ()
		{
			mine = false;
			foreach (var pair in gameControl.characterList)
			{
				if (pair.Value.transform == this.transform && pair.Key == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					mine = true;
				}
			}

			if (mine)
			{
				float deltaX = Input.GetAxis ("Horizontal") * speed;
				float deltaZ = Input.GetAxis ("Vertical") * speed;
				Vector3 movement = new Vector3 (deltaX, 0, deltaZ);
				movement = Vector3.ClampMagnitude (movement, speed); //Limits the max speed of the player

				movement.y = gravity;

				movement *= Time.deltaTime;		//Ensures the speed the player moves does not change based on frame rate
				movement = transform.TransformDirection(movement);
				_charCont.Move (movement);
			}
		}
		
		public void Movement(float axeX, float axeZ)
		{
			float deltaX = axeX;
			float deltaZ = axeZ;
			Vector3 movement = new Vector3 (deltaX, 0, deltaZ);
			movement = Vector3.ClampMagnitude (movement, speed); //Limits the max speed of the player

			movement.y = gravity;

			movement *= Time.deltaTime;		//Ensures the speed the player moves does not change based on frame rate
			movement = transform.TransformDirection(movement);
			_charCont.Move (movement);
		}
	}
}
