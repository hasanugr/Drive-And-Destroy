using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackBlock : MonoBehaviour
{
    private float _leanTweenProccess;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("PlayerShip") && _leanTweenProccess == 0)
        {
            print("You cant go Back");
            var seq = LeanTween.sequence();
            seq.append(LeanTween.value(gameObject, 0, 1, 0.1f).setOnUpdate((float val) =>
            {
                _leanTweenProccess = val;
            }));
            seq.append(LeanTween.alpha(gameObject, 1, 1).setEaseOutExpo());
            seq.append(LeanTween.alpha(gameObject, 0, 1).setEaseInExpo());
            seq.append(LeanTween.value(gameObject, 1, 0, 0.1f).setOnUpdate((float val) =>
            {
                _leanTweenProccess = val;
            }));
        }
    }
}
