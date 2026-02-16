using System.Collections.Generic;
using System.Linq;
using UnityEditor.Tilemaps;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class scout_movement : MonoBehaviour
{
    public float energyLossPerSecond = 1f;
    public Rigidbody2D rb;
    public Animator anim;
    public int facingDirection = 1;
    public bool moving;

    public ResourceManager resourceManager;

    private Vector2 direction;
    private float currentEnergy;

    public float visionRadius = 8f;


    private void Awake()
    {
//        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    void Update()
    {
        FogOfWarManager.Instance.RevealArea(transform.position, visionRadius);
    }


    private void FixedUpdate()
    {
        if (!moving)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("moving", false);
            return;
        }

        if (moving)
        {
            anim.SetBool("moving", true);
        }

        if (direction[0] > 0 && transform.localScale.x > 0 ||
            direction[0] < 0 && transform.localScale.x < 0)
        {
            Flip();
        }


        currentEnergy -= energyLossPerSecond * Time.fixedDeltaTime;

        if (currentEnergy <= 0f)
        {
            Stop();
        }

    }

    private void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }



    public void Stop()
    {
        moving = false;

        currentEnergy = 0f;
        rb.linearVelocity = Vector2.zero;
        
    }

    public void Shoot(Vector2 shootDirection)
    {
        direction = shootDirection.normalized;
        currentEnergy = resourceManager.scoutEnergy;
        moving = true;

        rb.linearVelocity = direction * resourceManager.scoutSpeed;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!moving)
            return;

        Vector2 normal = collision.contacts[0].normal;
        direction = Vector2.Reflect(direction, normal).normalized;

        rb.linearVelocity = direction * resourceManager.scoutSpeed;

        currentEnergy += resourceManager.scoutBounceGain;

        if (currentEnergy <= 0f)
            Stop();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!moving)
            return;
        
        if (collision.CompareTag("Danger"))
        {
            DangerTile danger = collision.GetComponent<DangerTile>();
            danger.DestroyScout(gameObject);
            return;
        }

        ResourceSource source = collision.GetComponent<ResourceSource>();
        ScoutTrail trail = GetComponent<ScoutTrail>();

        List<Vector2> path = trail.GetPath();

        Vector2 snapPoint = collision.bounds.center;
        path.Add(snapPoint);

        PathDatabase.Instance.AddPath(path, source);


        Stop();

        Debug.Log("Triggered by: " + collision.name);


    }
}
