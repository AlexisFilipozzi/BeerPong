﻿using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour {

    public float forceFactor;
    public float x_forceFactor;
    public float z_forceFactor;
    public float angle;
    public float minDistance;
    
    private Vector3 m_startPosition;
    private Rigidbody m_rb;
    private Vector3 m_ballRespawn;
    private bool m_startDrag;
    private bool m_canBeDrag;
    private float dt = 0.1F;
    private float DT = 0.0F;
    private Vector3 pos0;
    private Vector3 pos1;
    private float m_ballCameraDistance;

    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_startDrag = false;
        m_ballRespawn = m_rb.position;
        pos0 = m_rb.position;
        pos1 = m_rb.position;
        m_canBeDrag = true;

        // getting the initial distance ball camera
        float squareDistCameraBallX = transform.position.x - Camera.main.transform.position.x;
        squareDistCameraBallX = squareDistCameraBallX * squareDistCameraBallX;
        float squareDistCameraBallY = transform.position.y - Camera.main.transform.position.y;
        squareDistCameraBallY = squareDistCameraBallY * squareDistCameraBallY;
        float squareDistCameraBallZ = transform.position.z - Camera.main.transform.position.z;
        squareDistCameraBallZ = squareDistCameraBallZ * squareDistCameraBallZ;
        m_ballCameraDistance = Mathf.Sqrt(squareDistCameraBallX + squareDistCameraBallY + squareDistCameraBallZ);
    }

    bool DisableCupIfWin()
    {
        GameObject[] cups;
        cups = GameObject.FindGameObjectsWithTag("cup");
        GameObject closest = null;
        float distance = minDistance;
        Vector3 position = m_rb.position;
        foreach (GameObject cup in cups)
        {
            Vector3 diff = cup.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = cup;
                distance = curDistance;
            }
        }
        if (closest == null)
            return false;
        closest.SetActive(false);
        return true;
    }

    void OnMouseDown()
    {
        if (m_canBeDrag)
        {
            m_startDrag = true;
            m_startPosition = Input.mousePosition;
        }
    }

    Vector3 getPosition()
    {
        Vector3 viewPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        
        return Camera.main.ViewportToWorldPoint(new Vector3(viewPos.x, viewPos.y, m_ballCameraDistance));
    }

    float getAngle(float h)
    {
        print(h);
        print(180/Mathf.PI*Mathf.Atan(h / 1000));
        float temp = Mathf.Atan(h / 1000);
        if (h < 0)
            temp += -25 / 180 * Mathf.PI;
        return temp;
    }

    void OnMouseUp()
    {
        Vector3 endPosition = Input.mousePosition;
        if (!m_canBeDrag)
        {
            return;
        }
        Vector3 speed = endPosition - m_startPosition;
        float h = speed.y;
        float h0 = m_rb.position.y;
        float w = speed.x;
        float g = Physics.gravity.magnitude;
        float A = Mathf.Tan(getAngle(h));
        float Vz = Mathf.Sqrt(g)*Mathf.Abs(h)*z_forceFactor/Mathf.Sqrt(2*A*h*z_forceFactor+2*h0);
        float Vy = Vz * A;
        float T0 = (Vz + Mathf.Sqrt(Vz * Vz + 2 * g * h0)) / g;
        float Vx = w * x_forceFactor / T0;
        m_rb.isKinematic = false;
        m_startDrag = false;
        Vector3 vel = new Vector3(Vx, Vy, Vz);
        m_rb.velocity = vel;
        m_canBeDrag = false;
    }

    float getAcceleration()
    {
        Vector3 acc = pos0 - 2*pos1 + m_rb.position;
        pos0 = pos1;
        pos1 = m_rb.position;
        return acc.magnitude;
    }

    void die()
    {
        m_rb.isKinematic = true;
        m_rb.velocity = new Vector3(0, 0, 0);
        m_rb.position = m_ballRespawn;
        m_canBeDrag = true;
    }

    void Update()
    {
        if (Time.time > DT)
        {
            DT = Time.time + dt;
            if (m_startDrag && getAcceleration() < 2)
            {
                m_startPosition = Input.mousePosition;
            }
        }
        if (m_startDrag)
        {
            m_rb.position = getPosition();
        }
        if (m_rb.position.y < 0)
            die();
        else if (m_rb.position.y < 0.7)
        {
            if(DisableCupIfWin())
                die();
            if (Mathf.Abs(m_rb.velocity.magnitude) < 2 || (m_rb.velocity.z<0 && m_rb.velocity.y == 0))
                die();
        }
    }
    
}
