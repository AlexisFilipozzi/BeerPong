using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour {

    public float forceFactor;

    private float m_startTime;
    private Vector3 m_startPosition;
    private Rigidbody m_rb;

    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
    }

    void OnMouseDown()
    {
        m_startTime = Time.time;
        m_startPosition = getPosition();
        print("Test");

        // set position of the ball

    }

    Vector3 getPosition()
    {
        Vector3 result = Input.mousePosition;
        result.z = transform.position.z - Camera.main.transform.position.z;
        result = Camera.main.ScreenToWorldPoint(result);
        return result;
    }

    void OnMouseUp()
    {
        Vector3 endPosition = getPosition();

        Vector3 force = endPosition - m_startPosition;
        force.z = force.magnitude;

        m_rb.isKinematic = false;
        m_rb.AddForce(force * forceFactor);
    }
    
}
