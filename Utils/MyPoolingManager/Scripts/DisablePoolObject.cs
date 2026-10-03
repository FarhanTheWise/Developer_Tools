using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisablePoolObject : MonoBehaviour
{
    public float disableDelay = 1f;

    private void OnEnable()
    {
        StartCoroutine(DisableThis());   
    }

    private void OnDisable()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
/*        TryGetComponent<TrailRenderer>(out var renderer);
        renderer.emitting = false;*/
        // gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
    }

    IEnumerator DisableThis()
    {
        yield return new WaitForSeconds(disableDelay);
       
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Ground"))
        {
            gameObject.SetActive(false);
        }
      
    }
}
