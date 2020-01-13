using System.Collections.Generic;
using System.Net.Sockets;
using protocol.cs_theircraft;

namespace ChatRoomServer
{
    //目前玩家的定义为从一进游戏还没登陆就算，后面会把玩家和登陆前的阶段分开。
    public class Player
    {
        //当前玩家个数
        public static uint id_index = 0;
        //玩家id
        public uint id;
        //玩家socket，还没登陆就有值（登陆流程要用到socket）
        public Socket socket;
        //玩家当前拥有的区块视野
        public List<Vector2Int> chunks = new List<Vector2Int>();
        //玩家当前所在区块
        public Vector2Int curChunk;
        //玩家名
        public string name;
        //是否已登录
        public bool inRoom;
        //玩家位置
        public Vector3 position;
        //玩家朝向
        public Vector3 rotation;

    }
}
