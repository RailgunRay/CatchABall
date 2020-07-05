using UnityEngine;
using System;

public class Effect : MonoBehaviour {

    private EffectsManager effectsManager;
    private GameManager gameManager;
    private float _length; // 0 = infinity
    public float length;
    public EffectName effectName;
    public EffectType effectType;

	void Start () {
        effectsManager = FindObjectOfType<EffectsManager>();
        gameManager = FindObjectOfType<GameManager>();
        AssignAnEffect();
        AssignEffectTime();
        ResetEffectTiming();
        gameManager.ActivateEffect(this);
	}

    public void ResetEffectTiming()
    {
        length = _length;
    }

    void AssignEffectTime()
    {
        if (effectName == EffectName.Shield)
        {
            return;
        }
        else
        {
            _length = 12f;
        }
    }

    void AssignAnEffect() {
        if (effectType == EffectType.Positive)
        {
            if (UnityEngine.Random.Range(1, 11) <= 4)
            {
                effectName = EffectName.Shield;
            }
            else
            {
                effectName = EffectName.TimeDelay;
            }
        }
        else
        {
            if (UnityEngine.Random.Range(1, 3) == 1)
            {
                effectName = EffectName.Toxin;
            }
            else
            {
                effectName = EffectName.Chaos;
            }
        }
    }
}

public enum EffectType
{
    Negative = 0,
    Positive = 1,
    NoEffect = 2
}

public enum EffectName {
    Toxin,
    Chaos,
    TimeDelay,
    Shield,
    Empty
}
