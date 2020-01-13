using System.Collections.Generic;
using System.IO;
using protocol.cs_theircraft;
using protocol.cs_enum;
using System;

namespace ChatRoomServer
{
    public static class GameLogic
    {
        static readonly Dictionary<uint, Player> players = new Dictionary<uint, Player>();

        static void AddPlayer(Player player, CSPlayer playerData)
        {
            player.id = playerData.PlayerID;
            player.name = playerData.Name;
            player.position = new Vector3 { x = playerData.Position.x, y = playerData.Position.y, z = playerData.Position.z };
            player.rotation = new Vector3 { x = playerData.Rotation.x, y = playerData.Rotation.y, z = playerData.Rotation.z };
            player.inRoom = true;
            player.curChunk = new Vector2Int { x = (int)Math.Floor(player.position.x / 16f), y = (int)Math.Floor(player.position.z / 16f) };
            TerrainData.GetChunkPlayers(player.curChunk).Add(player);
            players.Add(player.id, player);
            Ultilities.Print($"player {player.name}({player.socket.RemoteEndPoint}) has logged in!");
        }

        public static void RemovePlayer(Player player)
        {
            players.Remove(player.id);
            TerrainData.GetChunkPlayers(player.curChunk).Remove(player);
            foreach (Vector2Int chunk in player.chunks)
            {
                TerrainData.GetChunkViewPlayers(chunk).Remove(player);
            }
            MessageNotify("system", player.name + " left this room, current number of player : " + players.Count);
        }


        public static void OnRegisterReq(Player player, MemoryStream stream)
        {
            CSRegisterReq req = NetworkManager.Deserialize<CSRegisterReq>(stream);
            Ultilities.Print($"CSRegisterReq,account={req.Account},req.Name={req.Name},req.Password={req.Password}");
            bool hasRegistered = Redis.GetAccountData(req.Account, out AccountData accountData);

            //检测是否已注册
            if (hasRegistered)
            {
                CSRegisterRes res = new CSRegisterRes { RetCode = 8 };
                NetworkManager.Enqueue(player.socket, ENUM_CMD.CS_REGISTER_RES, res);
            }
            else
            {
                accountData = new AccountData
                {
                    playerID = Redis.GetPlayerIndexAdd(),
                    account = req.Account,
                    password = req.Password,
                    name = req.Name
                };
                Ultilities.Print($"SetAccountData,playerID={accountData.playerID},account={accountData.account},password={accountData.password},name={accountData.name}");
                Redis.SetAccountData(accountData.account, accountData);
                CSRegisterRes res = new CSRegisterRes { RetCode = 0 };
                NetworkManager.Enqueue(player.socket, ENUM_CMD.CS_REGISTER_RES, res);
            }
        }

        public static void OnLoginReq(Player player, MemoryStream stream)
        {
            CSLoginReq req = NetworkManager.Deserialize<CSLoginReq>(stream);
            Ultilities.Print($"CSLoginReq,account={req.Account}");

            //检测是否已注册
            bool bAccountExist = Redis.GetAccountData(req.Account, out AccountData accountData);
            //Ultilities.Print($"bAccountExist={bAccountExist}");
            if (!bAccountExist)
            {
                CSLoginRes res = new CSLoginRes { RetCode = 6 };
                NetworkManager.Enqueue(player.socket, ENUM_CMD.CS_LOGIN_RES, res);
            }
            else
            {
                //检测是否已登录，防止顶号
                bool hasLoggedIn = players.ContainsKey(accountData.playerID);
                //Ultilities.Print($"hasLoggedIn={hasLoggedIn}");
                if (hasLoggedIn)
                {
                    CSLoginRes res = new CSLoginRes { RetCode = 5 };
                    NetworkManager.Enqueue(player.socket, ENUM_CMD.CS_LOGIN_RES, res);
                }
                else
                {
                    //检查密码是否正确
                    bool bCorrect = req.Password.Equals(accountData.password);
                    //Ultilities.Print($"req.Password={req.Password},accountData.password={accountData.password},bCorrect={bCorrect}");
                    if (!bCorrect)
                    {
                        CSLoginRes res = new CSLoginRes { RetCode = 7 };
                        NetworkManager.Enqueue(player.socket, ENUM_CMD.CS_LOGIN_RES, res);
                    }
                    else
                    {
                        bool hasData = Redis.GetPlayerData(accountData.playerID, out CSPlayer playerData);
                        //Ultilities.Print($"hasData={hasData}");
                        if (!hasData)
                        {
                            //如果是第一次登陆，则写数据库
                            playerData = new CSPlayer
                            {
                                PlayerID = accountData.playerID,
                                Name = accountData.name,
                                Position = new CSVector3 { x = 0, y = 30, z = 0 },
                                Rotation = new CSVector3 { x = 0, y = 0, z = 0 }
                            };
                            Redis.SetPlayerData(playerData.PlayerID, playerData);
                        }

                        AddPlayer(player, playerData);
                        //Ultilities.Print($"playerData,playerID={accountData.playerID},account={accountData.account},password={accountData.password},name={accountData.name}");
                        //Ultilities.Print($"player,id={player.id},name={player.name}");

                        CSLoginRes res = new CSLoginRes
                        {
                            RetCode = 0,
                            PlayerData = playerData
                        };
                        NetworkManager.Enqueue(player.socket, ENUM_CMD.CS_LOGIN_RES, res);

                        MessageNotify("system", "player " + player.name + " has logged in, current number of player : " + players.Count);
                    }
                }
            }
        }

        public static void OnSendMessageReq(Player player, MemoryStream stream)
        {
            CSSendMessageReq req = NetworkManager.Deserialize<CSSendMessageReq>(stream);
            //Console.WriteLine($"OnSendMessageReq,content={req.Content}");

            int retCode = player.inRoom ? 0 : -1;
            if (player.inRoom)
            {
                Ultilities.Print(req.Content, player.name);
            }

            CSSendMessageRes res = new CSSendMessageRes
            {
                RetCode = retCode
            };
            NetworkManager.Enqueue(player.socket, ENUM_CMD.CS_SEND_MESSAGE_RES, res);

            MessageNotify(player.name, req.Content);
        }

        static void MessageNotify(string name, string content)
        {
            foreach (Player player in players.Values)
            {
                CSMessageNotify notify = new CSMessageNotify
                {
                    Name = name,
                    Content = content
                };
                NetworkManager.Enqueue(player.socket, ENUM_CMD.CS_MESSAGE_NOTIFY, notify);
            }
        }

        public static void OnHeroMoveReq(Player player, MemoryStream stream)
        {
            CSHeroMoveReq req = NetworkManager.Deserialize<CSHeroMoveReq>(stream);
            //Ultilities.Print($"CSHeroMoveReq,id={player.id},position=({req.Position.x},{req.Position.y},{req.Position.z}),rotation=({req.Rotation.x},{req.Rotation.y},{req.Rotation.z})");
            player.position = new Vector3(req.Position.x, req.Position.y, req.Position.z);
            player.rotation = new Vector3(req.Rotation.x, req.Rotation.y, req.Rotation.z);

            //写数据库
            Redis.SetPlayerData(player.id, new CSPlayer
            {
                PlayerID = player.id,
                Name = player.name,
                Position = new CSVector3 { x = player.position.x, y = player.position.y, z = player.position.z },
                Rotation = new CSVector3 { x = player.rotation.x, y = player.rotation.y, z = player.rotation.z },
            });

            //玩家的移动同步给看得到这个chunk的其他玩家
            List<Player> playersInChunk = TerrainData.GetChunkViewPlayers(player.curChunk);
            foreach (Player p in playersInChunk)
            {
                if (p.id != player.id)
                {
                    CSPlayerMoveNotify notify = new CSPlayerMoveNotify
                    {
                        PlayerID = player.id,
                        Position = req.Position,
                        Rotation = req.Rotation
                    };
                    NetworkManager.Enqueue(p.socket, ENUM_CMD.CS_PLAYER_MOVE_NOTIFY, notify);
                }
            }
        }
    }
}
