using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerSelectController : MonoBehaviour
{
	private Image changingBG;

	private int noOfPlayers;
	private int currentPlayer;

	private GameObject numberSelector;
	private GameObject nameInput;
	private GameObject tokenSelector;

	private Text ipPlayerName;

	public Toggle isAIToggle;

	private bool isAI;

	// Start is called before the first frame update
	public void Start()
	{
		changingBG = GameObject.Find("SpectrumCycler").GetComponent<Image>();

		numberSelector = GameObject.Find("Number Selector");
		nameInput = GameObject.Find("Name Input");
		tokenSelector = GameObject.Find("Token Selector");

		ipPlayerName = GameObject.Find("ipPlayerName").GetComponent<Text>();

		nameInput.SetActive(false);
		tokenSelector.SetActive(false);

		currentPlayer = 1;
	}

	// Update is called once per fixed framerate
	public void FixedUpdate()
	{
		CycleHueSpectrum();
	}

	private void CycleHueSpectrum()
	{
		var currentColour = changingBG.color;
		Color.RGBToHSV(currentColour, out float h, out _, out _);
		changingBG.color = Color.HSVToRGB((h + 0.001f) % 1f, 0.6f, 1f);
	}

	public void SetNoOfPlayers(int noOfPlayers)
	{
		this.noOfPlayers = noOfPlayers;
		PlayerPrefs.SetInt("NoOfPlayers", noOfPlayers);
		Debug.Log("No of players saved as: " + noOfPlayers);

		numberSelector.SetActive(false);
		nameInput.SetActive(true);
	}

	public void SetPlayerName()
	{
		PlayerPrefs.SetString("Player" + currentPlayer, ipPlayerName.text);
		Debug.Log("Player" + currentPlayer + " name set to " + ipPlayerName.text);

		if (isAIToggle.isOn == true)
			PlayerPrefs.SetInt("Player" + currentPlayer + "_isAI", 1);
		else
			PlayerPrefs.SetInt("Player" + currentPlayer + "_isAI", 0);

		tokenSelector.SetActive(true);
		tokenSelector.GetComponentInChildren<Text>().text = ipPlayerName.text;
		ipPlayerName.text = " ";

		Debug.Log("Player" + currentPlayer + "_isAI" + " set to " + true);
		nameInput.SetActive(false);
		isAIToggle.isOn = false;
	}

	public void SetPlayersToken(string tokenName)
	{
		PlayerPrefs.SetString("Player" + currentPlayer + "_Token", tokenName);
		Debug.Log("Player" + currentPlayer + "_Token" + " set to " + tokenName);
		tokenSelector.SetActive(false);
		currentPlayer++;

		if (currentPlayer <= noOfPlayers)
		{
			nameInput.SetActive(true);
			nameInput.GetComponentInChildren<TextMeshProUGUI>().SetText("Enter name for Player " + currentPlayer);
			ipPlayerName.text = "";
		}
		else
		{
			TransitionToGame();
		}
	}

	public void TransitionToGame()
	{
		GameObject.Find("TransitionPanel").GetComponent<Animator>().SetBool("transiting", true);
		StartCoroutine(nameof(LoadGame), 2f);
	}

	private IEnumerator LoadGame(float sec)
	{
		yield return new WaitForSeconds(sec);
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		StopCoroutine(nameof(LoadGame));
	}

} // PlayerSelectController