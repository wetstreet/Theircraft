using UnityEngine;

public enum ClientState
{
    NotConnected,
    InLobby,
    InRoom,
}

public static class DataCenter
{
    public static uint playerID;
    public static string name;
    public static Vector3 spawnPosition;
    public static Vector3 spawnRotation;
    public static int renderDistance = 3;

    public static ClientState state;

    public static void RegisterCallbacks()
    {
    }


}