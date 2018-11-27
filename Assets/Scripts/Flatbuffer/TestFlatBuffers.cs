using UnityEngine;
using FlatBuffers;
using System.IO;

public class TestFlatBuffers : MonoBehaviour
{
    void Start()
    {
//        FlatBufferBuilder fbb = new FlatBufferBuilder(1);
//        StringOffset name = fbb.CreateString("zengtao");
//        Worker.StartWorker(fbb);
//        Worker.AddName(fbb, name);
//        Worker.AddType(fbb, Type.MANAGER);
//        Worker.AddInfo(fbb, Data.CreateData(fbb, 28));
//        var wo = Worker.EndWorker(fbb);
//        Worker.FinishWorkerBuffer(fbb, wo);
//
//        File.WriteAllBytes(Application.dataPath + "worker.mon", fbb.DataBuffer.ToFullArray());
//
//        var result = Worker.GetRootAsWorker(fbb.DataBuffer);
//        Debug.LogFormat("info:name:{0},type:{1}", result.Name, result.Info.Value.Age);

        //读取bin文件
//        var data = File.ReadAllBytes(@"E:\GitProject\Theircraft-unity\DataConfig\fb_bin\test.bin");
//        ByteBuffer bb = new ByteBuffer(data);
//        var r = Test.GetRootAsTest(bb);
//        for (int i = 0; i < r.ListLength; i++)
//        {
//            Debug.Log("--------------------------------");
//            Test_Item item = (Test_Item) r.List(i);
//            Debug.Log("item.ID = " + item.ID);
//            Debug.Log("item.Name = " + item.Name);
//            Debug.Log("item.Money = " + item.Money);
//            Debug.Log("item.Desc = " + item.Desc);
//            Debug.Log("--------------------------------");
//        }
        
        //测试dataConfigMgr
        var bb = DataConfigMgr.ReadBinData("Test");
        Test t = Test.GetRootAsTest(bb);
        Test_Item item = (Test_Item) t.List(0);
        Debug.Log("++++>>>"+item.Name);
        
    }
}