using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMgr : MonoBehaviour {

    public void LoadScene(string sceneName){
		SceneManager.LoadScene(sceneName);
	}
}
