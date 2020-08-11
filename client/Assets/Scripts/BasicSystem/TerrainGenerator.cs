using protocol.cs_theircraft;
using UnityEngine;
using Game;

public class TerrainGenerator : MonoBehaviour
{
    static readonly float scale = 35;
    static readonly int maxHeight = 15;

    public static byte[] GenerateChunkData(CSVector2Int chunk, byte[] blocks)
    {
        Random.InitState(chunk.x * 1000 + chunk.y);
        if(pr == null)
        {
            pr = new Perlin(DateTime.Now.ToString());
        }
        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                float x = 0.5f + i + chunk.x * 16;
                float z = 0.5f + j + chunk.y * 16;
                float noise = (GetHight(x / scale, z / scale) + 1) * maxHeight / 2;
                int height = Mathf.RoundToInt(maxHeight * noise);
                for (int k = height; k >= 0; k--)
                {
                    CSBlockType type = CSBlockType.None;
                    if (k == 0)
                    {
                        type = CSBlockType.BedRock;
                    }
                    else
                    {
                        int distanceFromHighestBlock = height - k;
                        switch (distanceFromHighestBlock)
                        {
                            case 0:
                                //random surface block
                                int dice = Random.Range(1, 200);
                                if (dice <= 20)
                                {
                                    int plantdice = Random.Range(1, 5);
                                    switch (plantdice)
                                    {
                                        case 1:
                                        case 2:
                                            type = CSBlockType.Grass;
                                            break;
                                        case 3:
                                            type = CSBlockType.Poppy;
                                            break;
                                        case 4:
                                            type = CSBlockType.Dandelion;
                                            break;
                                    }
                                }
                                else if (dice <= 199 && dice > 197)
                                {
                                    int treedice = Random.Range(1, 10);
                                    if (treedice == 1)
                                    {
                                        GenerateTree(blocks, i, k, j, chunk.ToVector2Int());
                                    }
                                }
                                break;
                            case 1:
                                type = CSBlockType.GrassBlock;
                                break;
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                                type = CSBlockType.Dirt;
                                break;
                            default:
                                type = CSBlockType.Stone;
                                break;
                        }
                    }
                    if (type != CSBlockType.None)
                    {
                        blocks[256 * k + 16 * i + j] = (byte)type;
                    }
                }
            }
        }
        return blocks;
    }

    static void GenerateTree(byte[] blocks, int x, int y, int z, Vector2Int chunk)
    {
        for (int k = y + 2; k <= y + 4; k++)
        {
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = z - 1; j <= z + 1; j++)
                {
                    LocalServer.SetBlockType(i + chunk.x * 16, k, j + chunk.y * 16, CSBlockType.OakLeaves);
                }
            }
        }
        for (int i = 0; i < 3; i++)
        {
            LocalServer.SetBlockType(x + chunk.x * 16, y + i, z + chunk.y * 16, CSBlockType.OakLog);
        }
    }


    public int GetHight(int x, int z)
    {
        float h1 = pr.perlin((x - scale * 8) / (48 * (float)Math.PI), (z - scale * 8) / (48 * (float)Math.PI));
        float h2 = pr.perlin((x - scale * 12) / (24 * (float)Math.E), (z - scale * 4) / (24 * (float)Math.E)) / 1.5f;
        float h3 = pr.perlin((x - scale * 4) / 32f, (z - size.y * 12) / 32f) / 3;
        return (int)((h1 + h2 + h3) * maxHeight / 4) + maxHeight / 2;
    }
    static Perlin pr;
}

namespace Game
{
    public class Random
    {
        public Random(string seed)
        {
            try
            {
                this.seed = Convert.ToInt64(seed);
            }
            catch
            {
                char[] c = seed.ToCharArray();
                for(long i = 0; i < c.Length; i++)
                {
                    this.seed += c[i] * (long)Math.Pow(0X10000, i & 0x3);
                }
            }
            pram = new byte[512];
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                pram[i] = (byte)i;
            }
            for (int i = 0; i <= byte.MaxValue; i++)
            {
                pram[i + 256] = (byte)i;
            }
            int r = (int)(this.seed & 0x1249249249249249) * Math.Sign(this.seed);
            int a = (int)(this.seed & 0x2492492492492492) * Math.Sign(this.seed);
            int b = (int)(this.seed & 0x4924924924924924) * Math.Sign(this.seed);
            for (int i = 0; i < 512; i++)
            {
                r = (r * a + b) % 512;
                byte chac = pram[i];
                pram[i] = pram[r];
                pram[r] = chac;
            }
        }
        public long Seed
        {
            get
            {
                return seed;
            }
        }
        public int NextFloat()
        {
            int result = 0;
            long t = pos;
            if (pos < byte.MaxValue) pos++;
            else pos = 0;
            result = pram[t % 512];
            return result;
        }
        public int PosOf(long pos)
        {
            pos %= 512;
            long t = pos;
            if (t < 0)
            {
                t += 512;
            }
            return pram[t % 512];
        }
        private byte pos = 0;
        private byte[] pram;
        private long seed;
    }
    public class Perlin
    {
        public static double Fade(double t)
        {
            return t * t * t * (10 + t * (t * 6 - 15));
        }


        public static double grad(int hash, double x, double y)
        {
            switch (Mathf.Abs(hash % 5))
            {
                case 0: return x + y;
                case 1: return -x + y;
                case 2: return x - y;
                case 3: return -x - y;
                case 4: return y + x;
                case 5: return y - x;
                default: return 0; // never happens
            }
        }
        public Perlin(string seed)
        {
            random = new Random(seed);
        }
        public float perlin(float x,float y)
        {
            x %= 512;
            y %= 512;
            int xi = (int)Math.Floor(x);
            int yi = (int)Math.Floor(y);
            double xf = x - xi;
            double yf = y - yi;
            float u = (float)Fade(xf);
            float v = (float)Fade(yf);
            int aa, ab, ba, bb;
            aa = random.PosOf(random.PosOf(xi) + yi);
            ab = random.PosOf(random.PosOf(xi) + yi + 1);
            ba = random.PosOf(random.PosOf(xi + 1) + yi);
            bb = random.PosOf(random.PosOf(xi + 1) + yi + 1);
            float x1, x2;
            x1 = Mathf.Lerp((float)grad(aa, xf, yf), (float)grad(ba, 1 - xf, yf), u);
            x2 = Mathf.Lerp((float)grad(ab, xf, 1 - yf), (float)grad(bb, 1 - xf, 1 - yf), u);
            return Mathf.Lerp(x1, x2, v) / 2;
        }
        public readonly Random random;
    }
}
