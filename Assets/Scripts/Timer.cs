using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    [SerializeField] private Animator Fader;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float GameTime;
    private float _currentTime;
    private void Start() => _currentTime = GameTime;
    private void FixedUpdate()
    {
        if (_currentTime > 0)
        {
            _currentTime -= Time.deltaTime;
            float minutes = Mathf.FloorToInt(_currentTime / 60);
            float seconds = Mathf.FloorToInt(_currentTime % 60);
            text.text = string.Format("{0:00} : {1:00}", minutes, seconds);
        }
        else
        {
            text.text = "00 : 00";
            enabled = false;
            StartCoroutine(endGame());
        }
    }

    private IEnumerator endGame()
    {
        gameManager.save();

        Fader.SetBool("fadeIn", true);
        Fader.SetBool("fadeOut", false);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Menu");
    }
}
