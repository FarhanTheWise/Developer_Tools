using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoolObject", menuName = "ScriptableObjects/Pool Object")]
public class PoolObject : ScriptableObject
{
    [Header("General Settings")]
    public string poolTitle;
    public bool OnAwake;
    public PoolName poolType;
    public GameObject prefab;
    public int size;
    public GameObject objectGroup;
    
}

public enum PoolName
{
    BulletTrails = -1,

    //Start of Enemy Pool
    NpcFlagCapture = 0,
    NpcZoneControl = 1,
    NpcZombie = 2,
    DeathCam = 3,
    BulletEffects = 4,
    EnvironmentEffects = 5,
    AssistHolder = 6,
    EliminationHolder = 7,
    MissionWaypoint = 8,
    HealthRefillBonus = 9,
    HealthBoostBonus = 10,
    WeaponBoostBonus = 11
    

}
