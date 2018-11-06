using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameEntry : MonoBehaviour {
    
	void Start () {
        TerrainGenerator.Init();
        List<Vector2Int> preloadChunks = Ultiities.GetSurroudingChunks(Vector2Int.zero);
        TerrainGenerator.ShowChunks(preloadChunks, true);
        //load chunks before create player character, to make sure player character don't spawn under the ground.
        PlayerController.Init();
    }

    Vector2Int lastChunk;
    void ChunkChecker()
    {
        Vector3 pos = PlayerController.Instance.transform.localPosition;
        Vector2Int chunk = Ultiities.GetChunk(pos);

        if (lastChunk != chunk)
        {
            List<Vector2Int> lastSurroudingChunks = Ultiities.GetSurroudingChunks(lastChunk);
            List<Vector2Int> surroudingChunks = Ultiities.GetSurroudingChunks(chunk);
            List<Vector2Int> loadChunks = surroudingChunks.Except(lastSurroudingChunks).ToList();
            List<Vector2Int> unloadChunks = lastSurroudingChunks.Except(surroudingChunks).ToList();
            TerrainGenerator.ShowChunks(loadChunks);
            TerrainGenerator.HideChunks(unloadChunks);
        }

        lastChunk = chunk;
    }

    void Update ()
    {
        ChunkChecker();
    }
}
