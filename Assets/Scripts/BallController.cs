﻿using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour {

    public float forceFactor;
    public float x_forceFactor;
    public float z_forceFactor;
    public float angle;
    public float minDistance;
    public float k;
    
    private Vector3 m_startPosition;
    private Rigidbody m_rb;
    private Vector3 m_ballRespawn;
    private bool m_startDrag;
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_startDrag = false;
        m_ballRespawn = m_rb.position;
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
        m_startPosition = Input.mousePosition;
        m_startDrag = true;
    }

    Vector3 getPosition()
    {
        Vector3 viewPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        return Camera.main.ViewportToWorldPoint(new Vector3(viewPos.x, viewPos.y, k));
    }

    float getAngle()
    {
        return (Mathf.PI/180)*angle;
    }

    void OnMouseUp()
    {
        Vector3 endPosition = Input.mousePosition;
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
        m_rb.isKinematic = false;
        m_startDrag = false;
        m_rb.velocity = new Vector3(Vx, Vy, Vz);
    }
    void die()
    {
        m_rb.isKinematic = true;
        
        m_rb.velocity = new Vector3(0, 0, 0);
        m_rb.position = m_ballRespawn;
    }
    void Update()
    {
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
            if (Mathf.Abs(m_rb.velocity.magnitude) < 2)
                die();
        }
    }
    
}
