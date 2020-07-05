using UnityEngine;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    UIManager uiManager;
    GraphicsManager graphicsManager;

    Colour[] baseColorArr;
    public static Colour[] sessionColors;

    [HideInInspector]
    public Coroutine ToxinCoroutine, ChaosCoroutine, TimeDelayCoroutine, ShieldCoroutine, TurnOffSlowMoCoroutine;
    public bool toxinIsActive, chaosIsActive, timeDelayIsActive, shieldIsActive;

    public bool gameIsPaused;
    private float timeScaleBeforeThePause;
    public GameObject pauseScreen;
    public TMPro.TextMeshProUGUI countdownText;

    //SpriteAndColor[] bucketSprites;
    //SpriteAndColor[] sessionBucketSprites;

    SpriteAndColor[] selectedStyleSprites;

    public static float score = 0;
    public bool gameOver;
    public event Action OnLoseEvent;

    public GameObject loseScreen;
    public GameObject bucketPrefab;
    private Transform[] _bucketSpawns;
    public int goldenBallsCollected;

    void Awake()
    {
        RestoreBaseColorArray();
        ShuffleColorArray();
        GetSessionColors();
        graphicsManager = FindObjectOfType<GraphicsManager>();
        selectedStyleSprites = graphicsManager.currentlySelectedUserPrefsCollection.styleSprites;
        //bucketSprites = GetBucketSessionSprites() /*graphicsManager.currentlySelectedUserPrefsCollection.bucketSprites*/;
    }

    void Start()
    {
        //AssignSessionSprites();
        uiManager = FindObjectOfType<UIManager>();
        gameOver = false;
        loseScreen.SetActive(false);
        SpawnBuckets();
        GoldenBall.onGoldenBallCollect += OnGoldenBallCollect;
    }


    void Update()
    {
        if (gameOver)
        {
            if (OnLoseEvent != null)
            {
                SaveCurrentGameState();
                OnLoseEvent();
            }
        }
        if (!gameOver)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!gameIsPaused)
                {
                    PauseTheGame();

                }
                else
                {
                    UnpauseTheGame();
                }
            }

            if (!gameIsPaused)
            {
                score += CalculateScoreByTime();
            }
        }
    }


    //void AssignSessionSprites()
    //{
    //    sessionBucketSprites = new SpriteAndColor[4];
    //    for (int i = 0; i < sessionBucketSprites.Length; i++)
    //    {
    //        for (int j = 0; j < bucketSprites.Length; j++)
    //        {
    //            if (GameManager.sessionColors[i] == bucketSprites[j].color)
    //            {
    //                sessionBucketSprites[i] = bucketSprites[j];
    //            }
    //        }
    //    }
    //}

    float CalculateScoreByTime()
    {
        float delay = FindObjectOfType<UIManager>().delayBeforeTheGame;
        float correctedTime = Time.timeSinceLevelLoad - delay;
        if (correctedTime <= 15f)
        {
            return Time.deltaTime;
        }
        else
        {
            return Time.deltaTime * ((((int)correctedTime) / 15) + 1);
        }
    }

    public void IncreaseScoreOnBallColect()
    {
        float correctedTime = Time.timeSinceLevelLoad - FindObjectOfType<UIManager>().delayBeforeTheGame;
        if (correctedTime <= 6f)
        {
            score += BallGame.collectBallReward;
        }
        else
        {
            score += BallGame.collectBallReward + ((int)correctedTime / 6) * 2;
        }
    }

    public void ActivateEffect(Effect effect)
    {
        switch (effect.effectName)
        {
            case EffectName.Toxin:
                //Debug.Log("Toxin");
                ActivateToxin();
                break;
            case EffectName.Chaos:
                //Debug.Log("Chaos");
                ActivateChaos();
                break;
            case EffectName.TimeDelay:
                //Debug.Log("Time Delay");
                ActivateTimeDelay();
                break;
            case EffectName.Shield:
                //Debug.Log("Shield");
                ActivateShield();
                break;
        }

        FindObjectOfType<EffectsManager>().AddEffect(effect);
    }

    void ShuffleColorArray()
    {
        System.Random rnd = new System.Random();
        int n = baseColorArr.Length;
        while (n > 1)
        {
            n--;
            int k = rnd.Next(n + 1);
            Colour value = baseColorArr[k];
            baseColorArr[k] = baseColorArr[n];
            baseColorArr[n] = value;
        }
    }

    void GetSessionColors()
    {
        sessionColors = new Colour[4];
        for (int i = 0; i < sessionColors.Length; i++)
        {
            sessionColors[i] = baseColorArr[i];
        }
    }

    void ActivateToxin()
    {
        toxinIsActive = true;
        var buckets = FindBuckets();
        for (int i = 0; i < buckets.Length; i++)
        {
            buckets[i].transform.localScale = new Vector3(.9f, .9f, .9f);
        }
    }

    void ActivateChaos()
    {
        chaosIsActive = true;
    }

    void ActivateTimeDelay()
    {
        timeDelayIsActive = true;
        Time.timeScale = .8f;
        Time.fixedDeltaTime = Time.timeScale * .02f;
    }

    void ActivateShield()
    {
        shieldIsActive = true;
    }

    void SpawnBuckets()
    {
        GetBucketsSpawnPoints();
        for (int i = 0; i < _bucketSpawns.Length; i++)
        {
            GameObject newBucket = Instantiate(bucketPrefab, _bucketSpawns[i].transform.position, Quaternion.identity);
            newBucket.transform.SetParent(GameObject.Find("Buckets").transform);

            Sprite newBucketSprite = GetSpriteByColor(sessionColors[i]);

            newBucket.GetComponent<Bucket>().Initialise(sessionColors[i], newBucketSprite);
        }
    }

    Sprite GetSpriteByColor(Colour color)
    {
        foreach (var clr in selectedStyleSprites)
        {
            if (clr.color == color)
            {
                return clr.bucketSprite;
            }
        }

        return null;
    }

    void GetBucketsSpawnPoints()
    {
        Transform bucketsSpawnersHolder = GameObject.Find("BucketsSpawns").transform;
        _bucketSpawns = new Transform[bucketsSpawnersHolder.childCount];
        for (int i = 0; i < _bucketSpawns.Length; i++)
        {
            _bucketSpawns[i] = bucketsSpawnersHolder.GetChild(i);
        }
    }

    void OnGoldenBallCollect()
    {
        goldenBallsCollected += 1;
    }

    void SaveCurrentGameState()
    {
        JsonWrapper wrapper = new JsonWrapper();
        GameData data = new GameData();
        data.GoldenBallsCollected = goldenBallsCollected;
        data.skinSelected = 0;
        wrapper.gameData = data;
        wrapper.SaveData();
    }


    public GameObject[] FindBuckets()
    {
        var buckets = GameObject.FindGameObjectsWithTag("Bucket");
        return buckets;
    }

    public void PauseTheGame()
    {
        var textColor = countdownText.color;
        countdownText.color = new Color(textColor.r, textColor.g, textColor.b, 0);
        timeScaleBeforeThePause = Time.timeScale;
        Time.timeScale = 0f;
        pauseScreen.SetActive(true);
        gameIsPaused = true;
    }

    public void UnpauseTheGame()
    {
        var textColor = countdownText.color;
        countdownText.color = new Color(textColor.r, textColor.g, textColor.b, 1);
        StartCoroutine(LaunchCountdownBeforeUnpauseRoutine());
    }

    void RestoreBaseColorArray()
    {
        baseColorArr = new Colour[]{ Colour.Red, Colour.Orange,
                                     Colour.Yellow, Colour.Green,
                                     Colour.Cyan, Colour.Blue,
                                     Colour.Violet };
    }

    IEnumerator HandleCountdownTextRoutine(float duration)
    {
        countdownText.color = new Color(countdownText.color.r, countdownText.color.g, countdownText.color.b, 1f);
        while (duration > 0f)
        {
            countdownText.text = "Game resumes in: " + Mathf.CeilToInt(duration);
            duration -= Time.unscaledDeltaTime;
            yield return null;
        }
        countdownText.color = new Color(countdownText.color.r, countdownText.color.g, countdownText.color.b, 0f);
    }

    IEnumerator LaunchCountdownBeforeUnpauseRoutine()
    {
        if (uiManager.ballsAreSpawning)
        {
            pauseScreen.SetActive(false);
            StopCoroutine("HandleCountdownTextRoutine");
            yield return StartCoroutine("HandleCountdownTextRoutine", 3f);
            Debug.Log("Resuming the game");
            gameIsPaused = false;
            Time.timeScale = timeScaleBeforeThePause;
        }
        else
        {
            pauseScreen.SetActive(false);
            gameIsPaused = false;
        }
    }
}