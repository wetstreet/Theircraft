using System.Collections.Generic;
using System.IO;
using protocol.cs_enum;
using protocol.cs_theircraft;

namespace ChatRoomServer
{
    public static class TerrainGenerator
    {
        static bool ProcessChunksEnterView(Player player, List<CSVector2Int> cschunks, out List<CSChunk> enterViewChunks)
        {
            bool retBool = true;
            enterViewChunks = new List<CSChunk>();

            //转换
            List<Vector2Int> chunks = Vector2Int.CSVector2IntList_To_Vector2IntList(cschunks);

            //校验
            foreach (Vector2Int chunk in chunks)
            {
                foreach (Player p in TerrainData.GetChunkViewPlayers(chunk))
                {
                    if (p.id == player.id)
                    {
                        retBool = false;
                        break;
                    }
                }
                foreach (Vector2Int c in player.chunks)
                {
                    if (c.x == chunk.x && c.y == chunk.y)
                    {
                        retBool = false;
                        break;
                    }
                }
            }
            
            //若校验成功则操作
            if (retBool)
            {
                foreach (Vector2Int chunk in chunks)
                {
                    player.chunks.Add(chunk);
                    List<Player> playersInChunk = TerrainData.GetChunkPlayers(chunk);
                    TerrainData.GetChunkViewPlayers(chunk).Add(player);

                    List<byte> blocksInBytes = new List<byte>();
                    List<CSBlock> blocks = TerrainData.GetChunkBlocks(chunk);
                    Dictionary<Vector3Int, CSBlockType> pos2type = new Dictionary<Vector3Int, CSBlockType>();
                    foreach (CSBlock block in blocks)
                    {
                        Vector3Int blockPos = Vector3Int.ParseFromCSVector3Int(block.position);
                        pos2type[blockPos] = block.type;
                    }
                    Vector3Int tempPos = new Vector3Int();
                    for (int k = 0; k < 256; k++)
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            for (int j = 0; j < 16; j++)
                            {
                                tempPos.x = chunk.x * 16 + i;
                                tempPos.y = k;
                                tempPos.z = chunk.y * 16 + j;
                                CSBlockType type = pos2type.ContainsKey(tempPos) ? pos2type[tempPos] : CSBlockType.None;
                                blocksInBytes.Add((byte)type);
                            }
                        }
                    }
                    CSChunk c = new CSChunk();
                    c.Position = chunk.ToCSVector2Int();
                    c.BlocksInBytes = blocksInBytes.ToArray();
                    //Ultilities.Print($"id={player.id},chunk=({c.Position.x},{c.Position.y}),length={playersInChunk.Count}");
                    foreach (Player p in playersInChunk)
                    {
                        //Ultilities.Print($"this id={p.id}");
                        if (p.id != player.id)
                        {
                            //Ultilities.Print($"add");
                            c.Players.Add(new CSPlayer
                            {
                                PlayerID = p.id,
                                Name = p.name,
                                Position = new CSVector3 { x = p.position.x, y = p.position.y, z = p.position.z },
                                Rotation = new CSVector3 { x = p.rotation.x, y = p.rotation.y, z = p.rotation.z }
                            });
                        }
                    }
                    enterViewChunks.Add(c);
                }
            }

            return retBool;
        }

        static bool ProcessChunksLeaveView(Player player, List<CSVector2Int> cschunks, out List<Vector2Int> retChunks)
        {
            bool retBool = true;
            List<Vector2Int> retChunkList = new List<Vector2Int>();

            //转换
            List<Vector2Int> chunks = Vector2Int.CSVector2IntList_To_Vector2IntList(cschunks);

            //校验
            foreach (Vector2Int chunk in chunks)
            {
                if (!player.chunks.Contains(chunk))
                {
                    retBool = false;
                    break;
                }
                bool hasPlayer = false;
                foreach (Player p in TerrainData.GetChunkViewPlayers(chunk))
                {
                    if (p.id == player.id)
                    {
                        hasPlayer = true;
                        break;
                    }
                }
                if (!hasPlayer)
                    retBool = false;
            }

            //若校验成功则操作
            if (retBool)
            {
                foreach (Vector2Int chunk in chunks)
                {
                    player.chunks.Remove(chunk);
                    TerrainData.GetChunkViewPlayers(chunk).Remove(player);
                    
                    retChunkList.Add(chunk);
                }
            }

            retChunks = retChunkList;
            return retBool;
        }

        public static void OnChunksEnterLeaveViewReq(Player player, MemoryStream stream)
        {
            CSChunksEnterLeaveViewReq req = NetworkManager.Deserialize<CSChunksEnterLeaveViewReq>(stream);
            Ultilities.Print("CSChunksEnterLeaveViewReq," + req.EnterViewChunks.Count + "," + req.LeaveViewChunks.Count);

            bool leaveView = true;
            bool enterView = ProcessChunksEnterView(player, req.EnterViewChunks, out List<CSChunk> enterViewChunks);
            List<Vector2Int> leaveViewChunks = null;
            if (enterView)
            {
                if (req.LeaveViewChunks.Count > 0)
                {
                    leaveView = ProcessChunksLeaveView(player, req.LeaveViewChunks, out leaveViewChunks);
                }
            }

            if (enterView && leaveView)
            {
                CSChunksEnterLeaveViewRes res = new CSChunksEnterLeaveViewRes
                {
                    RetCode = 0
                };
                res.EnterViewChunks.AddRange(enterViewChunks);
                if (leaveViewChunks != null)
                {
                    res.LeaveViewChunks.AddRange(Vector2Int.Vector2IntList_To_CSVector2IntList(leaveViewChunks));
                }
                Ultilities.Print("CSChunksEnterLeaveViewRes," + res.EnterViewChunks.Count + "," + res.LeaveViewChunks.Count);
                NetworkManager.Enqueue(player.socket, ENUM_CMD.CS_CHUNKS_ENTER_LEAVE_VIEW_RES, res);
            }
            else
            {
                CSChunksEnterLeaveViewRes res = new CSChunksEnterLeaveViewRes
                {
                    RetCode = 4
                };
                Ultilities.Print("CSChunksEnterLeaveViewRes," + res.EnterViewChunks.Count + "," + res.LeaveViewChunks.Count);
                NetworkManager.Enqueue(player.socket, ENUM_CMD.CS_CHUNKS_ENTER_LEAVE_VIEW_RES, res);
            }
        }

        public static void OnAddBlockReq(Player player, MemoryStream stream)
        {
            Ultilities.Print("OnAddBlockReq");
            CSAddBlockReq req = NetworkManager.Deserialize<CSAddBlockReq>(stream);

            Vector2Int chunk = Ultilities.GetChunk(req.block.position);
            bool addSuccess = TerrainData.AddBlockInChunk(chunk, req.block);

            //回包
            CSAddBlockRes res = new CSAddBlockRes();
            if (addSuccess)
            {
                res.RetCode = 0;
                res.block = req.block;
            }
            else
            {
                res.RetCode = 2;
            }
            NetworkManager.Enqueue(player.socket, ENUM_CMD.CS_ADD_BLOCK_RES, res);
            if (addSuccess)
            {
                //同步给有该chunk视野的其他玩家
                foreach (Player p in TerrainData.GetChunkViewPlayers(chunk))
                {
                    if (p.id != player.id)
                        AddBlockNotify(p, req.block);
                }
            }
        }

        static void AddBlockNotify(Player player, CSBlock block)
        {
            CSAddBlockNotify notify = new CSAddBlockNotify
            {
                block = block
            };
            NetworkManager.Enqueue(player.socket, ENUM_CMD.CS_ADD_BLOCK_NOTIFY, notify);
        }

        public static void OnDeleteBlockReq(Player player, MemoryStream stream)
        {
            Ultilities.Print("OnDeleteBlockReq");
            CSDeleteBlockReq req = NetworkManager.Deserialize<CSDeleteBlockReq>(stream);

            Vector2Int chunk = Ultilities.GetChunk(req.position);
            bool deleted = TerrainData.RemoveBlockInChunk(chunk, req.position);

            CSDeleteBlockRes res = new CSDeleteBlockRes();
            if (deleted)
            {
                res.RetCode = 0;
                res.position = req.position;
            }
            else
            {
                res.RetCode = 3;
            }
            NetworkManager.Enqueue(player.socket, ENUM_CMD.CS_DELETE_BLOCK_RES, res);
            if (deleted)
            {
                //同步给有该chunk视野的其他玩家
                foreach (Player p in TerrainData.GetChunkViewPlayers(chunk))
                {
                    if (p.id != player.id)
                        DeleteBlockNotify(p, req.position);
                }
            }
        }

        static void DeleteBlockNotify(Player player, CSVector3Int position)
        {
            CSDeleteBlockNotify notify = new CSDeleteBlockNotify
            {
                position = position
            };
            NetworkManager.Enqueue(player.socket, ENUM_CMD.CS_DELETE_BLOCK_NOTIFY, notify);
        }
    }
}
