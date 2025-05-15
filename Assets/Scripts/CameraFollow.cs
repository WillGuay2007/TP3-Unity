using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Vector3 m_CameraPlayerOffset;
    [SerializeField] private float m_LerpPercentage;
    [SerializeField] private GameObject m_Player;
    // Start is called before the first frame update
    void Start()
    {
        m_CameraPlayerOffset = transform.position - m_Player.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, m_Player.transform.position + m_CameraPlayerOffset, m_LerpPercentage * Time.deltaTime);
    }
}
