using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Mushroom : MonoBehaviour, IDamagable
{
    [SerializeField] private AudioSource audioEat;
    [SerializeField] private ParticleSystem particle;
    public int indexOfPlace { get; set; }
    void ToHit() { }
    private void Start()
    {
        float scale = transform.localScale.x;
        transform.localScale = Vector2.zero;
        transform.DOScale(scale, 1f).SetEase(Ease.OutBounce);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.TryGetComponent(out HP player))
        {
            enabled = false;

            audioEat.Play();
            particle.Stop();

            player.GetDamage();

            transform.DOScale(0, 0.5f).SetEase(Ease.InBounce);

            Destroy(gameObject, 4);
        }
    }
}
