using System.Runtime.Serialization.Formatters.Binary;
using Theircraft;

public enum ClientState
{
    NotConnected,
    InLobby,
    InRoom,
}

public static class DataCenter
{

    public static string name;

    public static ClientState state;

    public static void RegisterCallbacks()
    {
    }


}