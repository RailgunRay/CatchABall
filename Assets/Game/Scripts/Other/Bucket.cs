using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Bucket : MonoBehaviour
{

    public AudioSource audioSource;
    private SpriteRenderer _spriteRenderer;
    private Sprite _mySprite;
    public Colour color;
    public Transform particlePoint;
    private Transform bottomBorder;
    private Collider2D dragCollider;

    [SerializeField] private float _returnSpeed = .1f;
    private Vector2 _startingPosition;
    private EdgeCollider2D colliderToCatchTheBall;
    private Transform bucketHighestPoint;
    private Rigidbody2D myRigidbody;
    private bool isTarget;

    public void Initialise(Colour instanceColor, Sprite instanceSprite)
    {
        color = instanceColor;
        _mySprite = instanceSprite;
        _spriteRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
        //GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = _mySprite;
    }

    void Start()
    {
        dragCollider = gameObject.GetComponentInParent<BoxCollider2D>();
        bottomBorder = GameObject.Find("BottomBorder").transform;
        bucketHighestPoint = GameObject.Find("BucketHighestPoint").transform;
        audioSource = GetComponent<AudioSource>();
        _startingPosition = transform.position;
        colliderToCatchTheBall = GetComponent<EdgeCollider2D>();
        myRigidbody = GetComponent<Rigidbody2D>();
    }



    void SwitchBodyTypeToDynamic()
    {
        myRigidbody.bodyType = RigidbodyType2D.Dynamic;
        myRigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        myRigidbody.freezeRotation = true;
        myRigidbody.mass = 1f;
        myRigidbody.gravityScale = 0f;
    }

#if UNITY_EDITOR || UNITY_WEBGL

    void OnMouseDown()
    {
        isTarget = true;
    }

    void OnMouseUp()
    {
        isTarget = false;
        myRigidbody.velocity = Vector2.zero;
        myRigidbody.bodyType = RigidbodyType2D.Kinematic;
    }

    void OnMouseDrag()
    {
        DoMove(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

#endif

    void DoMove(Vector2 travelPosition)
    {
        if (Time.timeScale > .1f)
        {
            SwitchBodyTypeToDynamic();
            var newPosition = travelPosition;
            myRigidbody.velocity = Vector2.zero;
            float distance = (newPosition - (Vector2)transform.position).magnitude;
            float velocity = distance / Time.deltaTime;
            myRigidbody.AddForce((newPosition - (Vector2)transform.position).normalized * 
                velocity * myRigidbody.mass, ForceMode2D.Impulse);
        }
    }

    bool isTouched(Vector2 touchPos)
    {
        if (dragCollider == Physics2D.OverlapPoint(touchPos))
        {
            return true;
        }

        return false;
    }

    void FixedUpdate()
    {

#if UNITY_ANDROID

        if (!isTarget)
        {
            myRigidbody.velocity = Vector2.zero;
            myRigidbody.bodyType = RigidbodyType2D.Kinematic;
            myRigidbody.MovePosition(Vector3.Lerp(transform.position, _startingPosition, _returnSpeed * Time.deltaTime));
        }

        if (Input.touchCount != 1)
        {
            isTarget = false;
            return;
        }

        Touch touch = Input.touches[0];
        Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

        if (touch.phase == TouchPhase.Began)
        {
            isTarget = isTouched(touchPos) == true ? true : false;
        }
        else if (isTarget && (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary))
        {
            Debug.Log(myRigidbody.velocity);
            DoMove(touchPos);
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            isTarget = false;
        }

#elif UNITY_WEBGL || UNITY_EDITOR
        if (isTarget)
        {
            //Vector3 newLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            ////transform.position = new Vector3(newLocation.x, Mathf.Clamp(newLocation.y, _startingPosition.y,
            ////    bucketHighestPoint.position.y));
            //myRigidbody.AddForce((new Vector2(newLocation.x, Mathf.Clamp(newLocation.y, _startingPosition.y,
            //bucketHighestPoint.position.y)) - (Vector2)transform.position).normalized * 500f);
        }
        else
        {
            myRigidbody.MovePosition(Vector3.Lerp(transform.position, _startingPosition, _returnSpeed));
        }
#endif
    }
}