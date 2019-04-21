using Photon.Pun;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
	public class CameraControl : MonoBehaviour {
		
		private Camera playerCam;
		public enum RotationAxis{
			MouseX = 1,
			MouseY = 2
		}

		public RotationAxis axes = RotationAxis.MouseX;

		public float minimumVert = -90.0f;
		public float maximumVert = 90.0f;

		public float sensHorizontal = 10.0f;
		public float sensVertical = 10.0f;

		public float _rotationX = 0;
		
		private GameController gameControl;
		private bool mine;
		

		private void Start()
		{
			gameControl = GameObject.Find("Controller").GetComponent<GameController>();
			
			if (axes == RotationAxis.MouseY)
			{
				playerCam = GetComponent<Camera> ();

				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
		}

		// Update is called once per frame
		void Update () {
			
				//if (!PhotonNetwork.IsMasterClient) return;
				float rotationY;
				if (axes == RotationAxis.MouseX)
				{
					mine = false;
					foreach (var pair in gameControl.characterList)
					{
						if ((pair.Value.transform == this.transform) && pair.Key == PhotonNetwork.LocalPlayer.ActorNumber)
						{
							mine = true;
						}
					}

					if (mine || !PhotonNetwork.IsMasterClient)
					{
						transform.Rotate(0, Input.GetAxis("Mouse X") * sensHorizontal, 0);
						if (PhotonNetwork.IsMasterClient)
						{
							_rotationX -= Input.GetAxis("Mouse Y") * sensVertical;
							_rotationX =
								Mathf.Clamp(_rotationX, minimumVert,
									maximumVert); //Clamps the vertical angle within the min and max limits (45 degrees)

							rotationY = transform.localEulerAngles.y;
							transform.GetChild(1).localEulerAngles = new Vector3(_rotationX, 0, 0);
						}
						
					}
				}
				else if (axes == RotationAxis.MouseY)
				{
					
						_rotationX -= Input.GetAxis("Mouse Y") * sensVertical;
						_rotationX =
							Mathf.Clamp(_rotationX, minimumVert,
								maximumVert); //Clamps the vertical angle within the min and max limits (45 degrees)

						rotationY = transform.localEulerAngles.y;

						transform.localEulerAngles = new Vector3(_rotationX, rotationY, 0);
						//
						//transform.GetChild(1).Rotate(0, _rotationX,0);
						//transform.GetChild(1).localEulerAngles = new Vector3(transform.GetChild(1).localEulerAngles.x, 
						//transform.GetChild(1).localEulerAngles.y + 1.0f, transform.GetChild(1).localEulerAngles.z);
					
				}
			
		}
		
		void OnGUI() {
			if (axes == RotationAxis.MouseY)
			{
				int size = 20;
				float posX = playerCam.pixelWidth / 2 - size / 4;
				float posY = playerCam.pixelHeight / 2 - size / 2;
				GUI.Label(new Rect(posX, posY, size, size), "+");
			}
		}
	}
}
