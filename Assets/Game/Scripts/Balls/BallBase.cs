using UnityEngine;


public class BallBase : MonoBehaviour {

    protected GraphicsManager graphicsManager;
    const float widthMultiplier = .38f;

    protected Rigidbody2D myRigidbody;
    protected SpriteRenderer spriteRenderer;
    protected CircleCollider2D myCollider;

    public Colour myColor;
    SpriteAndColor[] selectedStyleSprites;
    public Vector2 startingForceMinMax;
    public Vector2 spawnScaleMinMax;
    public GameObject deathEffect;
    TrailRenderer trail;


    protected virtual void Start()
    {
        InitialiseComponents();
        graphicsManager = FindObjectOfType<GraphicsManager>();
        selectedStyleSprites = graphicsManager.currentlySelectedUserPrefsCollection.styleSprites;
        AssignDeathEffectMaterial();
        RandomiseColor();
        ConfigureTrail();
        AddForceOnSpawn();
    }

    void AssignDeathEffectMaterial()
    {
        var renderer = deathEffect.GetComponent<ParticleSystemRenderer>();
        Material mat = null;

        foreach (var item in selectedStyleSprites)
        {
            if (item.color == myColor)
            {
                mat = item.deathParticleMaterial;
            }
        }
        renderer.material = new Material(mat);
    }

    protected void InitialiseComponents()
    {
        transform.localScale = transform.localScale * Random.Range(spawnScaleMinMax.x, spawnScaleMinMax.y);
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        trail = GetComponent<TrailRenderer>();
    }

    protected virtual void RandomiseColor()
    {
        myColor = (Colour)Random.Range(0, System.Enum.GetNames(typeof(Colour)).Length);
        AssignSprite();
    }

    void ConfigureTrail()
    {
        //trail.startWidth = transform.localScale.x * widthMultiplier;
        //trail.endWidth = transform.localScale.x * widthMultiplier;
        trail.material = new Material(GetTrailMat(myColor));
    }

    Material GetTrailMat(Colour color)
    {
        foreach (var item in selectedStyleSprites)
        {
            if (item.color == color)
            {
                return item.trailMaterial;
            }
        }

        return null;
    }


    public void AssignSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    /// <summary>
    /// Assignes sprite depending on instance color
    /// </summary>
    protected void AssignSprite()
    {
        Sprite mySprite = null;
        foreach (var spr in selectedStyleSprites)
        {
            if (spr.color == myColor)
            {
                mySprite = spr.ballSprite;
            }
        }
        spriteRenderer.sprite = mySprite;
    }

    public virtual void MakeObjectInvisibleAndDestroy()
    {
        myRigidbody.bodyType = RigidbodyType2D.Static;
        myCollider.enabled = false;
        spriteRenderer.enabled = false;
        Destroy(gameObject, 10);
    }

    protected virtual void AddForceOnSpawn()
    {
        float forceStrenght = Random.Range(startingForceMinMax.x, startingForceMinMax.y);
        myRigidbody.AddForce(-transform.up * forceStrenght * myRigidbody.mass, ForceMode2D.Impulse);
    }
}
