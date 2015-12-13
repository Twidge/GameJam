using UnityEngine;

public class Fireball : MonoBehaviour {

    private const float BASE_MOVEMENT_SPEED = 5.0f;
    private const float LEVEL_X_BORDER = 8.75f;
    private const float LEVEL_Y_BORDER = 4.75f;

    private float lifeTime = 8f;

    private float _xSpeed;
    private float _ySpeed;
    protected GameObject ParentSnake;
	
	// Update is called once per frame
	void Update () {


        transform.position += new Vector3(_xSpeed, _ySpeed, 0);

	    WallCheck();

	    lifeTime -= Time.deltaTime;

        if(lifeTime <= 0)
            Destroy(gameObject);
	}

    public void Setup(Vector3 position, float travelAngle, GameObject parentSnake)
    {
        transform.position = position;

        _xSpeed = Mathf.Cos(travelAngle) * BASE_MOVEMENT_SPEED * Time.deltaTime;
        _ySpeed = Mathf.Sin(travelAngle) * BASE_MOVEMENT_SPEED * Time.deltaTime;

        ParentSnake = parentSnake;
    }

    private void WallCheck()
    {

        if (transform.position.x > LEVEL_X_BORDER)
        {
            _xSpeed *= -1;
        } else 

        if (transform.position.x < -LEVEL_X_BORDER)
        {
            _xSpeed *= -1;
        }

        if (transform.position.y > LEVEL_Y_BORDER)
        {
            _ySpeed *= -1;
        }else 
        if (transform.position.y < -LEVEL_Y_BORDER)
        {
            _ySpeed *= -1;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        var parentSnakeHead = ParentSnake.GetComponent<Head>();

        GameObject _segmentHead = null;

        if (collider.gameObject.tag == "TailSegment")
            _segmentHead = collider.GetComponent<Tail>().Head;

        if (collider.gameObject.tag == "TailSegment" &&
            _segmentHead != gameObject &&
            !_segmentHead.GetComponent<Head>().isInvincible)
        {
            // Move eaten segment to current tail

            parentSnakeHead.AddToTail(_segmentHead.GetComponent<Head>().tail[_segmentHead.GetComponent<Head>().tail.Count - 1]);

            parentSnakeHead.AddToTailObjects(collider.gameObject);
            _segmentHead.GetComponent<Head>().PopBackOfTail();

            collider.transform.position = new Vector3(parentSnakeHead.tailObjects[parentSnakeHead.tailObjects.Count - 1].transform.position.x,
                parentSnakeHead.tailObjects[parentSnakeHead.tailObjects.Count - 1].transform.position.y,
                0f);

            // Make segment head invincible

            _segmentHead.GetComponent<Head>().StartInvincibleTimer();

            // Play sound
            SoundManager.PlaySound("SnakeEat");
        }

        else if (collider.gameObject.tag == "Orb")
        {
            collider.gameObject.tag = "TailSegment";

            int c = 0;

            if (collider.GetComponent<SpriteRenderer>().color.r == 1)
                c = 0;

            else if (collider.GetComponent<SpriteRenderer>().color.g == 1)
                c = 1;

            else if (collider.GetComponent<SpriteRenderer>().color.b == 1)
                c = 2;

            parentSnakeHead.AddToTail(c);
            parentSnakeHead.AddToTailObjects(collider.gameObject);

            StartCoroutine(parentSnakeHead.OrbExplosion(collider.transform.position, c));

            // Play sound
            SoundManager.PlaySound("SnakeEat");
        }

        else if (collider.gameObject.tag == "Virus")
        {
            if (collider.GetComponent<SpriteRenderer>().color.r == 1)
            {
                for (int i = parentSnakeHead.tail.Count - 1; i >= 0; i--)
                {
                    if (parentSnakeHead.tail[i] == 0)
                        parentSnakeHead.RemoveFromTailAt(i, true);
                }
            }

            else if (collider.GetComponent<SpriteRenderer>().color.g == 1)
            {
                for (int i = parentSnakeHead.tail.Count - 1; i >= 0; i--)
                {
                    if (parentSnakeHead.tail[i] == 1)
                        parentSnakeHead.RemoveFromTailAt(i, true);
                }
            }

            else if (collider.GetComponent<SpriteRenderer>().color.b == 1)
            {
                for (int i = parentSnakeHead.tail.Count - 1; i >= 0; i--)
                {
                    if (parentSnakeHead.tail[i] == 2)
                        parentSnakeHead.RemoveFromTailAt(i, true);
                }
            }

            Destroy(collider.gameObject);

            // Play sound
            SoundManager.PlaySound("SnakeEatVirus");
        }
    }
}
