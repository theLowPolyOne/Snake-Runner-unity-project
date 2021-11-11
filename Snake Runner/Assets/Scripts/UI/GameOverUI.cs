using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameOverUI : MonoBehaviour
{
	public GameObject firstOption;

	private void OnEnable()
	{
		EventSystem.current.SetSelectedGameObject(firstOption);
	}

	public void PlayAgain()
	{

		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void MainMenu()
	{
		SceneManager.LoadScene(0);
	}
}