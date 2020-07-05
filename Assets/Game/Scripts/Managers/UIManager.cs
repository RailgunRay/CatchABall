using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{

    public float delayBeforeTheGame = 4f;
    public GameObject loseScreen;
    public bool ballsAreSpawning;
    public Sprite[] effectSprites;
    public GameObject[] effectsDisplays;

    private SpawnManager spawnManager;
    private GameManager gameManager;
    private EffectsManager effectsManager;

    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI goldenBallsCollectedText;


    void Start()
    {
        GoldenBall.onGoldenBallCollect += ChangeGoldenBallsCollectedText;
        gameManager = FindObjectOfType<GameManager>();
        effectsManager = FindObjectOfType<EffectsManager>();
        ReadGameData();
        goldenBallsCollectedText.text = "- " + gameManager.goldenBallsCollected;
        spawnManager = FindObjectOfType<SpawnManager>();
        countdownText.text = "Game Starts In: 3";
        scoreText.text = "Score: 0";
        FindObjectOfType<GameManager>().OnLoseEvent += OnLose;
    }

    void Update()
    {
        UpdateEffectsDisplays();

        if (ballsAreSpawning)
        {
            scoreText.text = "Score: " + Mathf.CeilToInt(GameManager.score);
            return;
        }
        else if (delayBeforeTheGame <= 0f && !ballsAreSpawning)
        {
            GameManager.score = 0;
            spawnManager.StartSpawns();
            ballsAreSpawning = true;
            var clr = countdownText.color;
            countdownText.color = new Color(clr.r, clr.g, clr.b, 0f);
        }
        else if (delayBeforeTheGame <= 1f)
        {
            countdownText.text = "Let's go!";
            delayBeforeTheGame -= Time.deltaTime;
        }
        else
        {
            countdownText.text = "Game Starts In: " + Mathf.Ceil(delayBeforeTheGame - 1f);
            delayBeforeTheGame -= Time.deltaTime;
        }
    }

    void OnLose()
    {
        loseScreen.SetActive(true);
        finalScoreText.text = "Final Score: " + Mathf.CeilToInt(GameManager.score);
        FindObjectOfType<GameManager>().OnLoseEvent -= OnLose;
    }

    void ReadGameData()
    {
        JsonWrapper wrapper = new JsonWrapper();
        string data = wrapper.ReadData();
        try
        {
            GameData gameData = JsonUtility.FromJson<JsonWrapper>(data).gameData;
            gameManager.goldenBallsCollected = gameData.GoldenBallsCollected;
        }
        catch (System.Exception)
        {
            gameManager.goldenBallsCollected = 0;
        }
    }

    void UpdateEffectsDisplays()
    {
        for (int i = 0; i < effectsManager.effects.Count; i++)
        {
            var ImageChild = effectsDisplays[i].transform.GetChild(0);
            var TextChild = effectsDisplays[i].transform.GetChild(1);

            if (effectsManager.effects.Count > 0)
            {
                var effectName = effectsManager.effects[i].effectName;
                string timeRemains = effectsManager.effects[i].effectName == EffectName.Shield ?
                    string.Empty : Mathf.CeilToInt(effectsManager.effects[i].length).ToString();


                ImageChild.GetComponent<Image>().sprite = effectSprites[(int)effectName];
                ImageChild.GetComponent<Image>().color = Color.white;
                TextChild.GetComponent<TextMeshProUGUI>().text = timeRemains;
            }

            
        }

        for (int i = 0; i < effectsDisplays.Length; i++)
        {
            if (i > effectsManager.effects.Count - 1)
            {
                var ImageChild = effectsDisplays[i].transform.GetChild(0);
                var TextChild = effectsDisplays[i].transform.GetChild(1);

                ImageChild.GetComponent<Image>().sprite = null;
                ImageChild.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                TextChild.GetComponent<TextMeshProUGUI>().text = string.Empty;
            }
        }
    }

    void ChangeGoldenBallsCollectedText()
    {
        goldenBallsCollectedText.text = "- " + gameManager.goldenBallsCollected;
    }

}
