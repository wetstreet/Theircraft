using UnityEngine;

public enum ClientState
{
    NotConnected,
    InLobby,
    InRoom,
}

public static class DataCenter
{
    public static Vector3 initialPosition;
    public static string name;

    public static ClientState state;

    public static void RegisterCallbacks()
    {
    }


}