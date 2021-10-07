﻿using System.Collections;
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
            default:
                ChatPanel.AddLine(ChatPanel.ErrorCode + "Unknown command.");
                break;
        }

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
