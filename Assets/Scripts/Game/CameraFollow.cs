using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Vector3 m_CameraPlayerOffset;
    [SerializeField] private float m_LerpPercentage;
    [SerializeField] private GameObject m_Player;
    [SerializeField] private float m_StopDistance;


    void Start()
    {
        m_CameraPlayerOffset = transform.position - m_Player.transform.position;
    }

    void LateUpdate()
    {
        //Le goal de la camera
        Vector3 targetPosition = m_Player.transform.position + m_CameraPlayerOffset;

        //La distance de la camera avec son goal
        float distance = Vector3.Distance(transform.position, targetPosition);

        //Si la distance est plus base que la StopDistance, alors la caméra va se mettre au goal, sinon, elle continue de se rapprocher du goal
        if (distance > m_StopDistance)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, m_LerpPercentage * Time.deltaTime);
        }
        else
        {
            transform.position = targetPosition;
        }
    }
}

