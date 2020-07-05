using UnityEngine;

public class MusicManager : MonoBehaviour {

	public static MusicManager musicManagerInstance;

	void Awake(){
		if(musicManagerInstance){
			Destroy(gameObject);
		}
		else{
			musicManagerInstance = this;
		}
	}
}
