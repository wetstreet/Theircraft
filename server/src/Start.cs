using protocol.cs_enum;

namespace ChatRoomServer
{
    class MainClass
    {
        public static void Main()
        {
            Redis.Init();
            RegisterCallbacks();
            NetworkManager.Start();
        }

        static void RegisterCallbacks()
        {
            NetworkManager.Register(ENUM_CMD.CS_LOGIN_REQ, GameLogic.OnLoginReq);
            NetworkManager.Register(ENUM_CMD.CS_REGISTER_REQ, GameLogic.OnRegisterReq);
            NetworkManager.Register(ENUM_CMD.CS_SEND_MESSAGE_REQ, GameLogic.OnSendMessageReq);
            NetworkManager.Register(ENUM_CMD.CS_CHUNKS_ENTER_LEVAE_VIEW_REQ, TerrainGenerator.OnChunksEnterLeaveViewReq);
            NetworkManager.Register(ENUM_CMD.CS_ADD_BLOCK_REQ, TerrainGenerator.OnAddBlockReq);
            NetworkManager.Register(ENUM_CMD.CS_DELETE_BLOCK_REQ, TerrainGenerator.OnDeleteBlockReq);
            NetworkManager.Register(ENUM_CMD.CS_HERO_MOVE_REQ, GameLogic.OnHeroMoveReq);
        }
    }
}
