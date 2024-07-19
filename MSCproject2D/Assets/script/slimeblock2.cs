using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class DeformableSlimeBlock : MonoBehaviour
{
    public float slowDownFactor = 0.5f;
    public float forceAmount = 10f;
    public float deformSpeed = 1.0f;
    public float restoreSpeed = 1.0f;
    public float maxDeformDistance = 1.0f;
    public float deformRadius = 1.0f;
    public float expandOffset = 1.0f;
    private float expandRadius;

    private List<Transform> edgePoints;
    private List<Vector3> originalPositions;
    private SpriteShapeController shapeController;
    private bool isDeforming = false;
    private List<Transform> objectsInSlime = new List<Transform>();
    private Coroutine deformCoroutine;
    void Start()
    {
        shapeController = GetComponent<SpriteShapeController>();
        edgePoints = new List<Transform>();
        originalPositions = new List<Vector3>();
        expandRadius = deformRadius + expandOffset;
        InitializeEdgePoints();
    }

    void InitializeEdgePoints()
    {
        for (int i = 0; i < shapeController.spline.GetPointCount(); i++)
        {
            Vector3 pointPosition = shapeController.spline.GetPosition(i);
            GameObject edgePoint = new GameObject("EdgePoint_" + i);
            pointPosition.y += this.transform.position.y; // add offset of slimeblock parent height
            //print(this.transform.parent.transform.position.y);
            //pointPosition.z = -0.5f;
            edgePoint.transform.position = pointPosition;
            
            edgePoint.transform.parent = transform;

            edgePoints.Add(edgePoint.transform);
            originalPositions.Add(edgePoint.transform.localPosition);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        /*if (other.CompareTag("Player"))
        {
            RollJump player = other.GetComponent<RollJump>();
            if (player != null)
            {
                //player.ModifySpeed(slowDownFactor);
                StartCoroutine(DeformEdgePoints(other.transform));
            }
        }
        if (other.CompareTag("Enemy"))
        {
            EnemyAStar enemy = other.GetComponent<EnemyAStar>();
            if (enemy != null)
            {
                //enemy.ModifySpeed(slowDownFactor);
                StartCoroutine(DeformEdgePoints(other.transform));
            }
        }*/
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            if (!objectsInSlime.Contains(other.transform))
            {
                objectsInSlime.Add(other.transform);

                if (deformCoroutine == null)
                {
                    deformCoroutine = StartCoroutine(DeformEdgePoints());
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        /*if (other.CompareTag("Player"))
        {
            RollJump player = other.GetComponent<RollJump>();
            if (player != null)
            {
                //player.ModifySpeed(1.0f / slowDownFactor);
                StartCoroutine(RestoreEdgePoints());
            }
        }
        if (other.CompareTag("Enemy"))
        {
            EnemyAStar enemy = other.GetComponent<EnemyAStar>();
            if (enemy != null)
            {
                StartCoroutine(RestoreEdgePoints());
            }
        }*/
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            if (objectsInSlime.Contains(other.transform))
            {
                objectsInSlime.Remove(other.transform);

                if (objectsInSlime.Count == 0 && deformCoroutine != null)
                {
                    StopCoroutine(deformCoroutine);
                    deformCoroutine = null;
                    StartCoroutine(RestoreEdgePoints());
                }
            }
        }
    }

    IEnumerator DeformEdgePoints()
    {
        print("startdeform");
        while (objectsInSlime.Count > 0)
        {
            for (int i = 0; i < edgePoints.Count; i++)
            {
                bool restoring = true;
                foreach (Transform obj in objectsInSlime)
                {
                    float distanceToObject = Vector3.Distance(edgePoints[i].localPosition + edgePoints[i].parent.localPosition, obj.localPosition);
                    if (distanceToObject < deformRadius)
                    {
                        Vector3 directionToObject = (obj.localPosition - edgePoints[i].localPosition - edgePoints[i].parent.localPosition).normalized;
                        Vector3 targetPosition = originalPositions[i] + directionToObject * Mathf.Min(maxDeformDistance, distanceToObject);
                        edgePoints[i].localPosition = Vector3.MoveTowards(edgePoints[i].localPosition, targetPosition, deformSpeed * Time.deltaTime);
                        restoring = false;
                    }
                    else if (distanceToObject < expandRadius)
                    {
                        Vector3 directionToObject = (obj.localPosition - edgePoints[i].localPosition - edgePoints[i].parent.localPosition).normalized;
                        Vector3 targetPosition = originalPositions[i] - directionToObject * maxDeformDistance;
                        edgePoints[i].localPosition = Vector3.MoveTowards(edgePoints[i].localPosition, targetPosition, deformSpeed * Time.deltaTime);
                        restoring = false;
                    }
                    
                }
                if (restoring)
                {
                    edgePoints[i].localPosition = Vector3.MoveTowards(edgePoints[i].localPosition, originalPositions[i], restoreSpeed * Time.deltaTime);
                }
            }

            UpdateSpriteShape();
            yield return null;
        }

        yield return RestoreEdgePoints();
    }

    IEnumerator RestoreEdgePoints()
    {
        print("restore");
        while (true)
        {
            bool allRestored = true;
            if (deformCoroutine != null) break;

            for (int i = 0; i < edgePoints.Count; i++)
            {
                edgePoints[i].localPosition = Vector3.MoveTowards(edgePoints[i].localPosition, originalPositions[i], restoreSpeed * Time.deltaTime);

                if (Vector3.Distance(edgePoints[i].localPosition, originalPositions[i]) > 0.01f)
                {
                    allRestored = false;
                }
            }

            UpdateSpriteShape();

            if (allRestored) break;

            yield return null;
        }

        yield return null;
    }

    void UpdateSpriteShape()
    {
        for (int i = 0; i < edgePoints.Count; i++)
        {
            shapeController.spline.SetPosition(i, edgePoints[i].localPosition);
        }
    }

    /*IEnumerator DeformEdgePoints()
    {
        //isDeforming = true;
        print("deformcor");
        while (true)
        {
            for (int i = 0; i < edgePoints.Count; i++)
            {
                float distanceToPlayer = Vector3.Distance(edgePoints[i].localPosition + edgePoints[i].parent.localPosition, player.transform.localPosition);
                if (distanceToPlayer < deformRadius)
                {
                    Vector3 directionToPlayer = (player.transform.localPosition - edgePoints[i].localPosition - edgePoints[i].parent.localPosition).normalized;
                    Vector3 targetPosition = originalPositions[i] + directionToPlayer * Mathf.Min(maxDeformDistance, distanceToPlayer);
                    edgePoints[i].localPosition = Vector3.MoveTowards(edgePoints[i].localPosition, targetPosition, deformSpeed * Time.deltaTime);
                }
                else if (distanceToPlayer < expandRadius)
                {
                    Vector3 directionToPlayer = (player.transform.localPosition - edgePoints[i].localPosition - edgePoints[i].parent.localPosition).normalized;
                    Vector3 targetPosition = originalPositions[i] - directionToPlayer * maxDeformDistance;
                    edgePoints[i].localPosition = Vector3.MoveTowards(edgePoints[i].localPosition, targetPosition, deformSpeed * Time.deltaTime);
                }
                else
                {
                    edgePoints[i].localPosition = Vector3.MoveTowards(edgePoints[i].localPosition, originalPositions[i], restoreSpeed * Time.deltaTime);
                }
            }
            print(name);
            UpdateSpriteShape();
            yield return null;
        }
    }

    IEnumerator RestoreEdgePoints()
    {
        //isDeforming = false;

        while (true)
        {
            bool allRestored = true;

            for (int i = 0; i < edgePoints.Count; i++)
            {
                edgePoints[i].localPosition = Vector3.MoveTowards(edgePoints[i].localPosition, originalPositions[i], restoreSpeed * Time.deltaTime);

                if (Vector3.Distance(edgePoints[i].localPosition, originalPositions[i]) > 0.01f)
                {
                    allRestored = false;
                }
            }

            UpdateSpriteShape();

            if (allRestored) break;

            yield return null;
        }

        yield return null;
    }*/

    
}
