using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public int humanPoints;
	public int gemPoints;

	public int highscore;

	public delegate void PointDelegate(int point);
	public event PointDelegate newHumanPointEvent;
	public event PointDelegate newGemPointEvent;
	public event PointDelegate newHighscoreEvent;

	public event Action pauseGameEvent;
	public event Action unpauseGameEvent;

	public static GameManager Instance = null;

	enum GameState
	{
		playing,
		paused,
		gameOver
	}
	GameState _gameState;

	void Awake()
	{
		humanPoints = 0;
		gemPoints = 0;

		highscore = PlayerPrefs.GetInt("Highscore");

		FindObjectOfType<PlayerController>().deathEvent += OnPlayerDeath;
		FindObjectOfType<PlayerController>().eatHumanEvent += OnEatHumanEvent;
		FindObjectOfType<PlayerController>().eatGemEvent += OnEatGemEvent;

		_gameState = GameState.playing;

		Time.timeScale = 1;
	}

	private void Start()
	{
		Application.targetFrameRate = 60;

		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}

		if (highscore > 0)
			newHighscoreEvent?.Invoke(highscore);
	}

	void Update()
	{
		switch (_gameState)
		{
			case GameState.playing:
				PlayingUpdate();
				break;
			case GameState.paused:
				PausedUpdate();
				break;
			case GameState.gameOver:
				GameOverUpdate();
				break;
		}
	}

	void PlayingUpdate()
	{
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown("joystick button 7"))
		{
			_gameState = GameState.paused;
			Time.timeScale = 0;
			pauseGameEvent();
		}
	}

	void PausedUpdate()
	{
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown("joystick button 7"))
		{
			_gameState = GameState.playing;
			Time.timeScale = 1;
			unpauseGameEvent();
		}
	}

	void GameOverUpdate()
	{

	}

	void OnEatHumanEvent()
	{
		humanPoints++;
		newHumanPointEvent?.Invoke(humanPoints);
	}

	void OnEatGemEvent()
	{
		gemPoints++;
		newGemPointEvent?.Invoke(gemPoints);
	}

	public void ResetGemPoints()
	{
		gemPoints = 0;
		newGemPointEvent?.Invoke(gemPoints);
	}

	void OnPlayerDeath()
	{
		Time.timeScale = 0;

		if (humanPoints > highscore)
		{
			highscore = humanPoints;
			newHighscoreEvent?.Invoke(highscore);
			PlayerPrefs.SetInt("Highscore", highscore);
		}
		PlayerPrefs.Save();

		_gameState = GameState.gameOver;
	}
}