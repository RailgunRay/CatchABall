using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class BallOLD : MonoBehaviour
{

    GameManager gameManager;
    public static int collectBallReward = 5;
    private Transform bucketHighestPoint;
    private Transform bottomBorder;
    public Colour color;
    public GameObject mainBallSprite;
    public GameObject effectObject;
    public GameObject onDestroyEffect;
    //r -> g -> b -> y (colors)

    public Sprite[] ballSprites;
    public Sprite[] NegativePositiveEffectSprites;
    public Material[] trailMaterials;
    public Material[] particleMaterials;
    public EffectType myEffectType = EffectType.NoEffect;

    public float rotationSpeed = 5f;
    private Rigidbody2D myRigidbody;
    private CircleCollider2D myCollider;
    private TrailRenderer trail;
    private bool isActive = true;
    private bool madeLeap = false;
    [SerializeField] private Vector2 shootingStrenghtMinMax;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        trail = GetComponent<TrailRenderer>();
        onDestroyEffect.GetComponent<ParticleSystemRenderer>().material = particleMaterials[(int)color];
        myRigidbody = GetComponent<Rigidbody2D>();
        Debug.Log(myRigidbody.velocity.magnitude);
        myCollider = GetComponent<CircleCollider2D>();
        bottomBorder = GameObject.Find("BottomBorder").transform;
        bucketHighestPoint = GameObject.Find("BucketHighestPoint").transform;
        ChooseEffectSprite();
        ChooseBallSpriteByColor();
        trail.material = trailMaterials[(int)color];
        var forceStrength = Random.Range(shootingStrenghtMinMax.x, shootingStrenghtMinMax.y);
        Debug.Log("Force strength: " + forceStrength);
        myRigidbody.AddForce(transform.up * -1 * forceStrength
            * myRigidbody.mass, ForceMode2D.Impulse);
        Debug.Log(myRigidbody.velocity.magnitude);
        //myRigidbody.velocity = transform.up * -1 *
        //                       Random.Range(shootingStrenghtMinMax.x, shootingStrenghtMinMax.y);

        Physics2D.queriesStartInColliders = false;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Velocity vector: " + myRigidbody.velocity + " Magnitude: " + myRigidbody.velocity.magnitude);
        }

        if (gameManager.chaosIsActive && transform.position.y <= bucketHighestPoint.position.y && !madeLeap)
        {
            MakeChaoticLeap();
        }

        Debug.DrawLine(transform.position, (Vector2)transform.position + myRigidbody.velocity, Color.cyan);
    }

    void FixedUpdate()
    {
        if (isActive)
        {
            if (transform.position.y < bottomBorder.position.y)
            {
                if (myEffectType != EffectType.NoEffect)
                {
                    HideTheBall();
                    return;
                }

                else if (gameManager.shieldIsActive)
                {
                    gameManager.shieldIsActive = false;
                }
                else
                {
                    GameOver();
                }
                HideTheBall();
            }
        }
    }
    
    void GameOver()
    {
        gameManager.gameOver = true;
        HideTheBall();
    }

    public void HideTheBall()
    {
        isActive = false;
        mainBallSprite.GetComponent<SpriteRenderer>().enabled = false;
        effectObject.GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        myCollider.enabled = false;
        GameObject deathEffect = Instantiate(onDestroyEffect, transform.position, Quaternion.identity);
        Destroy(deathEffect, 3f);
        Destroy(gameObject, 22);
    }

    void MakeChaoticLeap()
    {
        madeLeap = true;
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.name == "Bucket")
        {
            Debug.Log("Relative velocity: " + other.relativeVelocity + " magnitude: " + other.relativeVelocity.magnitude);
        }

        if (other.collider.name == "BottomBorder" && isActive)
        {
            if (myEffectType != EffectType.NoEffect)
            {
                HideTheBall();
                return;
            }

            else if (gameManager.shieldIsActive)
            {
                gameManager.shieldIsActive = false;
            }
            else
            {
                GameOver();
            }
            HideTheBall();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bucket" && other.GetType() == typeof(EdgeCollider2D))
        {
            Bucket bucket = other.transform.GetComponent<Bucket>();

            if (bucket.color != this.color)
            {
                if (gameManager.shieldIsActive)
                {
                    gameManager.shieldIsActive = false;
                }
                else
                {
                    gameManager.gameOver = true;
                }
                HideTheBall();
            }
            else
            {
                gameManager.IncreaseScoreOnBallColect();
                bucket.audioSource.Play();
                if (myEffectType != EffectType.NoEffect)
                {
                    var newEffect = effectObject.AddComponent<Effect>();
                    newEffect.effectType = this.myEffectType;
                }
                HideTheBall();
            }
        }
    }

    void ChooseEffectSprite()
    {
        if (myEffectType == EffectType.NoEffect) return;
        effectObject.GetComponent<SpriteRenderer>().sprite = NegativePositiveEffectSprites[(int)myEffectType];
    }

    void ChooseBallSpriteByColor()
    {
        switch (color)
        {
            case Colour.Red:
                mainBallSprite.GetComponent<SpriteRenderer>().sprite = ballSprites[(int)Colour.Red];
                break;
            case Colour.Green:
                mainBallSprite.GetComponent<SpriteRenderer>().sprite = ballSprites[(int)Colour.Green];
                break;
            case Colour.Blue:
                mainBallSprite.GetComponent<SpriteRenderer>().sprite = ballSprites[(int)Colour.Blue];
                break;
            case Colour.Yellow:
                mainBallSprite.GetComponent<SpriteRenderer>().sprite = ballSprites[(int)Colour.Yellow];
                break;
        }
    }
}
