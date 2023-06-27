using System;
using System.Collections;
using UnityEngine;
using TMPro;

public struct NitroBoosts
{
    public float Duration;
    public float Modifier;
    public float TimeRemaining;
    public int Charges;
    public NitroBoosts(float duration, float modifier, float timeRemaining, int charges)
    {
        Duration = duration;
        Modifier = modifier;
        TimeRemaining = timeRemaining;
        Charges = charges;
    }
}

public struct BoP
{
    public float Duration;
    public float TimeRemaining;
    public int Charges;
    public BoP(float duration, float timeRemaining, int charges)
    {
        Duration = duration;
        TimeRemaining = timeRemaining;
        Charges = charges;
    }
}

public class PlayerCharacter : MonoBehaviour
{
    [Tooltip("Player's base run speed in y/s.")]
    public float baseRunSpeed = 7.0f;
    public float speedCoefficient = 1.0f;

    public NitroBoosts nitro = new(5.0f, 1.5f, 0.0f, 1);
    public BoP bop = new(10.0f, 0.0f, 1);

    public bool isPlayer = true;
    public bool isVulnerable = true;
    
    private float pixelsPerYard = 7.0f;
    private GameObject _bopIcon;
    private GameObject _nitroIcon;

    public GameObject BoPIcon { get => _bopIcon; set => _bopIcon = value; }
    public GameObject NitroIcon { get => _nitroIcon; set => _nitroIcon = value; }

    public LineRenderer circleRenderer;
    private Rigidbody2D rigidBody;
    private GlobalState globalState;
    private InputManager inputManager;

    // Start is called before the first frame update
    void Start()
    {
        globalState = GameObject.Find("GlobalState").GetComponent<GlobalState>();
        inputManager = globalState.inputManager;
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var movementVector = new Vector2(
            inputManager.GetAxis("Horizontal"),
            inputManager.GetAxis("Vertical")
        );

        HandleNitroBoosts();
        HandleBoP();
        
        rigidBody.velocity = movementVector.normalized * Time.fixedDeltaTime * baseRunSpeed * pixelsPerYard * speedCoefficient;
    }

    void OnDestroy()
    {
        if (BoPIcon != null) { BoPIcon.SetActive(false); }
        if (NitroIcon != null) { NitroIcon.SetActive(false); }
    }
    
    void LateUpdate()
    {
        DrawBoPCircle(100, 0.18f);
    }

    void HandleNitroBoosts()
    {
        nitro.TimeRemaining = Math.Max(nitro.TimeRemaining - Time.fixedDeltaTime, 0.0f);

        if (nitro.Charges > 0 && inputManager.GetKey("1"))
        {
            nitro.Charges -= 1;
            nitro.TimeRemaining = nitro.Duration;
            speedCoefficient += nitro.Modifier;
            NitroIcon.SetActive(true);
            StartCoroutine(EndNitroBoosts());
        }

        if (NitroIcon.activeInHierarchy)
        {
            NitroIcon.GetComponentInChildren<TMP_Text>().text = String.Format("{0:F1}", nitro.TimeRemaining);
        }
    }

    private IEnumerator EndNitroBoosts()
    {
        yield return new WaitForSeconds(nitro.Duration);
        speedCoefficient -= nitro.Modifier;
        NitroIcon.SetActive(false);
    }
    
    void HandleBoP()
    {
        bop.TimeRemaining = Math.Max(bop.TimeRemaining - Time.fixedDeltaTime, 0.0f);

        if (bop.Charges > 0 && inputManager.GetKey("2"))
        {
            bop.Charges -= 1;
            bop.TimeRemaining = bop.Duration;
            isVulnerable = false;
            BoPIcon.SetActive(true);
            StartCoroutine(EndBoP());
        }

        if (BoPIcon.activeInHierarchy)
        {
            BoPIcon.GetComponentInChildren<TMP_Text>().text = String.Format("{0:F1}", bop.TimeRemaining);
        }
    }

    private void DrawBoPCircle(int steps, float radius)
    {
        if (bop.TimeRemaining <= 0.0f) { return; }
        
        circleRenderer.positionCount = steps;
        
        for(int currentStep = 0; currentStep < steps; currentStep++)
        {
            float circumferenceProgress = (float) currentStep / steps;
            float currentRadian = circumferenceProgress * 2 * Mathf.PI;
            float xPos = Mathf.Cos(currentRadian) * radius;
            float yPos = Mathf.Sin(currentRadian) * radius;

            var pos = new Vector2(xPos, yPos) + (Vector2) transform.position;
            circleRenderer.SetPosition(currentStep, pos);
        }
    }

    private IEnumerator EndBoP()
    {
        yield return new WaitForSeconds(bop.Duration);

        var anubArak = GameObject.Find("Anub'arak").GetComponent<Collider2D>();
        if (GetComponent<Collider2D>().IsTouching(anubArak))
        {
            globalState.LoseBecauseDie();
        }
        
        circleRenderer.positionCount = 0;
        isVulnerable = true;
        BoPIcon.SetActive(false);
    }
}
