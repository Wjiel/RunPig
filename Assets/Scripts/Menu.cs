using UnityEngine;
using UnityEngine.SceneManagement;
using YG;
using TMPro;
using System.Collections;
public class Menu : MonoBehaviour
{
    [SerializeField] private Animator fader;
    [SerializeField] private TextMeshProUGUI RecordText;

    [SerializeField] private TMP_FontAsset jaAssets;
    private void Start()
    {

        if (YandexGame.SDKEnabled)
        {
            LoadSaveCloud();
        }
    }
    private void OnEnable()
    {
        YandexGame.GetDataEvent += LoadSaveCloud;
    }
    private void OnDisable()
    {
        YandexGame.GetDataEvent -= LoadSaveCloud;
    }
    private void LoadSaveCloud()
    {

        if (YandexGame.EnvironmentData.language == "ru")
            RecordText.text = "Лучший результат: " + YandexGame.savesData.total.ToString();
        else if (YandexGame.EnvironmentData.language == "en")
            RecordText.text = "The best result: " + YandexGame.savesData.total.ToString();
        else if (YandexGame.EnvironmentData.language == "tr")
            RecordText.text = "En iyi sonuç: " + YandexGame.savesData.total.ToString();
        else if (YandexGame.EnvironmentData.language == "es")
            RecordText.text = "Mejor resultado: " + YandexGame.savesData.total.ToString();
        else if (YandexGame.EnvironmentData.language == "az")
            RecordText.text = "Ən yaxşı nəticə: " + YandexGame.savesData.total.ToString();
        else if (YandexGame.EnvironmentData.language == "be")
            RecordText.text = "Лепшы вынік: " + YandexGame.savesData.total.ToString(); 
        else if (YandexGame.EnvironmentData.language == "kk")
            RecordText.text = "Үздік нәтиже: " + YandexGame.savesData.total.ToString(); 
        else if (YandexGame.EnvironmentData.language == "uz")
            RecordText.text = "Eng yaxshi natija: " + YandexGame.savesData.total.ToString(); 
        else if (YandexGame.EnvironmentData.language == "de")
            RecordText.text = "Bestes Ergebnis: " + YandexGame.savesData.total.ToString();
    }
    public void StartGame()
    {
        StartCoroutine(start());
    }
    public IEnumerator start()
    {
        fader.SetBool("fadeIn", true);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(1);
    }
}
