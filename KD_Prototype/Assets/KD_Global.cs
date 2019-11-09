using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class KD_Global 
{
    public enum FactionTag
    {
        UTF,
        SER,
        GSR,
        HPA,
        SAA,
        SCRAPS,
        Neutral
    }
    public enum Characters
    {
        Character_SER_Samuel,
        Character_SER_Szymon,
        Character_SER_Emir,
        Character_SER_Kostas,
        Character_SER_Thomas,
        Character_SER_Mecha_Xiphos,
        Character_SER_Mecha_Rogatina,
        Character_SCRAPS_Wanderlei,
        Character_SCRAPS_Anderson,
        Character_SCRAPS_Arlo,
        Character_SCRAPS_Mason,
        Character_SCRAPS_Noah,
        Character_SCRAPS_Mecha_Flyboy,
        Character_SCRAPS_Vehicle_Cobra,
        Character_GSR_Fedor,
        Character_GSR_Khabib,
        Character_GSR_Rustam
    } 
    public enum UnitColors
    {
        Skin_Blue,
        Skin_Yellow,
        Skin_Red,
        Skin_White
    }
    static public Sprite[] MapUnitIcons;
    static public Color[] MapIconColors =
    {
        Color.blue,
        Color.yellow,
        Color.red,
        Color.white
    };

    //unti spawning
    static public void SpawnUnit(Blueprint_Unit_Master unitToSpawn, SpawnPoint spawnPoint)
    {
        //file path example for later
        //WeaponModel = (Resources.Load<GameObject>("KD_Assets/KD_Prefabs/Weapon_Human_AntiArmorRifle_Model"));

        //temp  
        Unit_Master tempUnitScript = null;
        GameObject tempUnitObject = null;

        tempUnitObject = Object.Instantiate(unitToSpawn.unitObject, spawnPoint.transform.position, spawnPoint.transform.rotation);
        tempUnitScript = tempUnitObject.GetComponent<Unit_Master>();
        tempUnitScript.Setup(unitToSpawn.infantrySheet, unitToSpawn.vehicleSheet);
    }

    static public void AssignTeamColorsToUnit(Unit_Master UnitToColor)
    {
        foreach (MeshRenderer x in UnitToColor.UnitSkins)
        {
            string filepath = "KD_Assets/Materials/" + UnitToColor.characterSheet.UnitStat_FactionTag.ToString();

            //x.material = Skins[(int)UnitToColor.characterSheet.UnitStat_FactionTag];
            x.material = Resources.Load<Material>(filepath);
                
        }
        
        UnitToColor.MapIconHighlight.color = MapIconColors[(int)UnitToColor.characterSheet.UnitStat_FactionTag];
        UnitToColor.UnitIcon.sprite = MapUnitIcons[(int)UnitToColor.characterSheet.unitType];
    }

}
