using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
	private Animator munipoliBoardAnimator;
	private Animator playButtonAnimator;
	private Animator quitButtonAnimator;
	private Animator transitionPanelAnimator;
	private Animator offlineButtonAnimator;

	private TextMeshProUGUI playTextTMP;
	private TextMeshProUGUI offlineTextTMP;

	private bool isUIEnabled = false;
	private byte menuStage = 0;

	// Start is called before the first frame update
	public void Start()
	{
		munipoliBoardAnimator = GameObject.Find("MunipoliBoard").GetComponent<Animator>();
		playButtonAnimator = GameObject.Find("PlayButton").GetComponent<Animator>();
		quitButtonAnimator = GameObject.Find("QuitButton").GetComponent<Animator>();
		transitionPanelAnimator = GameObject.Find("TransitionPanel").GetComponent<Animator>();
		offlineButtonAnimator = GameObject.Find("OfflineButton").GetComponent<Animator>();

		playTextTMP = GameObject.Find("PlayButton").GetComponent<TextMeshProUGUI>();
		offlineTextTMP = GameObject.Find("OfflineText").GetComponent<TextMeshProUGUI>();

		// Trigger Animations
		StartCoroutine(nameof(TriggerEntrances), 1.0f);

		//Enable UI once animations done
		StartCoroutine(nameof(EnableUIinSeconds), 3.0f);
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			if (menuStage == 0)
				Application.Quit(0);
			else
			{
				menuStage--;
				playButtonAnimator.SetBool("otherSelected", false);
				playButtonAnimator.SetBool("pressed", false);
				playTextTMP.CrossFadeColor(Color.white, 0.4f, false, false);

				quitButtonAnimator.SetBool("otherSelected", false);
				offlineButtonAnimator.SetBool("isEnabled", false);
			}
	}

	private IEnumerator TriggerEntrances(float sec)
	{
		yield return new WaitForSeconds(sec);
		munipoliBoardAnimator.SetBool("boardEnter", true);
		playButtonAnimator.SetBool("canEnter", true);
		quitButtonAnimator.SetBool("canEnter", true);
		StopCoroutine(nameof(TriggerEntrances));
	}

	private IEnumerator EnableUIinSeconds(float sec)
	{
		yield return new WaitForSeconds(sec);
		isUIEnabled = true;
		StopCoroutine(nameof(EnableUIinSeconds));
	}

	public void OnPlayClick()
	{
		if (isUIEnabled)
		{
			// Open SubMenu
			quitButtonAnimator.SetBool("otherSelected", true);
			menuStage = 1;
			offlineButtonAnimator.SetBool("isEnabled", true);

			//Button Reacts to Press
			StartCoroutine(nameof(TemporarilyDisableUI), 1.7f);
			playButtonAnimator.SetBool("pressed", true);
			playTextTMP.CrossFadeColor(Color.yellow, 0.4f, false, false);
		}
	}

	private IEnumerator TemporarilyDisableUI(float sec)
	{
		isUIEnabled = false;
		yield return new WaitForSeconds(sec);
		isUIEnabled = true;
	}

	public void OnOfflineClick()
	{
		if (isUIEnabled)
		{
			// Trigger transition
			offlineTextTMP.color = Color.cyan;
			offlineButtonAnimator.SetBool("pressed", true);

			transitionPanelAnimator.SetBool("transiting", true);
			StartCoroutine(nameof(LoadGame), 2.0f);
		}
	}

	private IEnumerator LoadGame(float sec)
	{
		yield return new WaitForSeconds(sec);
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		StopCoroutine(nameof(LoadGame));
	}

	public void OnQuitClick()
	{
		quitButtonAnimator.SetBool("pressed", true);
		Application.Quit(0);
	}

} // class