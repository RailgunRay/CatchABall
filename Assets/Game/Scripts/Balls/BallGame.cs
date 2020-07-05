using UnityEngine;

public class BallGame : BallBase
{
    RipplePostProcessor ripplePostProcessor;
    public System.Action effectAction;
    public EffectType effectType = EffectType.NoEffect;
    GameObject effectSpriteHolder;
    GameManager gameManager;
    EffectsManager effectsManager;
    Sprite[] effectTypeSprites;
    Transform bucketHighestPoint;

    Vector2 myVelocity;
    bool madeChaoticLeap = false;

    public event System.Action onBallCollect;
    public static float collectBallReward = 5f;
    const float deathEffectLength = 3f;

    protected override void Start()
    {
        base.Start();
        AssignSprite();
        effectSpriteHolder = transform.GetChild(0).gameObject;
        ripplePostProcessor = Camera.main.GetComponent<RipplePostProcessor>();
        if (effectType != EffectType.NoEffect) { SpawnEffect(); onBallCollect += effectAction; }
        //ConfigureDeathEffect();

        gameManager = FindObjectOfType<GameManager>();
        effectsManager = FindObjectOfType<EffectsManager>();
        bucketHighestPoint = GameObject.Find("BucketHighestPoint").transform;

        onBallCollect += MakeObjectInvisibleAndDestroy;
        onBallCollect += gameManager.IncreaseScoreOnBallColect;
    }

    private void FixedUpdate()
    {
        if (gameManager.chaosIsActive && transform.position.y <= bucketHighestPoint.position.y && !madeChaoticLeap)
        {
            MakeChaoticLeap();
        }
    }

    void SpawnEffect()
    {
        ApplyEffectSprite();
    }

    void ApplyEffectSprite()
    {
        if (effectType == EffectType.NoEffect) return;
        SpriteRenderer effectSprite = effectSpriteHolder.GetComponent<SpriteRenderer>();

        if (effectType == EffectType.Positive)
        {
            effectSprite.sprite = graphicsManager.currentlySelectedUserPrefsCollection.positiveEffectSprite;
        }
        else
        {
            effectSprite.sprite = graphicsManager.currentlySelectedUserPrefsCollection.negativeEffectHighlight;
        }
    }

    protected override void RandomiseColor() { }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bucket" && other.GetType() == typeof(EdgeCollider2D))
        {
            Bucket bucket = other.gameObject.GetComponent<Bucket>();

            if (myColor != bucket.color)
            {
                if (gameManager.shieldIsActive)
                {
                    gameManager.shieldIsActive = false;
                }
                else
                {
                    gameManager.gameOver = true;
                }

                DestroySelf();
            }
            else
            {
                if (effectType != EffectType.NoEffect)
                {
                    Effect newEffect = effectsManager.gameObject.AddComponent<Effect>();
                    newEffect.effectType = this.effectType;
                }

                if (this.onBallCollect != null)
                {
                    Collect();
                }
            }


        }
        else if (other.gameObject.name == "BottomBorder")
        {
            DestroySelf();

            if (effectType == EffectType.NoEffect)
            {
                if (gameManager.shieldIsActive)
                {
                    gameManager.shieldIsActive = false;
                }
                else
                {
                    gameManager.gameOver = true;
                }
            }
        }
    }

    void Collect()
    {
        SpawnDeathEffect();
        onBallCollect();
        onBallCollect -= this.MakeObjectInvisibleAndDestroy;
    }

    void DestroySelf()
    {
        MakeObjectInvisibleAndDestroy();
        SpawnDeathEffect();
    }

    void SpawnDeathEffect()
    {
        ripplePostProcessor.CreateRippleEffect(this.transform.position);
        GameObject newDeathEffect = Instantiate(this.deathEffect, transform.position, Quaternion.identity);
        Destroy(newDeathEffect, deathEffectLength);
    }

    //void ConfigureDeathEffect()
    //{
    //    deathEffect.GetComponent<ParticleSystemRenderer>().material = new Material(deathEffectMats[(int)myColor]);
    //}

    public override void MakeObjectInvisibleAndDestroy()
    {
        effectSpriteHolder.SetActive(false);
        base.MakeObjectInvisibleAndDestroy();
    }

    void MakeChaoticLeap()
    {
        madeChaoticLeap = true;
        float randomX, randomY;
        // 1 - leap right, 2 - leap left
        if (Random.Range(1, 3) == 1)
        {
            randomX = Random.Range(Mathf.Cos(30 * Mathf.Deg2Rad), Mathf.Cos(60 * Mathf.Deg2Rad));
            randomY = Random.Range(Mathf.Sin(30 * Mathf.Deg2Rad), Mathf.Sin(60 * Mathf.Deg2Rad));
        }
        else
        {
            randomX = Random.Range(Mathf.Cos(120 * Mathf.Deg2Rad), Mathf.Cos(150 * Mathf.Deg2Rad));
            randomY = Random.Range(Mathf.Sin(120 * Mathf.Deg2Rad), Mathf.Sin(150 * Mathf.Deg2Rad));
        }

        float forceStrenght = Random.Range(2f, 6f);

        Debug.Log("Adding Force: " + forceStrenght);

        myRigidbody.AddForce(new Vector2(randomX, randomY) * forceStrenght * myRigidbody.mass, ForceMode2D.Impulse);
    }
}
