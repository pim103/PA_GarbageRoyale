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

        private bool isOnWater;
        private bool mineWantToGoUp;

		// Use this for initialization
		void Start () {
			_charCont = GetComponent<CharacterController> ();
			gen = GameObject.Find("Controller").GetComponent<MazeConstructor>();
			gameControl = GameObject.Find("Controller").GetComponent<GameController>();

            isOnWater = false;
            mineWantToGoUp = false;
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
                float deltaY = 0.0f;
                
                if(Input.GetKeyDown(KeyCode.Space)) {
                    mineWantToGoUp = true;
                } else if (Input.GetKeyUp(KeyCode.Space))
                {
                    mineWantToGoUp = false;
                }
                
				Vector3 movement = new Vector3 (deltaX, deltaY, deltaZ);
				movement = Vector3.ClampMagnitude (movement, speed); //Limits the max speed of the player

                if (isOnWater && mineWantToGoUp)
                {
                    movement.y += 10.3f;
                } else if (isOnWater)
                {
                    movement.y += 9.3f;
                }
                movement.y += gravity;

                Debug.Log(movement);

				movement *= Time.deltaTime;		//Ensures the speed the player moves does not change based on frame rate
				movement = transform.TransformDirection(movement);
				_charCont.Move (movement);
			}
		}
		
		public void Movement(float axeX, float axeZ, bool wantToGoUp)
		{
			float deltaX = axeX;
			float deltaZ = axeZ;
            float deltaY = 0.0f;

			Vector3 movement = new Vector3 (deltaX, deltaY, deltaZ);
			movement = Vector3.ClampMagnitude (movement, speed); //Limits the max speed of the player

            if (isOnWater && wantToGoUp)
            {
                movement.y += 10.3f;
            } else if (isOnWater)
            {
                movement.y += 9.3f;
            }
            movement.y += gravity;

			movement *= Time.deltaTime;		//Ensures the speed the player moves does not change based on frame rate
			movement = transform.TransformDirection(movement);
			_charCont.Move (movement);
		}

        public void OnTriggerEnter(Collider other)
        {
            isOnWater = true;
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.gameObject.transform.position.y < _charCont.transform.position.y)
            {
                isOnWater = false;
            }
        }
	}
}
