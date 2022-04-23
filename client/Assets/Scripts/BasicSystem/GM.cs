using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GM
{
    public static void Process(string text)
    {
        text = text.Substring(1);
        string[] gm_params = text.Split();

        switch (gm_params[0])
        {
            case "time":
                Time(gm_params);
                break;
            case "kill":
                Kill();
                break;
            case "zombie":
                Zombie();
                break;
            case "gamemode":
                GameMode(gm_params);
                break;
            case "ao":
                AO(gm_params);
                break;
            case "tp":
                TP(gm_params);
                break;
            case "give":
                Give(gm_params);
                break;
            case "chunkdebug":
                NBTHelper.DebugInfo();
                break;
            default:
                ChatPanel.AddLine(ChatPanel.ErrorCode + "Unknown command.");
                break;
        }
    }

    static void Give(string[] gm_params)
    {
        if (gm_params.Length == 1)
        {
            ChatPanel.AddLine(ChatPanel.ErrorCode + "Usage: /give id [count]");
            return;
        }
        string id = gm_params[1];
        int count = 1;
        if (gm_params.Length > 2)
        {
            if (int.TryParse(gm_params[2], out int num))
            {
                count = num;
            }
        }
        try
        {
            NBTObject generator = NBTGeneratorManager.GetObjectGenerator("minecraft:" + id);
            InventorySystem.Increment(generator, 0, (byte)count);
            ItemSelectPanel.instance.RefreshUI();
            ChatPanel.AddLine("Added " + count + " " + id + " to your inventory.");
        }
        catch
        {
            ChatPanel.AddLine(ChatPanel.ErrorCode + "No item id=" + id);
        }
    }

    static void TP(string[] gm_params)
    {
        if (gm_params.Length == 4)
        {
            float x = float.Parse(gm_params[1]);
            float y = float.Parse(gm_params[2]);
            float z = float.Parse(gm_params[3]);
            Vector3 dest = new Vector3(x, y, z);

            PlayerController.instance.SetPosition(dest);
        }
    }

    static void AO(string[] gm_params)
    {
        if (gm_params.Length == 2)
        {
            if (gm_params[1] == "1")
                Shader.EnableKeyword("DEBUG_AO");
            else if (gm_params[1] == "0")
                Shader.DisableKeyword("DEBUG_AO");
        }
    }

    static void GameMode(string[] gm_params)
    {
        if (gm_params.Length == 2)
        {
            if (gm_params[1] == "1" || gm_params[1] == "c" || gm_params[1] == "creative")
            {
                GameModeManager.SetCreative();
                ChatPanel.AddLine("Your game mode has been updated to <color=#AAAAAA>Creative Mode");
            }
            else if (gm_params[1] == "0" || gm_params[1] == "s" || gm_params[1] == "survival")
            {
                GameModeManager.SetSurvival();
                ChatPanel.AddLine("Your game mode has been updated to <color=#AAAAAA>Survival Mode");
            }
            else
            {
                ChatPanel.AddLine(ChatPanel.ErrorCode + '\'' + gm_params[2] + "\' is not a valid number");
            }
        }
        else
        {
            ChatPanel.AddLine(ChatPanel.ErrorCode + "Usage: /gamemode <mode>");
        }
    }

    static void Zombie()
    {
        Monster.CreateMonster(1, PlayerController.instance.transform.position);
    }

    static void Kill()
    {
        PlayerController.instance.Health = 0;
    }

    static void Time(string[] gm_params)
    {
        if (gm_params.Length == 3)
        {
            if (gm_params[1] == "set" || gm_params[1] == "add")
            {
                bool success = int.TryParse(gm_params[2], out int result);
                if (success)
                {
                    if (result >= 0)
                    {
                        TimeOfDay.instance.tick = result;
                        ChatPanel.AddLine("Set the time to " + result);
                    }
                    else
                    {
                        ChatPanel.AddLine(ChatPanel.ErrorCode + "The number you have entered (" + result + ") is too small, it must be at least 0");
                    }
                }
                else
                {
                    ChatPanel.AddLine(ChatPanel.ErrorCode + '\'' + gm_params[2] +"\' is not a valid number");
                }
            }
            else
            {
                ChatPanel.AddLine(ChatPanel.ErrorCode + "Usage: /time <set|add> <value>");
            }
        }
        else
        {
            ChatPanel.AddLine(ChatPanel.ErrorCode + "Usage: /time <set|add> <value>");
        }
    }
}
