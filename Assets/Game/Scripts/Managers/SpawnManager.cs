using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Range(1, 100)]
    public int effectSpawnChance = 15;

    

    public Vector2 delayBeforeBallShootMinMax;
    private GameObject[] _spawners;
    private Coroutine SpawnBallsRoutine;

    GameManager gameManager;
    GraphicsManager graphicsManager;

    Sprite[] effectsSprites;

    public GameObject notificationPrefab;
    public GameObject ballPrefab;
    public GameObject goldenBallPrefab;
    public Vector2 ballLocalScaleMinMax;
    public Vector2 timeBetweenSpawnsMinMax;
    public SpriteAndColor[] selectedStyleCollection;


    private void Awake()
    {
        graphicsManager = FindObjectOfType<GraphicsManager>();
        effectsSprites = graphicsManager.currentlySelectedUserPrefsCollection.effectSprites;
        //notifications = graphicsManager.currentlySelectedUserPrefsCollection.spawnNotifications;
        selectedStyleCollection = graphicsManager.currentlySelectedUserPrefsCollection.styleSprites;
    }

    void Start()
    {
        
        gameManager = FindObjectOfType<GameManager>();
        _spawners = GameObject.FindGameObjectsWithTag("Spawner");
    }

    void Update()
    {
        if (gameManager.gameOver)
        {
            if (SpawnBallsRoutine != null)
            {
                StopCoroutine(SpawnBallsRoutine);
            }
        }
    }

    public void StartSpawns()
    {
        SpawnBallsRoutine = StartCoroutine(SpawnHighlightAndBallRoutine());
    }

    void SpawnHighlight(Transform randomSpawner, Colour newRandomColor)
    {
        GameObject newNotification = Instantiate(notificationPrefab,
                                                  randomSpawner.GetChild(0).transform.position,
                                                  randomSpawner.rotation);

        newNotification.GetComponent<SpriteRenderer>().sprite = GetNotificationSpriteByColor(newRandomColor);
        Destroy(newNotification, CalculateHighlightLength());
    }

    Sprite GetNotificationSpriteByColor(Colour color)
    {
        foreach (var clr in selectedStyleCollection)
        {
            if (clr.color == color)
            {
                return clr.notificationSprite;
            }
        }

        return selectedStyleCollection[0].notificationSprite;
    }

    float CalculateHighlightLength()
    {
        return Mathf.Lerp(delayBeforeBallShootMinMax.y, delayBeforeBallShootMinMax.x,
                          Difficulty.GetDifficultyPercent());
    }

    void SpawnNewBall(Transform randomSpawner, Colour newRandomColor)
    {
        TryToSpawnGoldenBall();
        float spawnSize = Random.Range(ballLocalScaleMinMax.x, ballLocalScaleMinMax.y);
        GameObject newBall = Instantiate(ballPrefab, randomSpawner.position,
                                        randomSpawner.rotation);

        newBall.GetComponent<BallGame>().myColor = newRandomColor;
        newBall.transform.localScale = Vector2.one * spawnSize;

        if (Random.Range(1, 101) <= effectSpawnChance)
        {
            newBall.GetComponent<BallGame>().effectType = SpawnEffect();
        }
        else
        {
            newBall.GetComponent<BallGame>().effectType = EffectType.NoEffect;
        }
    }

    EffectType SpawnEffect()
    {
        return Random.Range(1, 3) == 1 ? EffectType.Negative : EffectType.Positive;
    }

    Transform GetRandomSpawner()
    {
        return _spawners[Random.Range(0, _spawners.Length)].transform;
    }

    IEnumerator SpawnHighlightAndBallRoutine()
    {
        while (true)
        {
            Transform randomSpawner = GetRandomSpawner();
            Colour newRandomColor = GetRandomColor();
            SpawnHighlight(randomSpawner, newRandomColor);
            yield return new WaitForSeconds(CalculateHighlightLength());
            SpawnNewBall(randomSpawner, newRandomColor);
            float nextSpawnTime = Mathf.Lerp(timeBetweenSpawnsMinMax.y,
                                            timeBetweenSpawnsMinMax.x,
                                            Difficulty.GetDifficultyPercent());
            yield return new WaitForSeconds(nextSpawnTime);
        }
    }

    Colour GetRandomColor()
    {
        var colors = GameManager.sessionColors;
        return (Colour)colors.GetValue(Random.Range(0, colors.Length));
    }

    void TryToSpawnGoldenBall()
    {
        int chance = Random.Range(1, 101);
        if (chance <= GoldenBall.spawnChance)
        {
            Transform spawner = GetRandomSpawner();
            Instantiate(goldenBallPrefab, spawner.position, spawner.rotation);
        }
    }
}