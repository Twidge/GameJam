using UnityEngine;
using System.Collections;

public class RandomGenerator : MonoBehaviour
{
    public GameObject tailOrb;
    public GameObject virus;
    public GameObject fireballPickup;

    public bool shouldSpawn = false;

    private bool _nextOrb = true;
    private float ORB_TIMER;

    private bool _nextFireballPickup = true;
    private const int FIREBALL_PICKUP_TIMER = 10;

    private const float X_BOUND = 4f;
    private const float Y_BOUND = 3f;
    private const float MIN_DISTANCE_AWAY = 1f;

    private bool _nextVirus = true;
    private const int VIRUS_TIMER = 8;

	void Start ()
    {
        ORB_TIMER = 4f / SnakeGeneration.NumberOfPlayers;
    }
	
	void Update ()
    {
	    if(_nextOrb && shouldSpawn)
            StartCoroutine(RandomOrbTimer(ORB_TIMER));

        if (_nextVirus && shouldSpawn)
            StartCoroutine(RandomVirusTimer(VIRUS_TIMER));

        if (_nextFireballPickup)
            StartCoroutine(RandomFireballTimer(FIREBALL_PICKUP_TIMER));
	}

    IEnumerator RandomOrbTimer(float time)
    {
        _nextOrb = false;

        float x, y;
        bool carryOn = false;

        do
        {
            x = Random.Range(-1f * X_BOUND, X_BOUND);
            y = Random.Range(-1f * Y_BOUND, Y_BOUND);

            bool shouldCarryOn = false;

            for(int i = 0; i < SnakeGeneration.NumberOfPlayers; i++)
            {
                if(Vector3.Distance(this.GetComponent<SnakeGeneration>().snakeHeads[i].transform.position, new Vector3 (x, y, 0)) < MIN_DISTANCE_AWAY)
                {
                    shouldCarryOn = true;
                    break;
                }
            }

            if (shouldCarryOn)
                carryOn = true;

            else
                carryOn = false;

        } while(carryOn);

        int c = Random.Range(0, 3);

        while(time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        GameObject foo = Instantiate(tailOrb, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;

        AdjustColour(foo, c);

        foo.tag = "Orb";
        foo.GetComponent<CircleCollider2D>().isTrigger = true;
        foo.AddComponent<Tail>();
        foo.AddComponent<OrbAI>();
        _nextOrb = true;
    }

    IEnumerator RandomVirusTimer(int time)
    {
        _nextVirus = false;

        float x, y;
        bool carryOn = false;

        int c = Random.Range(0, 3);

        do
        {
            x = Random.Range(-1f * X_BOUND, X_BOUND);
            y = Random.Range(-1f * Y_BOUND, Y_BOUND);

            bool shouldCarryOn = false;

            for (int i = 0; i < SnakeGeneration.NumberOfPlayers; i++)
            {
                if (Vector3.Distance(this.GetComponent<SnakeGeneration>().snakeHeads[i].transform.position, new Vector3(x, y, 0)) < MIN_DISTANCE_AWAY)
                {
                    shouldCarryOn = true;
                    break;
                }
            }

            if (shouldCarryOn)
                carryOn = true;

            else
                carryOn = false;

        } while (carryOn);

        while(time > 0)
        {
            yield return new WaitForSeconds(1);
            time--;
        }

        GameObject foo = Instantiate(virus, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

        AdjustColour(foo, c);

        _nextVirus = true;
    }

    IEnumerator RandomFireballTimer(int time)
    {
        _nextFireballPickup = false;

        float x = Random.Range(-1f * X_BOUND, X_BOUND);
        float y = Random.Range(-1f * Y_BOUND, Y_BOUND);
        int c = Random.Range(0, 3);

        while (time > 0)
        {
            yield return new WaitForSeconds(1);
            time--;
        }

        Instantiate(fireballPickup, new Vector3(x, y, 0f), Quaternion.identity);

        _nextFireballPickup = true;
    }

    public void AdjustColour(GameObject g, int c)
    {
        if (c == 0)
            g.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f, 1f);

        else if (c == 1)
            g.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0f, 1f);

        else if (c == 2)
            g.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 1f, 1f);
    }
}
