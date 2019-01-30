using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Tower
{

    [SerializeField]
    private GameObject lineRendererPrefab;
    [SerializeField]
    private GameObject startPrefab;
    [SerializeField]
    private GameObject endPrefab;

    private bool firing;

    BeamCache[] beams;

    private GameObject[] firePoints;

    private void Awake()
    {
        TowerData = Def.Instance.TowerDictionary[Declarations.TowerType.Laser];
        firing = false;
        if (TowerData == null)
        {
            Debug.Log("null in init");
        }
    }

    private void Update()
    {
        if (LostTarget())
        {
            target = null;
            if (firing)
            {
                DisableBeams();
                firing = false;
            }
        }

        if (target != null)
        {
            LookAtTarget();
            if (CanShoot() && !firing)
            {
                Fire();
                firing = true;
            }
            if (firing)
            {
                PositionBeams();
            }
        }
        else
        {
            FindTarget();
        }
    }

    private void PositionBeams()
    {
        if (beams != null && beams.Length > 0 && target != null)
        {
            var endPosition = target.GetCenter();
            for (int i = 0; i < beams.Length; i++)
            {
                var beam = beams[i];
                var firePosition = firePoints[i].transform.position;

                beam.Start.transform.position = firePosition;

                beam.LineRenderer.positionCount = 2;
                beam.LineRenderer.SetPosition(0, firePosition);
                beam.LineRenderer.SetPosition(1, endPosition);

                beam.End.transform.position = endPosition;

                beam.Start.transform.LookAt(endPosition);
                beam.End.transform.LookAt(firePosition);

                target.DealDamage((TowerData as Declarations.LaserTower).CurrentDamage * Time.deltaTime);
            }
        }
    }

    private void DisableBeams()
    {
        if (beams != null && beams.Length > 0)
        {
            for (int i = 0; i < beams.Length; i++)
            {
                DestroyImmediate(beams[i].Start);
                DestroyImmediate(beams[i].LineRenderer);
                DestroyImmediate(beams[i].End);
            }
            beams = null;
        }
    }

    protected override void UpdateGunPartsReferences()
    {
        base.UpdateGunPartsReferences();

        var firePointsParent = currentGunHead.GetChild(0);
        firePoints = new GameObject[firePointsParent.childCount];
        for (int i = 0; i < firePointsParent.childCount; i++)
        {
            firePoints[i] = firePointsParent.GetChild(i).gameObject;
        }
    }

    private void Fire()
    {
        beams = new BeamCache[firePoints.Length];
        for (int i = 0; i < firePoints.Length; i++)
        {
            var beamStart = Instantiate(startPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            var beam = (Instantiate(lineRendererPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject).GetComponent<LineRenderer>();
            var beamEnd = Instantiate(endPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            beams[i] = new BeamCache(beamStart, beam, beamEnd);
        }
    }

    protected override void UpgradeTower()
    {
        Debug.Log("Upgrading tower");
        if (firing)
        {
            DisableBeams();
        }
        base.UpgradeTower();
        if (firing)
        {
            Fire();
            PositionBeams();
        }
    }

    class BeamCache
    {
        public LineRenderer LineRenderer;
        public GameObject Start;
        public GameObject End;

        public BeamCache(GameObject start, LineRenderer lineRenderer, GameObject end)
        {
            LineRenderer = lineRenderer;
            Start = start;
            End = end;
        }
    }
}
