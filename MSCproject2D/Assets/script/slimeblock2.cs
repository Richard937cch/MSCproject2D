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
    public float deformRadius = 2.0f;

    private List<Transform> edgePoints;
    private List<Vector3> originalPositions;
    private SpriteShapeController shapeController;
    private bool isDeforming = false;

    void Start()
    {
        shapeController = GetComponent<SpriteShapeController>();
        edgePoints = new List<Transform>();
        originalPositions = new List<Vector3>();

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
        if (other.CompareTag("Player"))
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
        }
    }

    IEnumerator DeformEdgePoints(Transform player)
    {
        isDeforming = true;

        while (isDeforming)
        {
            for (int i = 0; i < edgePoints.Count; i++)
            {
                float distanceToPlayer = Vector3.Distance(edgePoints[i].position, player.position);
                if (distanceToPlayer < deformRadius)
                {
                    Vector3 directionToPlayer = (player.position - edgePoints[i].position).normalized;
                    Vector3 targetPosition = originalPositions[i] + directionToPlayer * Mathf.Min(maxDeformDistance, distanceToPlayer);
                    edgePoints[i].localPosition = Vector3.MoveTowards(edgePoints[i].localPosition, targetPosition, deformSpeed * Time.deltaTime);
                }
            }

            UpdateSpriteShape();
            yield return null;
        }
    }

    IEnumerator RestoreEdgePoints()
    {
        isDeforming = false;

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
    }

    void UpdateSpriteShape()
    {
        for (int i = 0; i < edgePoints.Count; i++)
        {
            shapeController.spline.SetPosition(i, edgePoints[i].localPosition);
        }
    }
}
