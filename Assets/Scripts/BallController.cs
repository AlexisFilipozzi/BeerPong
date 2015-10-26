using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour {

    public float forceFactor;
    public float x_forceFactor;
    public float z_forceFactor;
    public float angle;

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
        print("Test " + forceFactor);
        print("Gravity" + Physics.gravity);
        // set position of the ball

    }

    Vector3 getPosition()
    {
        Vector3 result = Input.mousePosition;
        return result;
    }

    float getAngle()
    {
        return (Mathf.PI/180)*angle;
    }

    void OnMouseUp()
    {
        Vector3 endPosition = getPosition();
        Vector3 speed = endPosition - m_startPosition;
        getAngle();
        float h = speed.y;
        float h0 = m_rb.position.y;
        float w = speed.x;
        float g = Physics.gravity.magnitude;
        float A = Mathf.Tan(getAngle());
        float Vz = Mathf.Sqrt(g)*h*z_forceFactor/Mathf.Sqrt(2*A*h*z_forceFactor+2*h0);
        float Vy = Vz * A;
        float T0 = (Vz + Mathf.Sqrt(Vz * Vz + 2 * g * h0)) / g;
        float Vx = w * x_forceFactor / T0;
        v = new Vector3(Vx, Vy, Vz);
        m_rb.isKinematic = false;
        m_rb.velocity = v;
    }
    
}
