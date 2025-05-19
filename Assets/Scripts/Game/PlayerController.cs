using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject m_ProjectilePrefab;
    [SerializeField] private GameObject m_Gun;
    [SerializeField] private float m_ShootDistance;
    [SerializeField] private float m_ShootDelay;
    [SerializeField] private float m_PunchDelay;
    [SerializeField] private float m_ProjectileSpeed;
    [SerializeField] private float m_PunchDistance;
    [SerializeField] private AudioHandler m_AudioHandler;
    [SerializeField] private float m_RunSoundsDelay;
    private TimersHandler m_ShootTimer;
    private TimersHandler m_PunchTimer;
    private TimersHandler m_RunSoundsTimer;
    private bool m_IsRunning;
    private bool m_CanPunch = true;
    private bool m_IsPunchingLeft;
    private float m_FaceEnemySpeed = 5;
    private GameObject m_CurrentEnemyTarget;
    private NavMeshAgent m_Agent;
    private Animator m_Animator;
    private bool m_CanShoot = true;

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();

        m_ShootTimer = gameObject.AddComponent<TimersHandler>();
        m_ShootTimer.m_Duration = m_ShootDelay;
        m_PunchTimer = gameObject.AddComponent<TimersHandler>();
        m_PunchTimer.m_Duration = m_PunchDelay;
        m_RunSoundsTimer = gameObject.AddComponent<TimersHandler>();
        m_RunSoundsTimer.m_Duration = m_RunSoundsDelay;
    }

    void Update()
    {
        HandleMouseClick();
        HandleAnimations();
        HandleAttackTimers();
        AttackCurrentEnemy();
    }

    void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit info))
            {

                if (info.collider.gameObject.tag == "Enemy")
                {
                    //Faire que l'ennemi est plus null si le ray en touche un.
                    //Donc ca va set une nouvelle cible que le joueur va track.
                    m_CurrentEnemyTarget = info.collider.gameObject;
                    m_Agent.stoppingDistance = m_ShootDistance;
                } else
                {
                    //Si il touche pas un ennemi, il n'aura pas/plus de cible a track et il va se deplacer au point exact.
                    m_CurrentEnemyTarget = null;
                    m_Agent.stoppingDistance = 0;
                }

                //Gérer le déplacement
                m_Agent.destination = info.point;

            };
        }
    }

    void HandleAnimations()
    {
        m_Animator.SetFloat("Vertical", m_Agent.velocity.magnitude);
        //Je set son vertical pour definir si il run ou non

        //Il va courir si il n'a pas atteint son but. Sinon, il walk -> idle.
        if (m_Agent.remainingDistance <= m_Agent.stoppingDistance) { m_Animator.SetBool("running", false); m_IsRunning = false; }
        else { m_Animator.SetBool("running", true); m_IsRunning = true; }

        if (m_IsRunning)
        {
            if (!m_RunSoundsTimer.IsActive())
            {
                m_RunSoundsTimer.StartTimer();
            } else
            {
                if (m_RunSoundsTimer.UpdateTimer())
                {
                    m_AudioHandler.PlayRunSound();
                }
            }
        }
    }

    void HandleAttackTimers()
    {
        if (!m_CanShoot)
        {
            if (m_ShootTimer.UpdateTimer())
            {
                m_CanShoot = true;
            }
        }

        if (!m_CanPunch)
        {
            if (m_PunchTimer.UpdateTimer())
            {
                m_CanPunch = true;
            }
        }
    }

    void AttackCurrentEnemy()
    {
        //Si il a atteint la shooting distance, il va s'arrêter puis tirer
        //pathPending c'est pour eviter le bug que remainingDistance donne 0 même si c'est pas ca.
        if (m_CurrentEnemyTarget != null && m_Agent.remainingDistance <= m_ShootDistance && !m_Agent.pathPending)
        {
            FaceEnemy();
            if (Vector3.Distance(transform.position, m_CurrentEnemyTarget.transform.position) < m_PunchDistance)
            {
                //Si il est assez proche il va punch (je sais que ce n'étais pas censé être une feature mais je crois que ca ne fait pas de mal!)
                if (m_CanPunch) m_CanPunch = false; else return;
                m_IsPunchingLeft = !m_IsPunchingLeft;
                if (m_IsPunchingLeft) m_Animator.SetTrigger("punch_L"); else m_Animator.SetTrigger("punch_R");
                m_PunchTimer.StartTimer();
                m_AudioHandler.PlayPunchSound();
            } else
            {
                if (m_CanShoot) m_CanShoot = false; else return;
                //J'ai mit un event dans l'animation donc Shoot() va se faire appeler.
                m_ShootTimer.StartTimer();
                m_Animator.SetTrigger("shoot");
            }
        }
    }

    void Shoot()
    {
        if (m_CurrentEnemyTarget == null) return; // Pour être 100% sur qu'il existe
        Vector3 ShootDirection = (m_CurrentEnemyTarget.transform.position - m_Gun.transform.position).normalized;

        GameObject Projectile = Instantiate(m_ProjectilePrefab, m_Gun.transform.position, Quaternion.identity);

        ProjectileHandler ProjHandler = Projectile.GetComponent<ProjectileHandler>();
        ProjHandler.Initialize(ShootDirection, m_ProjectileSpeed, m_CurrentEnemyTarget);
        m_AudioHandler.PlayShootSound();

    }

    void FaceEnemy()
    {
        Vector3 RelativePos = m_CurrentEnemyTarget.transform.position - transform.position;

        //Cette ligne fait que mon joueur regarde vers l'ennemi sur tous les axes.
        Quaternion TargetRotation = Quaternion.LookRotation(RelativePos, Vector3.up);

        //Ces 2 lignes font qu'il regarde l'ennemi seulement sur l'axe des y.
        float TargetY = TargetRotation.eulerAngles.y;
        TargetRotation = Quaternion.Euler(0, TargetY, 0);

        //Ca fait une variable AngleDifference qui va dire les 2 angles sont proches comment
        float CurrentY = transform.eulerAngles.y;
        float NewY = TargetRotation.eulerAngles.y;
        float AngleDifference = Mathf.Abs(Mathf.DeltaAngle(CurrentY, TargetY));

        //C'est pour pas que le lerp continue a l'infini (si les 2 angles sont assez proches, le lerp s'arrete).
        if (AngleDifference > 0.5f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, TargetRotation, Time.deltaTime * m_FaceEnemySpeed);
        }
        else
        {
            transform.rotation = TargetRotation;
        }
    }

}
