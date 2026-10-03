using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class WaypointManager : MonoBehaviour
{

    [Header("General Settings")]
    public List<GameObject> waypointsActive;


    public static Action<MissionWaypointUpdated> setWaypoint = delegate{};
    public static Action<GameObject> disableWaypoint = delegate{};
    void Awake()
    {
        setWaypoint += SpawnWaypoint;
        disableWaypoint += DisableWaypoint;
    }

    void OnDestroy()
    {
        setWaypoint -= SpawnWaypoint;
        disableWaypoint -= DisableWaypoint;
    }

    private void SpawnWaypoint(MissionWaypointUpdated missionWaypoint)
    {
        var waypoint = PoolingManager.instance.GetSpawnObject<Image>(
            PoolName.MissionWaypoint, Vector3.zero, Quaternion.identity, transform);

        waypoint.gameObject.SetActive(true);
        missionWaypoint.SetImage(waypoint);
        missionWaypoint.enabled = true;
        missionWaypoint.Enable();

        waypointsActive.Add(missionWaypoint.gameObject);
    }

    private void DisableWaypoint(GameObject waypointObject)
    {
        waypointObject.SetActive(false);
        PoolingManager.instance.ReturnToPool(PoolName.MissionWaypoint, waypointObject);
        waypointsActive.Remove(waypointObject);
    }

    public void ResetAll()
    {
        foreach(var waypoint in waypointsActive)
        {
            PoolingManager.instance.ReturnToPool(PoolName.MissionWaypoint, waypoint);
        }
    }
}
