using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
	public class PlayerMovement : MonoBehaviour
    {

		public float speed = 6.0f;
		public float gravity = -9.8f;

        public AudioClip walkSound;

        private CharacterController _charCont;
		private MazeConstructor gen;
        private PlayerStats playerStats;
        private SoundManager soundM;
		
		private GameController gameControl;
		
		private bool mine;

        private bool isOnWater;
        private bool mineWantToGoUp;
        private bool headIsOnWater;
        private bool feetIsInWater;

        private AudioSource audioWalk;
        public bool needToPlaySong;
        public string soundNeeded;

        // Use this for initialization
        void Start () {
			_charCont = GetComponent<CharacterController> ();
            playerStats = GetComponent<PlayerStats>();
			gen = GameObject.Find("Controller").GetComponent<MazeConstructor>();
			gameControl = GameObject.Find("Controller").GetComponent<GameController>();
            soundM = GameObject.Find("Controller").GetComponent<SoundManager>();

            audioWalk = GetComponent<AudioSource>();

            isOnWater = false;
            headIsOnWater = false;
            mineWantToGoUp = false;
            feetIsInWater = false;
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

            needToPlaySong = (axeX != 0 || axeZ != 0);
            setSong(wantToGoUp);

            if (isOnWater && wantToGoUp)
            {
                movement.y += 10.8f;
            } else if (isOnWater)
            {
                movement.y += 8.8f;
            } else if (wantToGoUp)
            {
	            //movement.y += 18.3f;
	            
	            if (Physics.Raycast(transform.GetChild(0).transform.position, Vector3.down, 0.2f))
	            {
		            //this.GetComponent<Rigidbody>().velocity = Vector3.up * 10.0f;
		            movement.y += 80.3f;
	            }
            }
            movement.y += gravity;
			movement *= Time.deltaTime;		//Ensures the speed the player moves does not change based on frame rate
			movement = transform.TransformDirection(movement);
			_charCont.Move (movement);
		}

        private void setSong(bool wantToGoUp)
        {
            if (feetIsInWater)
            {
                soundNeeded = "walkInWater";
            }
            else if ((isOnWater && needToPlaySong) || (isOnWater && wantToGoUp))
            {
                needToPlaySong = true;
                soundNeeded = "swimming";
            }
            else
            {
                soundNeeded = "walk";
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            isOnWater = true;
        }

        public void OnTriggerStay(Collider other)
        {
            float posY = _charCont.transform.position.y;
            float waterPosY = other.transform.position.y;
            
            if (posY - 0.25 > waterPosY)
            {
                feetIsInWater = true;
            } else
            {
                feetIsInWater = false;
            }

            if(posY + 0.01f > waterPosY)
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
