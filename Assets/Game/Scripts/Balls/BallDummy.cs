using UnityEngine;


public class BallDummy : BallBase
{
    protected override void AddForceOnSpawn()
    {
        Vector2 startingPos = transform.position;
        Vector2 dirVector = (Random.insideUnitCircle - startingPos).normalized;
        float forceStrenght = Random.Range(startingForceMinMax.x, startingForceMinMax.y);

        myRigidbody.AddForce(dirVector * forceStrenght * myRigidbody.mass, ForceMode2D.Impulse);
    }

    new void RandomiseColor()
    {
        myColor = (Colour)Random.Range(0, System.Enum.GetNames(typeof(Colour)).Length);
        AssignSprite();
    }

    private void Update()
    {
        if (myRigidbody.position.y < -(Camera.main.orthographicSize + .5f))
        {
            MakeObjectInvisibleAndDestroy();
        }
    }
}
