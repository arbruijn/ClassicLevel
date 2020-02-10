using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    private new  Rigidbody rigidbody;

	// Use this for initialization
	void Start () {
		rigidbody = gameObject.GetComponent<Rigidbody>();
	}
	
    // Update is called once per frame
    void FixedUpdate() {
        float speed = 1000;
        float rspeed = 500;
        if (Input.GetKey(KeyCode.A))
        {
            rigidbody.AddForce(transform.forward * Time.fixedDeltaTime * speed);
        }
        if (Input.GetKey(KeyCode.Z))
        {
            rigidbody.AddForce(transform.forward * Time.fixedDeltaTime * -speed);
        }
        if (Input.GetKey(KeyCode.End))
        {
            rigidbody.AddForce(transform.right * Time.fixedDeltaTime * -speed);
        }
        if (Input.GetKey(KeyCode.PageDown))
        {
            rigidbody.AddForce(transform.right * Time.fixedDeltaTime * speed);
        }
        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            rigidbody.AddForce(transform.up * Time.fixedDeltaTime * speed);
        }
        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            rigidbody.AddForce(transform.up * Time.fixedDeltaTime * -speed);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rigidbody.AddTorque(transform.up * Time.fixedDeltaTime * -rspeed);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rigidbody.AddTorque(transform.up * Time.fixedDeltaTime * rspeed);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rigidbody.AddTorque(transform.right * Time.fixedDeltaTime * rspeed);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            rigidbody.AddTorque(transform.right * Time.fixedDeltaTime * -rspeed);
        }
        if (Input.GetKey(KeyCode.Home))
        {
            rigidbody.AddTorque(transform.forward * Time.fixedDeltaTime * rspeed);
        }
        if (Input.GetKey(KeyCode.PageUp))
        {
            rigidbody.AddTorque(transform.forward * Time.fixedDeltaTime * -rspeed);
        }
    }
}
