using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] private Animator Fader;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject CanvasPause;
    private bool isPaused = false;
    private void Start()
    {
        AudioListener.pause = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            changePause();
        }
    }
    public void changePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;

        CanvasPause.SetActive(isPaused);

        AudioListener.pause = true;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;

        CanvasPause.SetActive(isPaused);

        AudioListener.pause = false;
    }
    public void ExitGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        
        gameManager.save();
        StartCoroutine(endGame());
    }

    private IEnumerator endGame()
    {
        Fader.SetBool("fadeIn", true);
        Fader.SetBool("fadeOut", false);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Menu");
    }
}
