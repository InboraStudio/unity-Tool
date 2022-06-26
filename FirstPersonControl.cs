using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonControl : MonoBehaviour {

    public GameObject player ;

	private bool inMouseMode = false ;
	private float rotationX ;
	private float rotationY ;

	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked ;
		Cursor.visible = false ;
	}

	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.Space)) {
			Cursor.lockState = CursorLockMode.None ;
			Cursor.visible = true ;
			inMouseMode = true ;
		}
		if(Input.GetKeyUp(KeyCode.Space)) {
			Cursor.lockState = CursorLockMode.Locked ;
			Cursor.visible = false ;
			inMouseMode = false ;
		}

		if(!inMouseMode) {
			player.transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * 10f, 0f, 0f) ;
			player.transform.Translate(0f, 0f, Input.GetAxis("Vertical") * Time.deltaTime * 10f) ;
			rotationY += transform.localEulerAngles.y + Input.GetAxis("Mouse X") * 2.0f ;
			rotationX += Input.GetAxis("Mouse Y") * 2.0f ;
			Camera.main.transform.localEulerAngles = new Vector3(-rotationX, 0f, 0f) ;
			player.transform.localEulerAngles = new Vector3(0f, rotationY, 0f) ;
		}
	}

}
