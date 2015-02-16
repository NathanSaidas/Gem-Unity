using UnityEngine;
using System.Collections;

public class WASDMovement : MonoBehaviour 
{

    [SerializeField]
    private float m_Speed = 2.0f;

	// Update is called once per frame
	void Update ()
    {
	    if(Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.forward * m_Speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= Vector3.forward * m_Speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= Vector3.right * m_Speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * m_Speed * Time.deltaTime;
        }
	}
}
