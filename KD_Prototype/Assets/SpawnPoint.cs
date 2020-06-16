using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    //might use later
    public enum owner
    {
        player,
        enemy
    }

    [SerializeField]
    public owner spawnOwner;

    [SerializeField]
    public int spawnID; 
}
