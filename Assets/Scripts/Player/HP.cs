using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

public class HP : MonoBehaviour
{
    PlayerController playerController;
    [SerializeField] private Animator playerAnim;


    public GameManager gameManager;
    [SerializeField] private Animator Fader;
    Move move;
    [SerializeField] private int hp = 3;

    [SerializeField] private GameObject[] hpImage;

    private Camera _camera;
    private bool _canHit = true;
    private bool isView = false;

    [SerializeField] private GameObject ButtonViewReclam;

    private void Start()
    {
        _camera = Camera.main;
        playerController = GetComponent<PlayerController>();
        move = GetComponent<Move>();
    }
    public void GetDamage()
    {
        StartCoroutine(getDamage());
    }
    private IEnumerator getDamage()
    {
        if (_canHit)
        {
            hp--;
            hpImage[hp].transform.DOScale(0, 1).SetEase(Ease.InBounce);

            playerController.enabled = false;
            move.enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;

            _camera.DOOrthoSize(3f, 0.2f).SetEase(Ease.Linear);
            _camera.DOShakePosition(1, 1f);

            if (hp == 0)
            {
                if (isView == false)
                {
                    Fader.SetBool("fadeIn", true);
                    Fader.SetBool("fadeOut", false);

                    ButtonViewReclam.SetActive(true);
                }
                else
                {
                    gameManager.save();

                    Fader.SetBool("fadeIn", true);
                    Fader.SetBool("fadeOut", false);
                    yield return new WaitForSeconds(1);
                    SceneManager.LoadScene(0);
                }
            }
            else
            {
                StartCoroutine(noDamage());
                yield return StartCoroutine(gameManager.fade());

                playerController.enabled = true;
                move.enabled = true;
                GetComponent<BoxCollider2D>().enabled = true;

                _camera.DOOrthoSize(5, 0.3f).SetEase(Ease.Linear);
            }
        }
    }

    private IEnumerator noDamage()
    {
        playerAnim.SetBool("hit", true);
        _canHit = false;

        yield return new WaitForSeconds(5);

        playerAnim.SetBool("hit", false);
        _canHit = true;
    }

    private void OnEnable()
    {
        YandexGame.RewardVideoEvent += AddHp;
    }
    private void OnDisable()
    {
        YandexGame.RewardVideoEvent -= AddHp;
    }
    public void AddHp(int id)
    {
        ButtonViewReclam.SetActive(false);

        isView = true;

        StartCoroutine(noDamage());

        StartCoroutine(gameManager.fade());

        hpImage[hp].transform.DOScale(1, 4).SetEase(Ease.OutBounce);

        hp++;

        playerController.enabled = true;
        move.enabled = true;
        GetComponent<BoxCollider2D>().enabled = true;

        _camera.DOOrthoSize(5, 0.3f).SetEase(Ease.Linear);
    }


    public void ExampleOpenRewardAd(int id)
    {
        YandexGame.RewVideoShow(id);
    }
    public void Exit()
    {
        SceneManager.LoadScene(0);
    }
}
