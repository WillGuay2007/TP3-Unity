using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHandler : MonoBehaviour
{
    [SerializeField] private int m_ProjectileDamage;
    private float m_Speed;
    private GameObject m_CurrentEnemyTarget;
    private Vector3 m_Direction;
    private bool m_IsInitialised = false;
    private float m_DestroyDistance = 100;
    private float m_MaxSpeed = 25;

    void Update()
    {
        //Si le projectile est initialisť, update la position
        if (!m_IsInitialised) return;
        transform.position += m_Direction * m_Speed * Time.deltaTime;
        if (Vector3.Distance(transform.position, m_CurrentEnemyTarget.transform.position) > m_DestroyDistance) Destroy(gameObject);
    }

    public void Initialize(Vector3 Direction, float Speed, GameObject Target)
    {
        m_Direction = Direction;
        m_CurrentEnemyTarget = Target;

        //Puisqu'on travaille avec transform, je veut m'assurer que la balle n'aye pas trop vite.
        if (Speed > m_MaxSpeed) m_Speed = m_MaxSpeed; else m_Speed = Speed;

        m_IsInitialised = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            EnemyHandler EnemyHandlerScript = other.gameObject.GetComponent<EnemyHandler>();
            EnemyHandlerScript.MoveToPlayer(); //Pour pas qu'il reste la a rien faire
            EnemyHandlerScript.TakeDamage(m_ProjectileDamage);
            Destroy(gameObject);
        }
        //Sinon, la balle passe a travers.
    }
}
