using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHandler : MonoBehaviour
{
    [SerializeField] private GameObject m_Player;
    [SerializeField] private float m_AttackDistance;
    [SerializeField] private int m_AttackDamage;
    [SerializeField] private float m_AttackDelay;
    [SerializeField] private float m_SpotDistance;
    [SerializeField] private int m_EnemyHealth;
    [SerializeField] private GameHUD m_GameHUD;
    private bool m_CanAttack = true;
    private float m_FacePlayerSpeed = 5;
    private PlayerController m_PlayerController;
    private Animator m_Animator;
    private NavMeshAgent m_Agent;
    private TimersHandler m_AttackTimer;
    private bool m_SpottedPlayer;
    private float m_AgentOriginalSpeed;
    private float m_VelocityToAnimationSpeedMultiplier = 4;
    private bool m_IsDead;
    private float m_DeathDespawnTimer = 5;

    void Start()
    {
        m_PlayerController = m_Player.GetComponent<PlayerController>();
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
        m_AttackTimer = gameObject.AddComponent<TimersHandler>();
        m_AttackTimer.m_Duration = m_AttackDelay;
        m_Agent.updateRotation = false; // Je gère déja la rotation.
        m_Agent.stoppingDistance = m_AttackDistance;
        m_AgentOriginalSpeed = m_Agent.speed;
    }

    void Update()
    {
        if (m_IsDead) return;
        //Gérer la logique du spot et follow
        FollowPlayerIfNearby();

        if (!m_SpottedPlayer) return;
        HandleAttackTimers();
        FacePlayer();
        if (Vector3.Distance(transform.position, m_Player.transform.position) < m_AttackDistance)
        {
            if (m_CanAttack)
            {
                m_CanAttack = false;
                //Un event dans l'animation va appeler AttackPlayer()
                m_Animator.SetTrigger("Attack");
            }
        }
    }

    void AttackPlayer()
    {
        m_PlayerController.TakeDamage(m_AttackDamage);

        //NOTE: J'ai fait ca pour garder une balance entre le tir et le punch.
        //D'autres balances sont ajoutés aussi, je les ai commentés.
        //C'est a dire: Laisser une chance au joueur de partir soit pour s'enfuir ou bien prendre une bonne distance pour tirer.
        //Le punch est une touche personnelle que je voulais ajouter.
        m_Agent.speed = 0.5f;
    }

    void HandleAttackTimers()
    {
        if (m_CanAttack) return;
        if (!m_AttackTimer.IsActive()) m_AttackTimer.StartTimer();
        if (m_AttackTimer.UpdateTimer())
        {
            m_CanAttack = true;
            m_Agent.speed = m_AgentOriginalSpeed;
        }
    }

    void OnDeath()
    {
        m_Agent.enabled = false;
        GetComponent<Collider>().enabled = false;
        m_Animator.SetTrigger("Die");
        m_PlayerController.DeselectEnemy();
        Destroy(gameObject, m_DeathDespawnTimer);
    }

    public void TakeDamage(int Damage)
    {
        if (m_IsDead) return;
        m_EnemyHealth -= Damage;
        if (m_EnemyHealth <= 0) {
            m_EnemyHealth = 0;
            m_IsDead = true;
            OnDeath();
        };
        m_GameHUD.NotifyEnemyHealth(m_EnemyHealth);
    }

    void FacePlayer()
    {
        //Le même code que celui dans le player.
        Vector3 RelativePos = m_Player.transform.position - transform.position;

        //Cette ligne fait que mon joueur regarde vers l'ennemi sur tous les axes.
        Quaternion TargetRotation = Quaternion.LookRotation(RelativePos, Vector3.up);

        //Ces 2 lignes font qu'il regarde l'ennemi seulement sur l'axe des y.
        float TargetY = TargetRotation.eulerAngles.y;
        TargetRotation = Quaternion.Euler(0, TargetY, 0);

        //Ca fait une variable AngleDifference qui va dire les 2 angles sont proches comment
        float CurrentY = transform.eulerAngles.y;
        float AngleDifference = Mathf.Abs(Mathf.DeltaAngle(CurrentY, TargetY));

        //C'est pour pas que le lerp continue a l'infini (si les 2 angles sont assez proches, le lerp s'arrete).
        if (AngleDifference > 0.5f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, TargetRotation, Time.deltaTime * m_FacePlayerSpeed);
        }
        else
        {
            transform.rotation = TargetRotation;
        }
    }

    public void MoveToPlayer()
    {
        if (m_SpottedPlayer) return;
        m_Agent.speed = m_AgentOriginalSpeed; //Si il étais ralenti, je veut qu'il cours vers le joueur plutot que de marcher quand on lui tire dessu.
        m_Agent.SetDestination(m_Player.transform.position);
    } 
    //Cette fonction est pour appeler dans le script de projectile.
    //Je veut que si une balle touche l'ennemi mais qu'il n'a pas spot le player, il va se deplacer vers sa position.

    public int GetHealth()
    {
        return m_EnemyHealth;
    }

    void FollowPlayerIfNearby()
    {
        float DistanceToPlayer = Vector3.Distance(transform.position, m_Player.transform.position);

        if (DistanceToPlayer <= m_SpotDistance)
        {
            //Le code pour si le player a été "spotté"
            m_SpottedPlayer = true;
            m_Agent.SetDestination(m_Player.transform.position);
        }
        else
        {
            //Le code pour si il n'a pas été "spotté"
            m_SpottedPlayer = false;
        }
        m_Animator.SetFloat("Speed", m_Agent.velocity.magnitude * m_VelocityToAnimationSpeedMultiplier); // Le multiplier c'est pour qu'il puisse run et pas juste walk
    }
}
