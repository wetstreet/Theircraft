using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameEntry : MonoBehaviour {

	// Use this for initialization
	void Start () {
        TerrainGenerator.Init();
        List<Vector2Int> preloadChunks = Ultiities.GetSurroudingChunks(Vector2Int.zero);
        TerrainGenerator.SyncGenerateChunks(preloadChunks);
        //load chunks before create player character, to make sure player character don't spawn under the ground.
        PlayerController.Init();
    }

    // Update is called once per frame
    Vector2Int lastChunk;
	void Update ()
    {
        Vector3 pos = PlayerController.Instance.transform.localPosition;
        Vector2Int chunk = Ultiities.GetChunk(pos);

        if (lastChunk != chunk)
        {
            List<Vector2Int> lastSurroudingChunks = Ultiities.GetSurroudingChunks(lastChunk);
            List<Vector2Int> surroudingChunks = Ultiities.GetSurroudingChunks(chunk);
            List<Vector2Int> loadChunks = surroudingChunks.Except(lastSurroudingChunks).ToList();
            List<Vector2Int> unloadChunks = lastSurroudingChunks.Except(surroudingChunks).ToList();
            TerrainGenerator.GenerateChunks(loadChunks);
            TerrainGenerator.HideChunks(unloadChunks);
        }

        lastChunk = chunk;
    }
}
