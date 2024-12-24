using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using YG;

public class GameManager : MonoBehaviour
{
    [SerializeField] private AudioSource eatAudio;
    [SerializeField] private Animator Fader;

    [SerializeField] private Transform Player;

    private int _total = 0;

    [SerializeField] private TextMeshProUGUI textTotal;

    [Header("Fruits")]
    [SerializeField] private int MaxFriutsOnMap;
    [SerializeField] private GameObject[] fruitPrefabs;
    [SerializeField] private float DelayBetweenSpawnFruits;
    private int currentFruits;
    private float timeFruits;

    [Header("Enemy")]
    [SerializeField] private int MaxEnemiesOnMap;
    [SerializeField] private Transform[] EnemyPrefab;
    [SerializeField] private Transform[] PlaceForSpawn;
    private List<int> PlaceIsEmpty = new List<int>();
    [SerializeField] private float DelayBetweenSpawnEnemy;
    private int currentEnemy;

    private Coroutine spawnRun;

    private void Start()
    {
        timeFruits = DelayBetweenSpawnFruits;

        spawnRun = StartCoroutine(spawnEnemy());

    }
    private void FixedUpdate()
    {
        if (currentFruits != MaxFriutsOnMap)
            if (timeFruits > 0)
            {
                timeFruits -= Time.deltaTime;
            }
            else
            {
                int random = Random.Range(0, PlaceForSpawn.Length - 1);

                if (isEmptyPlace(random))
                {
                    timeFruits = DelayBetweenSpawnFruits;
                    currentFruits++;

                    Instantiate(fruitPrefabs[Random.Range(0, fruitPrefabs.Length - 1)], PlaceForSpawn[random].position, Quaternion.identity);
                }
            }


    }
    private IEnumerator spawnEnemy()
    {
        while (currentEnemy != MaxEnemiesOnMap)
        {

            int random = Random.Range(0, PlaceForSpawn.Length - 1);

            if (isEmptyPlace(random))
            {
                PlaceIsEmpty.Add(random);

                Instantiate(EnemyPrefab[Random.Range(0, EnemyPrefab.Length)],
                 PlaceForSpawn[random].position, Quaternion.identity).GetComponent<IDamagable>().indexOfPlace = random;

                currentEnemy++;
            }

            yield return new WaitForSeconds(DelayBetweenSpawnEnemy);
        }

        spawnRun = null;
    }
    private bool isEmptyPlace(int _random)
    {
        foreach (int index in PlaceIsEmpty)
        {
            if (_random == index)
            {
                return false;
            }
        }

        return true;
    }
    private void OnEnable()
    {
        Fruits.PickUpFruits += PickUpFruit;
        ForestWall.removeArray += DestroyEnemy;
    }
    private void OnDisable()
    {
        Fruits.PickUpFruits -= PickUpFruit;
        ForestWall.removeArray -= DestroyEnemy;
    }
    private void DestroyEnemy(int index)
    {
        for (int i = 0; i < PlaceIsEmpty.Count; i++)
        {
            if (PlaceIsEmpty[i] == index)
            {
                PlaceIsEmpty.RemoveAt(i);
                currentEnemy--;
                break;
            }
        }

        if (spawnRun == null)
            spawnRun = StartCoroutine(spawnEnemy());
    }
    private void PickUpFruit(int total)
    {
        _total += total;
        textTotal.text = _total.ToString();

        currentFruits--;

        eatAudio.Play();
        save();
    }

    public IEnumerator fade()
    {
        Fader.SetBool("fadeIn", true);
        Fader.SetBool("fadeOut", false);

        yield return new WaitForSeconds(2);

        while (true)
        {
            int random = Random.Range(0, PlaceForSpawn.Length - 1);

            if (isEmptyPlace(random))
            {
                Player.position = PlaceForSpawn[random].position;

                break;
            }
        }


        Fader.SetBool("fadeIn", false);
        Fader.SetBool("fadeOut", true);
    }
    public void save()
    {
        if (_total >= YandexGame.savesData.total)
        {
            YandexGame.savesData.total = _total;
            YandexGame.SaveProgress();
            YandexGame.NewLeaderboardScores("LeaderBoard", _total);
        }
    }
}
