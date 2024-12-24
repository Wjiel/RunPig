using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Thorns : MonoBehaviour, IDamagable
{
    [SerializeField] private float delay = 5;
    [SerializeField] private Animator animator;
    private bool isAttack;

    public int indexOfPlace { get; set; }

    Collider2D _collider;
    void ToHit() { }
    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        StartCoroutine(change());


        float scale = transform.localScale.x;
        transform.localScale = Vector2.zero;
        transform.DOScale(scale, 1f).SetEase(Ease.OutBounce);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isAttack)
            if (other.transform.TryGetComponent(out HP player))
            {
                player.GetDamage();
            }
    }

    private IEnumerator change()
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            animator.SetTrigger("change");
            isAttack = true;

            _collider.enabled = false;
            _collider.enabled = true;

            yield return new WaitForSeconds(1);
            animator.SetTrigger("change");
            isAttack = false;
        }
    }
}
