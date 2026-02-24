using UnityEngine;

public class EnemyAntMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 8f;

    [Header("Combat")]
    public float attackRange = 0.2f;
    public float antDetectionRange = 5f;
    public LayerMask playerLayer;
    public Transform detectionPoint;

    [Header("Patrol")]
    public Transform[] patrolPoints;   // Leave empty for stationary enemy
    public float waypointTolerance = 0.05f;

    private Rigidbody2D rb;
    private Transform ant;
    private ScoutCombat scoutCombat;
    private scout_movement scoutMovement;
    private Vector2 spawnPosition;

    private int currentPatrolIndex = 0;

    public EnemyState enemyState;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spawnPosition = rb.position;

        if (patrolPoints != null && patrolPoints.Length > 0)
            enemyState = EnemyState.Patrolling;
        else
            enemyState = EnemyState.Idle;
    }

    void Update()
    {
        CheckForAnt();
    }

    void FixedUpdate()
    {
        switch (enemyState)
        {
            case EnemyState.Patrolling:
                Patrol();
                break;

            case EnemyState.Chasing:
                MoveTowards(ant.position);
                break;

            case EnemyState.ReturningToSpawn:
                MoveTowards(spawnPosition);
                if (Vector2.Distance(rb.position, spawnPosition) <= waypointTolerance)
                    enemyState = EnemyState.Idle;
                break;

            case EnemyState.Attacking:  // currently not used, but will be implemented once health system is in place
                Debug.Log("Attacking Ant!");
                AttackAnt();
                break;

            case EnemyState.Idle:
                rb.MovePosition(rb.position);
                break;
        }
    }

    // =========================
    // PATROL LOGIC
    // =========================

    private void Patrol()
    {
        if (patrolPoints.Length == 0)
        {
            enemyState = EnemyState.Idle;
            return;
        }

        Vector2 target = patrolPoints[currentPatrolIndex].position;
        MoveTowards(target);

        if (Vector2.Distance(rb.position, target) <= waypointTolerance)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            Debug.Log("Reached patrol point, moving to next...");
        }
    }


    // =========================
    // MOVEMENT
    // =========================

    private void MoveTowards(Vector2 target)
    {
        Vector2 newPosition = Vector2.MoveTowards(
            rb.position,
            target,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPosition);

        // Attack check while chasing
        if (enemyState == EnemyState.Chasing &&
            ant != null &&
            Vector2.Distance(rb.position, ant.position) <= attackRange)
        {
            //scoutCombat.DestroyScout(); //to be changed when i get the health system working 
            //enemyState = EnemyState.Attacking;
            AttackAnt();
        }
    }

    // =========================
    // COMBAT
    // =========================

    private void AttackAnt()
    {
        if (ant != null && scoutCombat != null)
        {
            scoutCombat.DestroyScout();
        }
        
        rb.MovePosition(rb.position);
    }

    // =========================
    // DETECTION
    // =========================

    private void CheckForAnt()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            detectionPoint.position,
            antDetectionRange,
            playerLayer
        );

        if (hits.Length > 0)
        {
            ant = hits[0].transform;
            scoutCombat = ant.GetComponent<ScoutCombat>();
            scoutMovement = ant.GetComponent<scout_movement>();

            if (scoutMovement != null && Vector2.Distance(scoutMovement.spawnPoint, (Vector2)ant.transform.position) > waypointTolerance)
            {
                enemyState = EnemyState.Chasing;
            }
            else if (Vector2.Distance(scoutMovement.spawnPoint, (Vector2)ant.transform.position) < waypointTolerance && patrolPoints.Length > 0)
            {
                enemyState = EnemyState.Patrolling;
            }
            else
            {
                enemyState = EnemyState.ReturningToSpawn;
            }
        }
        else
        {
            ant = null;

            if (patrolPoints != null && patrolPoints.Length > 0)
                enemyState = EnemyState.Patrolling;
            else if (Vector2.Distance(rb.position, spawnPosition) > 0.05f)
                enemyState = EnemyState.ReturningToSpawn;
            else
                enemyState = EnemyState.Idle;
        }
    }

    // =========================
    // DEBUG
    // =========================

    private void OnDrawGizmosSelected()
    {
        if (detectionPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(detectionPoint.position, antDetectionRange);
            Gizmos.DrawWireSphere(detectionPoint.position, attackRange);
        }

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Gizmos.color = Color.green;

            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    Gizmos.DrawSphere(patrolPoints[i].position, 0.1f);

                    if (i + 1 < patrolPoints.Length && patrolPoints[i + 1] != null)
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                }
            }
        }
    }
}

public enum EnemyState
{
    Idle,
    Patrolling,
    ReturningToSpawn,
    Chasing,
    Attacking
}