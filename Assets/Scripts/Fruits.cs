using System;
using DG.Tweening;
using UnityEngine;

public class Fruits : MonoBehaviour
{
    public static event Action<int> PickUpFruits;
    [SerializeField, Range(1, 10)] private int total = 0;
    private void Start()
    {
        float scale = transform.localScale.x;
        transform.localScale = Vector2.zero;

        transform.DOScale(scale, 1f).SetEase(Ease.OutBounce);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            enabled = false;

            PickUpFruits?.Invoke(total);

            transform.DOScale(0, 0.5f).SetEase(Ease.InBounce);

            Destroy(gameObject, 1);
        }
    }
}
