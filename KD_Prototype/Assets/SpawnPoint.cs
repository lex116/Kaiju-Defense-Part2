using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    //might use later
    enum owner
    {
        attackers,
        defenders
    }

    [SerializeField]
    owner spawnOwner;

    [SerializeField]
    int spawnID; 

}
