using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public LineRenderer glowRenderer;

    Vector3[] points;

    GameObject start;
    Enemy end;
    
    [SerializeField]
    AudioClip[] lightningSounds;
    [SerializeField]
    AudioSource audioSource;

    public void GenerateLightning(GameObject start, Enemy end)
    {
        this.start = start;
        this.end = end;
        GeneratePositons();
        StartCoroutine(SetPositions());
    }

    private void GeneratePositons()
    {
        if (start != null && end != null)
        {
            if (!audioSource.isPlaying && !GameManager.instance.Paused)
            {
                audioSource.clip = lightningSounds[UnityEngine.Random.Range(0, lightningSounds.Length)];
                audioSource.Play();
            }
            Debug.Log(audioSource.outputAudioMixerGroup.name);
            var startPos = Vector3.zero;
            var startEnemy = start.GetComponent<Enemy>();
            if (startEnemy != null)
            {
                startPos = startEnemy.GetCenter();
            }
            else
            {
                startPos = start.transform.position;
            }

            transform.position = startPos;
            transform.LookAt(end.GetCenter());
            var noise = Random.Range(0.2f, 0.3f);
            var distance = Vector3.Distance(startPos, end.GetCenter());

            var positions = new List<Vector3>();
            positions.Add(Vector3.zero);

            var distanceleft = distance;
            var endPoint = new Vector3(0, 0, distance);
            while (distanceleft > 0)
            {
                var spaceToMove = 0.0f;
                if (distanceleft <= 0.3)
                {
                    spaceToMove = distanceleft / 2;
                    distanceleft = 0;
                }
                else
                {
                    spaceToMove = Random.Range(0.25f, 0.5f);
                }
                var dir = endPoint - positions[positions.Count - 1];
                var tempPos = positions[positions.Count - 1] + (dir.normalized * spaceToMove);
                var newPos = new Vector3(tempPos.x + Random.Range(-noise, noise), tempPos.y + Random.Range(-noise, noise), tempPos.z);
                positions.Add(newPos);

                if (distanceleft != 0)
                {
                    distanceleft = Vector3.Distance(newPos, endPoint);
                }
                else
                {
                    break;
                }
            }
            positions.Add(endPoint);
            points = positions.ToArray();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator SetPositions()
    {
        lineRenderer.positionCount = 0;
        glowRenderer.positionCount = 0;
        for (int i = 0; i < points.Length; i++)
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(i, points[i]);
            glowRenderer.positionCount++;
            glowRenderer.SetPosition(i, points[i]);
            if (i % 15 == 0)
                yield return new WaitForSeconds(0.0005f);
        }

        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        for (int i = 0; i < 10; i++)
        {
            GeneratePositons();
            lineRenderer.positionCount = points.Length;
            glowRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
            glowRenderer.SetPositions(points);
            yield return new WaitForSeconds(0.02f);
        }

        Destroy(gameObject);
    }
}
