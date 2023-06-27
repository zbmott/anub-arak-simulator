using System.Collections;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class GlobalState : MonoBehaviour
{
    public enum GameState {
        Active,
        Paused,
        Completed,
    }
    
    public float elapsedTime = 0.0f;
    public int alliesRemaining = 24;
    
    public GameObject overlay;
    public TMP_Text instructionsText;
    public TMP_Text winText;
    public TMP_Text loseText;
    public TMP_Text alliesRemainingText;
    public TMP_Text timerText;
    public IcePatch[] icePatches;
    public InputManager inputManager;
    public GameState currentState = GameState.Paused;
    public GameObject pcPrefab;
    public GameObject raidMemberPrefab;
    public GameObject bopIcon;
    public GameObject nitroIcon;

    private bool isRestarting = false;
    
    void Start()
    {
        PauseGame();
        SpawnPlayerCharacter();

        var screenDims = Screen.currentResolution;
        var canvasTransform = overlay.transform.parent.GetComponent<RectTransform>();
        canvasTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, screenDims.width);
        canvasTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, screenDims.height);
    }
    
    void Update()
    {
        elapsedTime += Time.deltaTime;
        timerText.text = $"Time: {elapsedTime:F1}";
        alliesRemainingText.text = $"Allies left: {alliesRemaining:D}";

        if (elapsedTime >= 60.0)
        {
            WinBecauseTime();
        }
        
        switch (currentState)
        {
            case GameState.Paused:
                if (inputManager.GetKey("w"))
                {
                    overlay.SetActive(false);
                    instructionsText.gameObject.SetActive(false);

                    SpawnIcePatch();
                    
                    UnpauseGame();
                }
                break;
            case GameState.Completed:
                if (!isRestarting)
                {
                    StopAllCoroutines();
                    StartCoroutine(RestartGame(5));
                }
                break;
                
        }
    }

    public bool IsPaused => currentState == GameState.Paused;
    public bool IsActive => currentState == GameState.Active;
    public bool IsCompleted => currentState == GameState.Completed;

    public void UnpauseGame()
    {
        Time.timeScale = 1.0f;
        currentState = GameState.Active;
    }
    
    public void PauseGame()
    {
        Time.timeScale = 0.0f;
        currentState = GameState.Paused;
    }

    public void CompleteGame()
    {
        Time.timeScale = 0.0f;
        currentState = GameState.Completed;
    }

    public void SpawnIcePatch()
    {
        icePatches = icePatches.Where(i => i.enabled).ToArray();

        var jitter = Random.insideUnitCircle * 2.0f;
        var icePatch = icePatches[Random.Range(0, icePatches.Length)].gameObject;
        icePatch.transform.Translate(jitter);
        icePatch.SetActive(true);
    }

    public void UnspawnPlayerCharacter()
    {
        var playerCharacter = FindObjectOfType<PlayerCharacter>();
        var raidMember = Instantiate(raidMemberPrefab, playerCharacter.transform.position, Quaternion.identity);

        raidMember.GetComponent<SpriteRenderer>().color = playerCharacter.GetComponent<SpriteRenderer>().color;

        var startLocation = new Vector3(-0.5f, 0.5f, 0.0f) + (Vector3)Random.insideUnitCircle;
        var raidMemberController = raidMember.GetComponent<RaidMember>();
        raidMemberController.startLocation = startLocation;
        raidMemberController.currentState = RaidMemberState.Active;
        
        Destroy(playerCharacter.gameObject);
    }

    public GameObject SpawnPlayerCharacter()
    {
        var raidMembers = FindObjectsOfType<RaidMember>();
        var target = raidMembers[Random.Range(0, raidMembers.Length)].gameObject;

        var playerCharacter = Instantiate(pcPrefab, target.transform.position, Quaternion.identity);
        var playerSpriteRenderer = playerCharacter.GetComponent<SpriteRenderer>();
        var playerController = playerCharacter.GetComponent<PlayerCharacter>();
        playerSpriteRenderer.color = target.GetComponent<SpriteRenderer>().color;
        playerController.BoPIcon = bopIcon;
        playerController.NitroIcon = nitroIcon;

        Destroy(target);

        return playerCharacter;
    }
    
    public void LoseBecauseTankPatch()
    {
        CompleteGame();

        var alliesRemainingStr = $"At least you didn't kill any of your friends.";
        if (alliesRemaining < 24)
        {
            alliesRemainingStr = $"{24 - alliesRemaining:D} other raid members died.";
        }
        
        loseText.text = $"You lose! You kited Anub'arak into an ice patch the tanks use during phase 1.\n\n"
                        + $"{alliesRemainingStr}\n\nChaos reigns!\n\nThe game will reload in 5 seconds.";
        overlay.SetActive(true);
        loseText.gameObject.SetActive(true);
    }

    public void LoseBecauseDie()
    {
        CompleteGame();
        
        var alliesRemainingStr = $"At least he didn't kill any of your allies.";
        if (alliesRemaining < 24)
        {
            alliesRemainingStr = $"He also killed {24 - alliesRemaining:D} of your friends.";
        }
        
        loseText.text = $"You lose! Anub'arak impaled you to death!\n\n{alliesRemainingStr}\n\nYou "
                        + $"survived for {elapsedTime:F1} seconds.\n\nThe game will reload in 5 seconds.";
        overlay.SetActive(true);
        loseText.gameObject.SetActive(true);
    }

    public void WinBecauseTime()
    {
        CompleteGame();

        winText.text =$"You win!\n\nYou survived for {elapsedTime:F1} seconds!\n\n{alliesRemaining:D} of your allies survived!\n\nThe game will reload in 5 seconds.";
        overlay.SetActive(true);
        winText.gameObject.SetActive(true);
    }

    public IEnumerator RestartGame(int delay)
    {
        isRestarting = true;
        yield return new WaitForSecondsRealtime(delay);
        SceneManager.LoadScene("MainScene");
    }
}
