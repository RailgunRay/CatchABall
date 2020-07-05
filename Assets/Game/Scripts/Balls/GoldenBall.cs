using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class GoldenBall : MonoBehaviour {

	public static event System.Action onGoldenBallCollect;
	public static int spawnChance = 12;
	public Vector2 speedMinMax;
    public GameObject deathEffect;

    private Transform bottomBorder;
	private CircleCollider2D myCollider;
	private Rigidbody2D myRigidbody;
	private float speed;


	void Start () {
        bottomBorder = GameObject.Find("BottomBorder").transform;
		myCollider = GetComponent<CircleCollider2D>();
		myRigidbody = GetComponent<Rigidbody2D>();
		speed = Random.Range(speedMinMax.x, speedMinMax.y);
		myRigidbody.AddForce(transform.up * -1 * speed * myRigidbody.mass, ForceMode2D.Impulse);
	}

    private void Update()
    {
        if (transform.position.y < bottomBorder.position.y)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Bucket" && other.GetType() == typeof(EdgeCollider2D)) {
			if (onGoldenBallCollect != null) {
				onGoldenBallCollect();
			}

            Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
		}
	}
}
