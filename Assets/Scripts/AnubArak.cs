using System.Collections;
using UnityEngine;

public enum AnubArakState
{
    Chasing,
    Stunned
}

public struct SpeedUp
{
    public float Interval;
    public float Cooldown;
    public float Increment;
    
    public SpeedUp(float interval, float increment)
    {
        Interval = interval;
        Cooldown = interval;
        Increment = increment;
    }
}

public class AnubArak : MonoBehaviour
{
    
    public float baseRunSpeed = 5.0f;
    public float pixelsPerYard = 7.0f;
    public float speedCoefficient = 1.0f;
    
    public SpeedUp speedUp = new(4.0f, 0.5f);

    private GameObject target;
    private Rigidbody2D rigidBody;
    private GlobalState globalState;
    private AnubArakState currentState = AnubArakState.Chasing;
    
    // Start is called before the first frame update
    void Start()
    {
        globalState = GameObject.Find("GlobalState").GetComponent<GlobalState>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (currentState)
        {
            case AnubArakState.Chasing:
                HandleChasing();
                break;
            case AnubArakState.Stunned:
                break;
        }

    }

    void HandleChasing()
    {
        handleSpeedUp();
        
        var currentSpeed = Time.fixedDeltaTime * baseRunSpeed * speedCoefficient * pixelsPerYard;
        
        if (target is null) { target = acquireTarget(); }

        var vectorDiff = target.transform.position - transform.position;
        if (vectorDiff.magnitude < 0.05f)
        {
            rigidBody.velocity = Vector2.zero;
        }
        else
        {
            rigidBody.velocity = vectorDiff.normalized * currentSpeed;
        }

    }

    IEnumerator Stun(int duration)
    {
        currentState = AnubArakState.Stunned;
        rigidBody.velocity = Vector2.zero;
        speedCoefficient = 1.0f;
        globalState.SpawnIcePatch();
        globalState.UnspawnPlayerCharacter();

        yield return new WaitForSeconds(duration);
        
        target = globalState.SpawnPlayerCharacter();
        currentState = AnubArakState.Chasing;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (currentState == AnubArakState.Stunned) { return; }
        
        var dunkHitBox = GameObject.Find("DunkHitBox").GetComponent<Collider2D>();
        
        var icePatch = other.gameObject.GetComponent<IcePatch>();
        if (icePatch is not null && dunkHitBox.IsTouching(other))
        {
            if (icePatch.isTankingPatch) { globalState.LoseBecauseTankPatch(); return; }

            globalState.WinBecauseOnePointOh();
            Destroy(icePatch.gameObject);
            // StartCoroutine(Stun(2));
        }

        var raidMember = other.gameObject.GetComponent<RaidMember>();
        if (raidMember is not null)
        {
            globalState.alliesRemaining -= 1;
            Destroy(other.gameObject);
        }
        
        var playerCharacter = other.gameObject.GetComponent<PlayerCharacter>();
        if (playerCharacter is not null && playerCharacter.isVulnerable)
        {
            globalState.LoseBecauseDie();
        }
    }
    
    private void handleSpeedUp()
    {
        speedUp.Cooldown -= Time.fixedDeltaTime;
        if (speedUp.Cooldown <= 0.0f)
        {
            speedCoefficient += speedUp.Increment;
            speedUp.Cooldown = speedUp.Interval;
        }
    }

    private GameObject acquireTarget()
    {
        var possibleTargets = FindObjectsOfType<PlayerCharacter>();
        foreach (var possibleTarget in possibleTargets)
        {
            if (possibleTarget.isPlayer)
            {
                return possibleTarget.gameObject;
            }
        }

        return null;
    }
}
