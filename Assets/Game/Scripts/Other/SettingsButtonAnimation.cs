using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButtonAnimation : MonoBehaviour {

    Animator anim;
    bool settingsAreOpened;

	void Start () {
        anim = GetComponent<Animator>();
        //settingsAreOpened = anim.GetBool("SettingsOpened");
	}
    

    public void ManageSettingsClick()
    {
        //if (anim.IsInTransition(0)) { }
        anim.SetBool("Opened", anim.GetBool("Opened") ? false : true);
    }
}
