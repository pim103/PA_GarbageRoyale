using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
	public class PlayerMovement : MonoBehaviour {

		public float speed = 6.0f;
		public float gravity = -9.8f;

		private CharacterController _charCont;
		private MazeConstructor gen;
        private PlayerStats playerStats;
		
		private GameController gameControl;
		
		private bool mine;

        private bool isOnWater;
        private bool mineWantToGoUp;
        private bool headIsOnWater;

		// Use this for initialization
		void Start () {
			_charCont = GetComponent<CharacterController> ();
            playerStats = GetComponent<PlayerStats>();
			gen = GameObject.Find("Controller").GetComponent<MazeConstructor>();
			gameControl = GameObject.Find("Controller").GetComponent<GameController>();

            isOnWater = false;
            headIsOnWater = false;
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
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    mineWantToGoUp = true;
                }
                else if (Input.GetKeyUp(KeyCode.Space))
                {
                    mineWantToGoUp = false;
                }

                if(!playerStats.getIsDead())
                {
                    Movement(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), mineWantToGoUp);
                }
			}
		}
		
		public void Movement(float axeX, float axeZ, bool wantToGoUp)
		{
            if(isOnWater)
            {
                speed = 4.0f;
            } else
            {
                speed = 6.0f;
            }

			float deltaX = axeX * speed;
			float deltaZ = axeZ * speed;
            float deltaY = 0.0f;

			Vector3 movement = new Vector3 (deltaX, deltaY, deltaZ);
			movement = Vector3.ClampMagnitude (movement, speed); //Limits the max speed of the player

            if (isOnWater && wantToGoUp)
            {
                movement.y += 10.8f;
            } else if (isOnWater)
            {
                movement.y += 8.8f;
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

        public void OnTriggerStay(Collider other)
        {
            if(_charCont.transform.position.y + 0.01f > other.transform.position.y)
            {
                headIsOnWater = false;
            } else
            {
                headIsOnWater = true;
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.gameObject.transform.position.y < _charCont.transform.position.y)
            {
                isOnWater = false;
                headIsOnWater = false;
            }
        }

        public bool PlayerisOnWater()
        {
            return isOnWater;
        }

        public bool getHeadIsOnWater()
        {
            return headIsOnWater;
        }
    }
}
