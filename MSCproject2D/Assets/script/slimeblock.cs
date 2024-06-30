using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

public class slimeblock : MonoBehaviour
{
    public float slowDownFactor = 0.5f;
    public float deformationAmount = 0.1f;
    public float deformationSpeed = 1.0f;
    public float restorationSpeed = 1.0f;

    private Vector3 originalScale;
    private Coroutine deformationCoroutine;

    private PolygonCollider2D polygonCollider;
    void Start()
    {
        originalScale = transform.localScale;
        polygonCollider = GetComponent<PolygonCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            print("enter");
            RollJump player = other.GetComponent<RollJump>();
            if (player != null)
            {
                //player.ModifySpeed(slowDownFactor);
                if (deformationCoroutine != null) StopCoroutine(deformationCoroutine);
                deformationCoroutine = StartCoroutine(Deform());
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RollJump player = other.GetComponent<RollJump>();
            if (player != null)
            {
                //player.ModifySpeed(1.0f / slowDownFactor);
                if (deformationCoroutine != null) StopCoroutine(deformationCoroutine);
                deformationCoroutine = StartCoroutine(Restore());
            }
        }
    }

    

    private IEnumerator Deform()
    {
        while (transform.localScale.y > originalScale.y - deformationAmount)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y - Time.deltaTime * deformationSpeed, transform.localScale.z);
            yield return null;
        }
    }

    private IEnumerator Restore()
    {
        while (transform.localScale.y < originalScale.y)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + Time.deltaTime * restorationSpeed, transform.localScale.z);
            yield return null;
        }
        transform.localScale = originalScale;  // Ensure it returns exactly to the original scale
    }
}
