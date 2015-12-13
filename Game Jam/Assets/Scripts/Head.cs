using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Head : MonoBehaviour
{
    protected const float NO_TURN = 0;
    protected const float QUARTER_TURN = 90;
    protected const float HALF_TURN = 180;
    protected const float FULL_TURN = 360;
    private const float BASE_MOVEMENT_SPEED = 2.0f;
    private const float TURN_SPEED = 2.5f;
    private const float OUT_OF_BOUNDS_FORCE = 0.05f;
    private const float LEVEL_X_BORDER = 7f;
    private const float LEVEL_Y_BORDER = 3.75f;
    private const float INVINCIBLE_DURATION = 2.0f;
    private const float BODY_SPEED_LOSS_MULTIPLIER = 0.05f;

    protected bool ControlledMovementAllowed;
    protected float TravelAngle;
    protected bool RestrictedByBorders;
    protected bool HasFireball = true;

    public bool shouldMove = false;

    public ParticleSystem orbExplosion;

    private float _movementSpeed;

    public List<int> tail = new List<int>(); // List of tail segments as colours
    public List<Tail> tailObjects = new List<Tail>(); // List of tail segments as game objects

    public int score;

    public bool isInvincible; // I AM INVEENCEEBLE

    public virtual void Update()
    {
        if(shouldMove)
        {
            AngleCorrection();

            if (RestrictedByBorders)
            {
                ControlledMovementAllowed = !OutOfBoundsCheck();
            }

            MoveSnake();

            //Update children
            for (int i = 0; i < tailObjects.Count; i++)
            {
                tailObjects[i].FollowParent(i == 0 ? gameObject : tailObjects[i - 1].gameObject);
            }

            CheckMatches();
        }
    }

    private void CheckMatches()
    {
        for (int i = 0; i < tailObjects.Count; i++)
        {
            int successive = 1; // Number of successive segments of the same colour

            for (int j = i + 1; j < tail.Count; j++)
            {
                if (tail[j] == tail[i])
                    successive++;

                else
                    break;
            }

            if (successive >= 3)
            {
                for (int k = i; k < i + successive; k++)
                {
                    RemoveFromTailAt(i, true);
                }

                AddToScore(((successive - 1) * successive) / 2);

                SoundManager.PlaySound("SnakeCombo");
            }
        }
    }

    protected void UseFireball()
    {
        HasFireball = false;

        var fireball = Instantiate(Resources.Load("Prefabs/Fireball"), Vector2.zero, Quaternion.identity) as GameObject;
        fireball.GetComponent<Fireball>().Setup(transform.position, TravelAngle, gameObject);
    }

    private void AngleCorrection()
    {
        if (TravelAngle * Mathf.Rad2Deg > HALF_TURN)
            TravelAngle = -HALF_TURN * Mathf.Deg2Rad;
        else if (TravelAngle * Mathf.Rad2Deg < -HALF_TURN)
            TravelAngle = HALF_TURN * Mathf.Deg2Rad;
    }

    private bool OutOfBoundsCheck()
    {
        float angleDegrees = Mathf.Rad2Deg * TravelAngle;

        if (transform.position.x > LEVEL_X_BORDER)
        {
            if (angleDegrees <= NO_TURN)
                TravelAngle -= OUT_OF_BOUNDS_FORCE;
            else
                TravelAngle += OUT_OF_BOUNDS_FORCE;

            return true;
        }

        if (transform.position.x < -LEVEL_X_BORDER)
        {
            if (angleDegrees >= NO_TURN)
                TravelAngle -= OUT_OF_BOUNDS_FORCE;
            else
                TravelAngle += OUT_OF_BOUNDS_FORCE;

            return true;
        }

        angleDegrees = Mathf.Abs(Mathf.Rad2Deg * TravelAngle);

        if (transform.position.y > LEVEL_Y_BORDER)
        {
            if (angleDegrees <= QUARTER_TURN)
                TravelAngle -= OUT_OF_BOUNDS_FORCE;
            else
                TravelAngle += OUT_OF_BOUNDS_FORCE;

            return true;
        }
        if (transform.position.y < -LEVEL_Y_BORDER)
        {
            if (angleDegrees >= QUARTER_TURN)
                TravelAngle -= OUT_OF_BOUNDS_FORCE;
            else 
                TravelAngle += OUT_OF_BOUNDS_FORCE;

            return true;
        }

        return false;
    }

    private void MoveSnake()
    {
        _movementSpeed = Mathf.Max(BASE_MOVEMENT_SPEED, _movementSpeed) - (BODY_SPEED_LOSS_MULTIPLIER * tailObjects.Count);
        transform.position += new Vector3(Mathf.Cos(TravelAngle)*_movementSpeed*Time.deltaTime,
            Mathf.Sin(TravelAngle)*_movementSpeed*Time.deltaTime,
            0);

        transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg*TravelAngle - 90);
    }

    protected void Turn(TurnDirection turnDirection)
    {
        int directionBit = turnDirection == TurnDirection.LEFT ? 1 : -1;
        TravelAngle += TURN_SPEED * Time.deltaTime * directionBit;
    }

    public void AddToScore(int x)
    {
        score += x;
    }

    public void AddToTail(int x)
    {
        tail.Add(x);
    }

    public void AddToTailObjects(GameObject x)
    {
        if (tailObjects.Count != 0)
        {
            tailObjects[tailObjects.Count - 1].GetComponent<CircleCollider2D>().isTrigger = false;
            tailObjects[tailObjects.Count - 1].GetComponent<CircleCollider2D>().enabled = false;
        }

        tailObjects.Add(x.GetComponent<Tail>());
        x.GetComponent<Tail>().SetParent(gameObject);
        tailObjects[tailObjects.Count - 1].GetComponent<CircleCollider2D>().isTrigger = true;
        x.transform.parent = transform.parent;
    }

    public void RemoveFromTailAt(int x, bool destroy = false)
    {
        if(tail.Count <= x)
            throw new NotSupportedException("Cannot remove a tail segment that does not exist");

        if (x == tail.Count)
            PopBackOfTail(destroy);

        else
        {
            tail.RemoveAt(x);

            if (destroy)
                Destroy(tailObjects[x].gameObject);

            tailObjects.RemoveAt(x);
        }
        
    }

    public void PopBackOfTail(bool destroy = false)
    {
        tail.RemoveAt(tail.Count - 1);

        if (destroy)
            Destroy(tailObjects[tailObjects.Count - 1].gameObject);

        tailObjects.RemoveAt(tailObjects.Count - 1);

        if (tailObjects.Count != 0)
        {
            tailObjects[tailObjects.Count - 1].GetComponent<CircleCollider2D>().enabled = true;
            tailObjects[tailObjects.Count - 1].GetComponent<CircleCollider2D>().isTrigger = true;
        }
    }

    public void StartInvincibleTimer()
    {
        StartCoroutine(InvincibleTimer(INVINCIBLE_DURATION));
    }

    private System.Collections.IEnumerator InvincibleTimer(float time)
    {
        StartCoroutine(InvincibleFlash(INVINCIBLE_DURATION));

        isInvincible = true;

        while(time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        isInvincible = false;
    }

    private System.Collections.IEnumerator InvincibleFlash(float time)
    {
        float foo = time;

        // Flash three times

        for(int i = 1; i <= 3; i++)
        {
            float x = foo - ((time * (i - 1)) / 3);
            float y = foo - ((time * (i - 1)) / 3) - (time / 6);
            float z = foo - ((time * i) / 3);

            while(time > y)
            {
                this.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, (y - time) / (y - x), 1f);
                time -= Time.deltaTime;
                yield return null;
            }

            while(time <= y && time > z)
            {
                this.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, (time - y) / (z - y), 1f);
                time -= Time.deltaTime;
                yield return null;
            }
        }
    }

    public System.Collections.IEnumerator OrbExplosion(Vector3 pos, int c)
    {
        ParticleSystem ps = Instantiate(orbExplosion, pos, Quaternion.identity) as ParticleSystem;

        if (c == 0)
            ps.startColor = new Color(1f, 0f, 0f, 1f);

        else if (c == 1)
            ps.startColor = new Color(0f, 1f, 0f, 1f);

        else if (c == 2)
            ps.startColor = new Color(0f, 0f, 1f, 1f);

        yield return new WaitForSeconds(1);

        Destroy(ps.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject _segmentHead = null;

        if (collider.gameObject.tag == "TailSegment")
            _segmentHead = collider.GetComponent<Tail>().Head;

        if (collider.gameObject.tag == "TailSegment" && 
            _segmentHead != gameObject &&
            !_segmentHead.GetComponent<Head>().isInvincible)
        {
            // Move eaten segment to current tail

            AddToTail(_segmentHead.GetComponent<Head>().tail[_segmentHead.GetComponent<Head>().tail.Count - 1]);

            AddToTailObjects(collider.gameObject);
            _segmentHead.GetComponent<Head>().PopBackOfTail();

            collider.transform.position = new Vector3(tailObjects[tailObjects.Count - 1].transform.position.x,
                tailObjects[tailObjects.Count - 1].transform.position.y,
                0f);

            // Make segment head invincible

            _segmentHead.GetComponent<Head>().StartInvincibleTimer();

            // Play sound
            SoundManager.PlaySound("SnakeEat");
        }

        else if(collider.gameObject.tag == "Orb")
        {
            collider.gameObject.tag = "TailSegment";

            int c = 0;

            if (collider.GetComponent<SpriteRenderer>().color.r == 1)
                c = 0;

            else if (collider.GetComponent<SpriteRenderer>().color.g == 1)
                c = 1;

            else if (collider.GetComponent<SpriteRenderer>().color.b == 1)
                c = 2;

            AddToTail(c);
            AddToTailObjects(collider.gameObject);

            StartCoroutine(OrbExplosion(collider.transform.position, c));

            // Play sound
            SoundManager.PlaySound("SnakeEat");
        }

        else if(collider.gameObject.tag == "Virus")
        {            
            if(collider.GetComponent<SpriteRenderer>().color.r == 1)
            {
                for(int i = tail.Count - 1; i >= 0; i--)
                {
                    if (tail[i] == 0)
                        RemoveFromTailAt(i, true);
                }
            }

            else if(collider.GetComponent<SpriteRenderer>().color.g == 1)
            {
                for(int i = tail.Count - 1; i >= 0; i--)
                {
                    if (tail[i] == 1)
                        RemoveFromTailAt(i, true);
                }
            }

            else if(collider.GetComponent<SpriteRenderer>().color.b == 1)
            {
                for (int i = tail.Count - 1; i >= 0; i--)
                {
                    if (tail[i] == 2)
                        RemoveFromTailAt(i, true);
                }
            }

            Destroy(collider.gameObject);

            // Play sound
            SoundManager.PlaySound("SnakeEatVirus");
        }
        else if (collider.gameObject.tag == "FireballPickup")
        {
            HasFireball = true;
            Destroy(collider.gameObject);
        }
    }
}