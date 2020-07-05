using System.Collections;
using UnityEngine;

public class MainMenuSpawner : MonoBehaviour
{

    public GameObject ballDummyPrefab;
    public Vector2 delayBetweenSpawnsMinMax;
    Camera viewCamera;

    void Awake()
    {
        viewCamera = Camera.main;
        
    }

    private void Start()
    {
        StartCoroutine(SpawnBallsRoutine());
        Time.fixedDeltaTime = 1 / 50f;
        Time.timeScale = 1f;
    }

    IEnumerator SpawnBallsRoutine()
    {
        while (true)
        {
            Vector2 randomPos = GetRandomSpot();
            Vector2 dirVector = (randomPos - Random.insideUnitCircle).normalized;
            float rotZ = Mathf.Atan2(dirVector.y, dirVector.x);
            //Debug.Log(dirVector + " " + rotZ);
            GameObject newBall = Instantiate(ballDummyPrefab, randomPos, Quaternion.Euler(0, 0, rotZ * Mathf.Rad2Deg));
            newBall.transform.SetParent(transform);
            float waitTime = Random.Range(delayBetweenSpawnsMinMax.x, delayBetweenSpawnsMinMax.y);
            yield return new WaitForSeconds(waitTime);
        }
    }

    Vector2 GetRandomSpot()
    {
        int randomSide = Random.Range(0, 3);
        float randomX, randomY;

        switch (randomSide)
        {
            case 0:
                randomY = Random.Range(0f, viewCamera.pixelHeight);
                var randomSpot = viewCamera.ScreenToWorldPoint(new Vector2(0f, randomY));
                return new Vector2(randomSpot.x - 1.5f, randomSpot.y);
            case 1:
                randomX = Random.Range(0f, viewCamera.pixelWidth);
                randomSpot = viewCamera.ScreenToWorldPoint(new Vector2(randomX, viewCamera.pixelHeight));
                return new Vector2(randomSpot.x, randomSpot.y + 1.5f);
            case 2:
                randomY = Random.Range(0f, viewCamera.pixelHeight);
                return viewCamera.ScreenToWorldPoint(new Vector2(viewCamera.pixelWidth + 1.5f, randomY));
            //case 3:
            //    randomX = Random.Range(0f, viewCamera.pixelWidth);
            //    return viewCamera.ScreenToWorldPoint(new Vector2(randomX, -1f));
        }

        return Vector2.zero;
    }
}
