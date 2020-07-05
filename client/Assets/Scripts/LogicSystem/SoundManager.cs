using protocol.cs_theircraft;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    static readonly string Material_Grass = "Grass";
    static readonly string Material_Gravel = "Gravel";
    static readonly string Material_Sand = "Sand";
    static readonly string Material_Stone = "Stone";
    static readonly string Material_Wood = "Wood";
    static readonly string Material_Glass = "Glass";

    static Dictionary<CSBlockType, string> type2material = new Dictionary<CSBlockType, string>
    {
        {CSBlockType.GrassBlock, Material_Grass },

        {CSBlockType.Dirt, Material_Gravel },
        {CSBlockType.Gravel, Material_Gravel },

        {CSBlockType.Brick, Material_Stone },
        {CSBlockType.BrickStairs, Material_Stone },
        {CSBlockType.Furnace, Material_Stone },
        {CSBlockType.BrickWall, Material_Stone },
        {CSBlockType.Stone, Material_Stone },
        {CSBlockType.DoubleStoneSlab, Material_Stone },
        {CSBlockType.BedRock, Material_Stone },
        {CSBlockType.CoalOre, Material_Stone },
        {CSBlockType.IronOre, Material_Stone },
        {CSBlockType.GoldOre, Material_Stone },
        {CSBlockType.DiamondOre, Material_Stone },
        {CSBlockType.EmeraldOre, Material_Stone },
        {CSBlockType.RedstoneOre, Material_Stone },
        {CSBlockType.CoalBlock, Material_Stone },
        {CSBlockType.IronBlock, Material_Stone },
        {CSBlockType.GoldBlock, Material_Stone },
        {CSBlockType.DiamondBlock, Material_Stone },
        {CSBlockType.EmeraldBlock, Material_Stone },
        {CSBlockType.RedstoneBlock, Material_Stone },
        {CSBlockType.CobblestoneStairs, Material_Stone },
        {CSBlockType.StoneBrickStairs, Material_Stone },
        {CSBlockType.NetherBrickStairs, Material_Stone },
        {CSBlockType.SandstoneStairs, Material_Stone },
        {CSBlockType.QuartzStairs, Material_Stone },
        {CSBlockType.Cobblestone, Material_Stone },
        {CSBlockType.StoneBricks, Material_Stone },
        {CSBlockType.CobblestoneWall, Material_Stone },
        {CSBlockType.MossyCobblestone, Material_Stone },
        {CSBlockType.MossyCobblestoneWall, Material_Stone },
        {CSBlockType.MossyStoneBricks, Material_Stone },
        {CSBlockType.MossyStoneBrickStairs, Material_Stone },
        {CSBlockType.MossyStoneBrickWall, Material_Stone },
        {CSBlockType.StoneSlab, Material_Stone },
        {CSBlockType.SmoothStoneSlab, Material_Stone },
        {CSBlockType.CobblestoneSlab, Material_Stone },
        {CSBlockType.MossyCobblestoneSlab, Material_Stone },
        {CSBlockType.StoneBrickSlab, Material_Stone },
        {CSBlockType.MossyStoneBrickSlab, Material_Stone },
        {CSBlockType.BrickSlab, Material_Stone },
        {CSBlockType.NetherBrickSlab, Material_Stone },
        {CSBlockType.QuartzSlab, Material_Stone },

        {CSBlockType.Torch, Material_Wood },
        {CSBlockType.OakLog, Material_Wood },
        {CSBlockType.SpruceLog, Material_Wood },
        {CSBlockType.BirchLog, Material_Wood },
        {CSBlockType.JungleLog, Material_Wood },
        {CSBlockType.AcaciaLog, Material_Wood },
        {CSBlockType.DarkOakLog, Material_Wood },
        {CSBlockType.OakPlanks, Material_Wood },
        {CSBlockType.OakWoodStairs, Material_Wood },
        {CSBlockType.SpruceWoodStairs, Material_Wood },
        {CSBlockType.BirchWoodStairs, Material_Wood },
        {CSBlockType.JungleWoodStairs, Material_Wood },
        {CSBlockType.SpruceWoodPlanks, Material_Wood },
        {CSBlockType.BirchWoodPlanks, Material_Wood },
        {CSBlockType.JungleWoodPlanks, Material_Wood },
        {CSBlockType.AcaciaWoodPlanks, Material_Wood },
        {CSBlockType.DarkOakWoodPlanks, Material_Wood },
        {CSBlockType.Bookshelf, Material_Wood },
        {CSBlockType.OakSlab, Material_Wood },
        {CSBlockType.SpruceSlab, Material_Wood },
        {CSBlockType.BirchSlab, Material_Wood },
        {CSBlockType.JungleSlab, Material_Wood },
        {CSBlockType.AcaciaSlab, Material_Wood },
        {CSBlockType.DarkOakSlab, Material_Wood },

        {CSBlockType.RedSand, Material_Sand },
        {CSBlockType.Sand, Material_Sand },
        
        {CSBlockType.Glass, Material_Glass },
        {CSBlockType.Ice, Material_Glass },
        {CSBlockType.PackedIce, Material_Glass },
    };

    static SoundManager instance;

    public static void Init()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/WwiseGlobal");
        GameObject obj = Instantiate(prefab);
        instance = obj.AddComponent<SoundManager>();

        AkBankManager.LoadBank("Main", false, false);
        SetMasterVolume();
    }

    public static void SetMasterVolume()
    {
        AkSoundEngine.SetRTPCValue("MainVolume", SettingsPanel.MasterVolume);
    }
    
    public static void PlayClickSound()
    {
        AkSoundEngine.PostEvent("UI_Click", Camera.main.gameObject);
    }

    public static void PlayPlaceSound(CSBlockType type, GameObject gameObject)
    {
        string material = Material_Grass;
        if (type2material.ContainsKey(type))
        {
            material = type2material[type];
        }
        AkSoundEngine.SetSwitch("Materials", material, gameObject);
        AkSoundEngine.PostEvent("Player_Place", gameObject);
    }

    public static void PlayBreakSound(CSBlockType type, GameObject gameObject)
    {
        string material = Material_Grass;
        if (type2material.ContainsKey(type))
        {
            material = type2material[type];
        }
        AkSoundEngine.SetSwitch("Materials", material, gameObject);
        AkSoundEngine.PostEvent("Player_Break", gameObject);
    }

    public static void PlayFootstepSound(CSBlockType type, GameObject gameObject)
    {
        string material = Material_Grass;
        if (type2material.ContainsKey(type))
        {
            material = type2material[type];
        }
        AkSoundEngine.SetSwitch("Materials", material, gameObject);
        AkSoundEngine.PostEvent("Player_Footstep", gameObject);
    }

    public static void PlayPopSound()
    {
        AkSoundEngine.PostEvent("Player_Pop", Camera.main.gameObject);
    }
}
