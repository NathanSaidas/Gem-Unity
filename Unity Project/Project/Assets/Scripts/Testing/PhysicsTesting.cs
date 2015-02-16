using UnityEngine;
using System.Collections;

public class PhysicsTesting : MonoBehaviour 
{
    public float m_Radius = 0.0f;
    public Collider[] m_CollidersHit = null;


	// Update is called once per frame
	void Update () 
    {
        m_CollidersHit = Physics.OverlapSphere(transform.position, m_Radius,1);
	}
}
