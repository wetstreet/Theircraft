using System;
using System.Collections.Generic;
using protocol.cs_theircraft;

namespace ChatRoomServer
{
    public static class TerrainData
    {
        static FastNoise fn;
        static readonly int scale = 2;
        static readonly int maxHeight = 15;
        public static int GetHeight(int x, int z)
        {
            if (fn == null)
            {
                fn = new FastNoise();
                fn.SetNoiseType(FastNoise.NoiseType.Perlin);
            }
            float noise = fn.GetNoise((x + 0.5f) * scale, (z + 0.5f) * scale);
            int height = (int)Math.Round(maxHeight * noise);
            return height;
        }

        //具有该chunk视野信息的player列表（一个player有多个chunk视野）
        //添加/破坏方块时用，同步给列表内的所有玩家
        static Dictionary<Vector2Int, List<Player>> chunkViewPlayersDict = new Dictionary<Vector2Int, List<Player>>();
        //处于chunk内的玩家列表（登陆拉取玩家列表/同步玩家移动时用，拿到视野内的不重复的玩家列表）
        static Dictionary<Vector2Int, List<Player>> chunkPlayersDict = new Dictionary<Vector2Int, List<Player>>();
        //chunk对应的方块列表
        static Dictionary<Vector2Int, List<CSBlock>> chunkMap = new Dictionary<Vector2Int, List<CSBlock>>();

        public static List<Player> GetChunkViewPlayers(Vector2Int chunk)
        {
            if (!chunkViewPlayersDict.ContainsKey(chunk))
                chunkViewPlayersDict[chunk] = new List<Player>();
            return chunkViewPlayersDict[chunk];
        }

        public static List<Player> GetChunkPlayers(Vector2Int chunk)
        {
            if (!chunkPlayersDict.ContainsKey(chunk))
                chunkPlayersDict[chunk] = new List<Player>();
            return chunkPlayersDict[chunk];
        }

        public static List<CSBlock> GetChunkBlocks(Vector2Int chunk)
        {
            if (!chunkMap.ContainsKey(chunk))
            {
                bool b = Redis.GetChunkData(chunk, out List<CSBlock> blockList);
                //Ultilities.Print($"chunk({chunk.x},{chunk.y}), generated = {b}");
                if (b)
                {
                    chunkMap[chunk] = blockList;
                }
                else
                {
                    blockList = new List<CSBlock>();
                    for (int i = 0; i < 16; i++)
                    {
                        for (int j = 0; j < 16; j++)
                        {
                            int x = i + chunk.x * 16;
                            int z = j + chunk.y * 16;
                            int maxHeight = GetHeight(i + chunk.x * 16, j + chunk.y * 16) + 20;
                            for (int k = maxHeight; k >= 0; k--)
                            {
                                CSBlock item = new CSBlock
                                {
                                    position = new CSVector3Int()
                                };
                                item.position.x = x;
                                item.position.y = k;
                                item.position.z = z;
                                //Ultilities.Print($"i={i},j={j},height{item.position.y}");
                                if (k == maxHeight)
                                    item.type = CSBlockType.Grass;
                                else
                                    item.type = CSBlockType.Dirt;
                                blockList.Add(item);
                            }
                        }
                    }
                    Redis.SetChunkData(chunk, blockList);
                    chunkMap[chunk] = blockList;
                }
            }
            return chunkMap[chunk];
        }

        public static bool RemoveBlockInChunk(Vector2Int chunk, CSVector3Int blockPosition)
        {
            List<CSBlock> chunkBlocks = GetChunkBlocks(chunk);
            bool deleted = false;
            //若存在则删除
            foreach (CSBlock block in chunkBlocks)
            {
                if (block.position.x == blockPosition.x &&
                    block.position.y == blockPosition.y &&
                    block.position.z == blockPosition.z)
                {
                    deleted = true;
                    chunkBlocks.Remove(block);
                    Redis.SetChunkData(chunk, chunkBlocks);
                    break;
                }
            }
            return deleted;
        }

        public static bool AddBlockInChunk(Vector2Int chunk, CSBlock block)
        {
            List<CSBlock> chunkBlocks = GetChunkBlocks(chunk);
            //检查是否已存在
            bool addSuccess = false;
            bool isExist = false;
            foreach (CSBlock b in chunkBlocks)
            {
                if (b.position.x == block.position.x &&
                    b.position.y == block.position.y &&
                    b.position.z == block.position.z)
                {
                    isExist = true;
                    break;
                }
            }
            //添加到list中
            if (!isExist)
            {
                chunkBlocks.Add(block);
                Redis.SetChunkData(chunk, chunkBlocks);
                addSuccess = true;
            }
            return addSuccess;
        }
    }
}
