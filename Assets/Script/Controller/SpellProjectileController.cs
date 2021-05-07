using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpellProjectileController : MonoBehaviour
{
    public void StartHit(Vector3 target, UnityAction hitAction)
    {
        StartCoroutine(Move(target, hitAction));
    }

    private IEnumerator Move(Vector3 target, UnityAction hitAction)
    {
        yield return new WaitForSeconds(0.2f);
        //Detecta a distancia
        while (Mathf.Abs(Vector3.Distance(this.transform.position, target)) > 0f)
        {
            if (Vector3.Distance(this.transform.position, target) <= 0.5f)
            {
                break;
            }
            //Move a spell
            float step = 2f * Time.deltaTime;
            this.transform.position = Vector3.MoveTowards(this.transform.position, target, step);
            yield return new WaitForSeconds(0.005f);
        }
        if (Vector3.Distance(this.transform.position, target) <= 0.5f)
        {
            Destroy(this.gameObject);
            hitAction?.Invoke();
        }
    }
}
