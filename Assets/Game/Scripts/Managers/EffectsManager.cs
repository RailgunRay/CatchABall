using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour {

    public float ToxinEffectLength, TimeDelayLength, ChaosLegth;
    public List<Effect> effects;
    private GameManager gameManager;
    Coroutine turnOffSlowmo; 

	void Start () {
        gameManager = FindObjectOfType<GameManager>();
        effects = new List<Effect>();
	}
	
	
	void Update () {
        if (!gameManager.timeDelayIsActive && Time.timeScale < .998f && !gameManager.gameIsPaused)
        {
            //Debug.Log(Time.timeScale);
            Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, .05f);
            Time.fixedDeltaTime = Time.timeScale * .02f;
        }

        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].effectName == EffectName.Shield)
            {
                if (gameManager.shieldIsActive == false) effects.Remove(effects[i]);
                continue;
            }
            if (effects[i].length <= 0)
            {
                TurnOffEffect(effects[i]);
                effects.Remove(effects[i]);
            }
            else
            {
                effects[i].length -= Time.unscaledDeltaTime;
            }
        }
	}

    public void AddEffect(Effect effect)
    {
        for (int i = 0; i < effects.Count; i++)
        {
            if (effect.effectName == effects[i].effectName)
            {
                effects[i].ResetEffectTiming();
                return;
            }
        }

        effects.Add(effect);
    }

    public void TurnOffEffect(Effect effect)
    {
        switch (effect.effectName)
        {
            case EffectName.Toxin:
                var buckets = gameManager.FindBuckets();
                for (int i = 0; i < buckets.Length; i++)
                {
                    buckets[i].transform.localScale = Vector3.one;
                }
                gameManager.toxinIsActive = false;
                break;
            case EffectName.Chaos:
                gameManager.chaosIsActive = false;
                break;
            case EffectName.TimeDelay:
                gameManager.timeDelayIsActive = false;
                break;
            case EffectName.Shield:
                gameManager.shieldIsActive = false;
                break;
        }
    }
}
