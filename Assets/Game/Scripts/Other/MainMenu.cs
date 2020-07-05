using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

    public TMPro.TextMeshProUGUI goldenBallsText;
    public GameObject mainMenuButtons;
    public GameObject settingMenuButtons;
    private bool settingsAreOpened;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            LoadData();
        }
    }

    public void OpenSettings()
    {
        settingsAreOpened = true;
        mainMenuButtons.SetActive(false);
        settingMenuButtons.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsAreOpened = false;
        settingMenuButtons.SetActive(false);
        mainMenuButtons.SetActive(true);
    }

	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && settingsAreOpened)
        {
            CloseSettings();
        }
	}

    void LoadData()
    {
        int gBallsCollected;
        JsonWrapper wrapper = new JsonWrapper();
        string data = wrapper.ReadData();
        try
        {
            GameData gameData = JsonUtility.FromJson<JsonWrapper>(data).gameData;
            gBallsCollected = gameData.GoldenBallsCollected;
        }
        catch (System.Exception)
        {
            gBallsCollected = 0;
        }
        goldenBallsText.text = "- " + gBallsCollected;
    }

    public void TakeScreenshotAndShare()
    {
        StartCoroutine("TakeScreenshotAndShareRoutine");
    }

    IEnumerator TakeScreenshotAndShareRoutine()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filePath = System.IO.Path.Combine(Application.temporaryCachePath, "screenshot.png");
        System.IO.File.WriteAllBytes(filePath, ss.EncodeToPNG());

        Destroy(ss);

        new NativeShare().AddFile(filePath).SetSubject("Subject goes here").SetText("Hey! Just wanna tell you about new cool game by Team Lux - \"Colorfall\"! " +
            "Download it here for Android: 'Android link place' \nfor IOS: 'IOS link place'").Share();
    }
}
