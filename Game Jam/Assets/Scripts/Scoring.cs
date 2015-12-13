using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Scoring : MonoBehaviour
{
    public Text playerOneText, playerTwoText, playerThreeText, playerFourText;
    private int _playerOneScore = 0;
    private int _playerTwoScore = 0;
    private int _playerThreeScore = 0;
    private int _playerFourScore = 0;
    private bool _gameOver = false;
    private bool _gameEnding = false;
    private const int POINTS_TO_WIN = 20;
    private const float GAME_END_TIME = 10f;
    public int winningPlayer;

    public Text winnerText;

    public PlayerHead[] playerHeads;

	void Start ()
    {
        playerHeads = new PlayerHead[SnakeGeneration.NumberOfPlayers];

        for (int i = 0; i < playerHeads.Length; i++)
            playerHeads[i] = this.GetComponent<SnakeGeneration>().snakeHeads[i].GetComponent<PlayerHead>();

        playerOneText.text = "P1 Score: 0";
        playerTwoText.text = "P2 Score: 0";

        if (playerHeads.Length >= 3)
            playerThreeText.text = "P3 Score: 0";

        else
            playerThreeText.text = "";

        if (playerHeads.Length >= 4)
            playerFourText.text = "P4 Score: 0";

        else
            playerFourText.text = "";
	}
	
	void Update ()
    {
        if (!_gameOver)
            UpdateScores();

        CheckWinCondition();

        if(_gameOver && !_gameEnding)
        {
            _gameEnding = true;

            StartCoroutine(EndGame(GAME_END_TIME));
        }
	}

    IEnumerator EndGame(float time)
    {
        for (int i = 0; i < SnakeGeneration.NumberOfPlayers; i++)
        {
            playerHeads[i].shouldMove = false;
        }

        this.GetComponent<RandomGenerator>().shouldSpawn = false;

        winnerText.text = "Player " + (winningPlayer + 1).ToString() + " wins!";

        while(time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        Application.LoadLevel(Application.loadedLevel);
    }

    public void CheckWinCondition()
    {
        if(_playerOneScore >= POINTS_TO_WIN)
        {
            winningPlayer = 0;
            _gameOver = true;
        }

        else if(_playerTwoScore >= POINTS_TO_WIN)
        {
            winningPlayer = 1;
            _gameOver = true;
        }

        else if(_playerThreeScore >= POINTS_TO_WIN)
        {
            winningPlayer = 2;
            _gameOver = true;
        }

        else if(_playerFourScore >= POINTS_TO_WIN)
        {
            winningPlayer = 3;
            _gameOver = true;
        }
    }

    public void UpdateScores()
    {
        _playerOneScore = playerHeads[0].score;
        _playerTwoScore = playerHeads[1].score;

        playerOneText.text = "P1 Score: " + _playerOneScore.ToString();
        playerTwoText.text = "P2 Score: " + _playerTwoScore.ToString();

        if (playerHeads.Length >= 3)
        {
            _playerThreeScore = playerHeads[2].score;
            playerThreeText.text = "P3 Score: " + _playerThreeScore.ToString();
        }

        if (playerHeads.Length >= 4)
        {
            _playerFourScore = playerHeads[3].score;
            playerFourText.text = "P4 Score: " + _playerFourScore.ToString();
        }
    }
}
