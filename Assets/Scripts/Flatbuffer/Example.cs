using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlatBuffers;
using MyData;
using System.IO;

public class TestFlatBuffers : MonoBehaviour {

    void Start () {
        FlatBufferBuilder fbb = new FlatBufferBuilder(1);
        StringOffset name = fbb.CreateString("zengtao");
        Worker.StartWorker(fbb);
        Worker.AddName(fbb, name);
        Worker.AddType(fbb, Type.MANAGER);
        Worker.AddInfo(fbb, Data.CreateData(fbb, 28));
        var wo = Worker.EndWorker(fbb);
        Worker.FinishWorkerBuffer(fbb,wo);

        File.WriteAllBytes(Application.dataPath + "worker.mon", fbb.DataBuffer.ToFullArray());

        var result = Worker.GetRootAsWorker(fbb.DataBuffer);
        Debug.LogFormat("info:name:{0},type:{1}", result.Name, result.Info.Value.Age);
    }
}
