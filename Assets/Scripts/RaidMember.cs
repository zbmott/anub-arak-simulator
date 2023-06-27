using System.Collections;
using UnityEngine;

public enum RaidMemberState
{
    Active,
    Deciding,
    Waiting
}

public class RaidMember : MonoBehaviour
{
    public float speed = 5.0f;
    public float pixelsPerYard = 7.0f;
    public Vector3 startLocation;
    public Vector2 destination;
    public RaidMemberState currentState = RaidMemberState.Deciding;
    
    private Rigidbody2D rigidBody;

    void Start()
    {
        startLocation = startLocation == Vector3.zero ? transform.position : startLocation;
        rigidBody = GetComponent<Rigidbody2D>();
        destination = Vector2.zero;
    }
    
    void FixedUpdate()
    {
        switch (currentState)
        {
            case RaidMemberState.Deciding:
                HandleDeciding();
                break;
            case RaidMemberState.Active:
                HandleActive();
                break;
            case RaidMemberState.Waiting:
                break;
        }
    }

    void HandleDeciding()
    {
        if (Random.value <= 0.33f)
        {
            StartCoroutine(Wait(2));
        }
        else
        {
            destination = Random.insideUnitCircle;

            if ((transform.position - startLocation).magnitude >= 1.0f)
            {
                destination += (Vector2) startLocation;
            }
            else
            {
                destination += (Vector2) transform.position;
            }

            currentState = RaidMemberState.Active;
        }

    }
    
    void HandleActive()
    {
        if (((Vector2)transform.position - destination).magnitude < 0.01f)
        {
            currentState = RaidMemberState.Deciding;
            return;
        }
        
        var currentSpeed = Time.fixedDeltaTime * speed * pixelsPerYard;
        rigidBody.velocity = (destination - (Vector2) transform.position).normalized * currentSpeed;
    }

    IEnumerator Wait(int duration)
    {
        currentState = RaidMemberState.Waiting;
        rigidBody.velocity = Vector2.zero;
        yield return new WaitForSeconds(duration);
        currentState = RaidMemberState.Deciding;
    }
}
