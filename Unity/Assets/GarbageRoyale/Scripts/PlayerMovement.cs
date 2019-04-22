using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
	public class PlayerMovement : MonoBehaviourPunCallbacks
    {

        //public float speed = 6.0f;
        //public float jumpSpeed = 20.0f;
        //public float gravity = -9.8f;
        public float speed = 6.0f;
        public float jumpSpeed = 8.0f;
        public float gravity = 20.0f;
        public float swimY = 3.0f;

        public AudioClip walkSound;

        private CharacterController _charCont;
		private MazeConstructor gen;
        private PlayerStats playerStats;
        private SoundManager soundM;
		
		private GameController gameControl;
		
		private bool mine;
        private Vector3 moveDirection = Vector3.zero;

        private bool isOnWater;
	    private bool isInTransition;
        private bool mineWantToGoUp;
	    private bool mineWantToGoDown;
        private bool headIsOnWater;
        private bool feetIsInWater;

        private AudioSource audioWalk;
        public bool needToPlaySong;
        public string soundNeeded;

        private Animator anim;
        private Camera cam;

        // Use this for initialization
        void Start () {
			_charCont = GetComponent<CharacterController> ();
            playerStats = GetComponent<PlayerStats>();
			gen = GameObject.Find("Controller").GetComponent<MazeConstructor>();
			gameControl = GameObject.Find("Controller").GetComponent<GameController>();
            soundM = GameObject.Find("Controller").GetComponent<SoundManager>();

            audioWalk = GetComponent<AudioSource>();
            anim = GetComponent<Animator>();

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
				else if (Input.GetKeyDown(KeyCode.LeftShift))
				{
					mineWantToGoDown = true;
				}
				else if (Input.GetKeyUp(KeyCode.LeftShift))
				{
					mineWantToGoDown = false;
				}

				if(!playerStats.getIsDead())
				{
					Movement(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), Input.GetButton("Jump"), mineWantToGoDown, true);
				}
			}
		}
		
		public void Movement(float axeX, float axeZ, bool wantToGoUp, bool wantToGoDown, bool serverCall)
		{
            if(isOnWater)
            {
                speed = 4.0f;
            } else
            {
                speed = 6.0f;
            }

            needToPlaySong = (axeX != 0 || axeZ != 0);
            setSong(wantToGoUp);
            
            if(needToPlaySong)
            {
                anim.Play("Movement");
            }

            if (_charCont.isGrounded)
            {
                // We are grounded, so recalculate
                // move direction directly from axes

                moveDirection = new Vector3(axeX, 0.0f, axeZ);
                moveDirection *= speed;

                moveDirection = transform.TransformDirection(moveDirection);

                if (wantToGoUp && !isOnWater)
                {
                    moveDirection.y = jumpSpeed;
                }
            } else if (!isOnWater)
            {
                float tempY = moveDirection.y;
                moveDirection = new Vector3(axeX, 0.0f, axeZ);
                moveDirection *= speed;

                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection.y = tempY;
            } else
            {
                moveDirection = new Vector3(axeX, 0.0f, axeZ);
                moveDirection *= speed;

                moveDirection = transform.TransformDirection(moveDirection);
            }
            
            if (isInTransition && wantToGoUp && moveDirection.y < 6.0f)
            {
                moveDirection.y += 6.0f;
            }
            if (isInTransition && wantToGoDown)
            {
                moveDirection.y += 1.0f;
            }
            else if (isInTransition)
            {
                moveDirection.y += 9.8f * Time.deltaTime;
            }
            else if (isOnWater && wantToGoUp && moveDirection.y < 3.0f)
            {
                moveDirection.y += 3.0f;
            }
            else if (isOnWater)
            {
                moveDirection.y += 1.0f;
            }

            moveDirection.y -= gravity * Time.deltaTime;

            _charCont.Move(moveDirection * Time.deltaTime);

            if(_charCont.transform.position.y > (8+4) * 8 + 11)
            {
                photonView.RPC("returnToMenu", RpcTarget.All, null);
            }
        }

        [PunRPC]
        private void returnToMenu()
        {
            PhotonNetwork.LoadLevel("SampleScene");
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
	        if(other.name == "Water") isOnWater = true;
	        if (other.name == "Transition") isInTransition = true;
        }

        public void OnTriggerStay(Collider other)
        {
	        if (other.name == "Water")
	        {
		        float posY = _charCont.transform.position.y;
		        float waterPosY = other.transform.position.y;

		        if (posY - 0.25 > waterPosY)
		        {
			        feetIsInWater = true;
		        }
		        else
		        {
			        feetIsInWater = false;
		        }

		        if (posY + 0.01f > waterPosY)
		        {
			        headIsOnWater = false;
		        }
		        else
		        {
			        headIsOnWater = true;
		        }
	        }
        }

        public void OnTriggerExit(Collider other)
        {
	        if (other.name == "Water")
	        {
		        if (other.gameObject.transform.position.y < _charCont.transform.position.y)
		        {
			        isOnWater = false;
			        headIsOnWater = false;
		        }
	        }

	        if (other.name == "Transition")
	        {
		        isInTransition = false;
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
