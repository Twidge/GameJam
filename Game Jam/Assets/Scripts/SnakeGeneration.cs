using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SnakeGeneration : MonoBehaviour
{
    public static int NumberOfPlayers = 4;
    public int MaxNumberOfNeutrals = 4;
    public int NeutralSpawnRate = 1;

    //x, y, moveLeftKey, moveRightKey
    public string[] PlayerSetup;

    private List<PlayerConfig> _playerConfigs;

    private const int startingSegments = 5; // Number of starting segments
    private const int tailColours = 3;

    private GameObject[] snakes; // Snakes
    private GameObject[] segments; // Tail segments
    public GameObject snake; // Snake prefab
    public GameObject[] snakeHeads; // Snake heads
    public GameObject tailSegment; // Tail segment prefab
    public Canvas canvas; // Canvas (NOT prefab)
    public Text countdownText; // Countdown text

    public struct PlayerConfig
    {
        public float xPos;
        public float yPos;
        public string moveLeft;
        public string moveRight;
        public float startingTravelDirection;
    }

    private void Start()
    {
        int foo; // Random number
        int bar = 0;

        snakes = new GameObject[NumberOfPlayers]; // Snakes

        segments = new GameObject[startingSegments];
        snakeHeads = new GameObject[snakes.Length];

        if (PlayerSetup.Length != NumberOfPlayers)
        {
            throw new NotSupportedException("The number of players must correspond to the number of player setups");
        }

        SetupPlayerConfigs();

        for (int i = 0; i < snakes.Length; i++)
        {
            snakes[i] = Instantiate(snake, Vector2.zero, Quaternion.identity) as GameObject;

            var newSnakeHead = Instantiate(Resources.Load("Prefabs/PlayerHead" + (i + 1)), Vector2.zero, Quaternion.identity) as GameObject;
            var playerConfig = _playerConfigs.First();
            _playerConfigs.Remove(_playerConfigs.First());

            if (newSnakeHead != null)
            {
                newSnakeHead.transform.parent = snakes[i].transform;
                newSnakeHead.transform.position = new Vector3(playerConfig.xPos, playerConfig.yPos, 0);
                newSnakeHead.GetComponent<PlayerHead>().SetControlsAndStartingDirection(playerConfig.moveLeft, playerConfig.moveRight, playerConfig.startingTravelDirection);

                snakeHeads[i] = newSnakeHead;
            }
            else
            {
                throw new NullReferenceException("Could not Instantiate snake head");
            }

            for (int j = 0; j < startingSegments; j++)
            {
                segments[j] = Instantiate(tailSegment, Vector3.zero, Quaternion.identity) as GameObject;
                segments[j].AddComponent<Tail>();
                segments[j].GetComponent<Tail>().SetParent(snakeHeads[i]);

                if (j == 0)
                    foo = Random.Range(0, tailColours);

                else
                {
                    do
                    {
                        foo = Random.Range(0, tailColours);
                    } while (foo == bar);
                }

                if (foo == 0)
                    segments[j].GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f, 1f);

                else if (foo == 1)
                    segments[j].GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0f, 1f);

                else if (foo == 2)
                    segments[j].GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 1f, 1f);

                snakeHeads[i].GetComponent<Head>().AddToTail(foo);
                snakeHeads[i].GetComponent<Head>().AddToTailObjects(segments[j]);

                segments[j].transform.parent = snakes[i].transform;

                bar = foo;
            }
        }

        StartCoroutine(CountdownText());
    }

    public System.Collections.IEnumerator CountdownText()
    {
        float time = 4;
        countdownText.text = "3...";

        while(time > 3)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        countdownText.text = "2...";

        while(time > 2)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        countdownText.text = "1...";

        while(time > 1)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        countdownText.text = "GO!";

        while(time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        countdownText.text = "";

        foreach(GameObject s in snakeHeads)
        {
            s.GetComponent<Head>().shouldMove = true;
        }

        this.GetComponent<RandomGenerator>().shouldSpawn = true;
    }

    private void SetupPlayerConfigs()
    {
        _playerConfigs = new List<PlayerConfig>(NumberOfPlayers);

        foreach (string playerSetup in PlayerSetup)
        {
            string[] info = playerSetup.Replace(" ", "").Split(',');

            _playerConfigs.Add(new PlayerConfig
            {
                xPos = float.Parse(info[0]),
                yPos = float.Parse(info[1]),
                moveLeft = info[2],
                moveRight = info[3],
                startingTravelDirection = float.Parse(info[4]),
            });
        }
    }
}