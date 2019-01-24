/*
* @auth:Roshan
* @date:2018年11月27日20:34:55
* @func:读取配置表管理
*/

using System;
using System.Collections.Generic;
using System.IO;
using FlatBuffers;
using UnityEngine;

public class DataConfigMgr
{
    private static readonly string _fbBinPath = Environment.CurrentDirectory + @"\DataConfig\fb_bin\";

    private static Dictionary<string, ByteBuffer> _fbCacheDic = new Dictionary<string, ByteBuffer>();

    /**
     * 根据文件名读取ByteBuffer，再根据bb初始化要读取的IFlatBufferObject对象，比较麻烦
     * binfileName:二进制文件路径
     */
    public static ByteBuffer ReadBinData(string binFileName)
    {
        if (string.IsNullOrEmpty(binFileName))
        {
            return null;
        }

        var data = File.ReadAllBytes(string.Format("{0}{1}.bin", _fbBinPath, binFileName));
        if (data.Length <= 0)
        {
            return null;
        }

        //todo:flatbuffer足够快则不需要缓存
        if (!_fbCacheDic.ContainsKey(binFileName))
        {
            ByteBuffer bb = new ByteBuffer(data);
            _fbCacheDic.Add(binFileName, bb);
            return bb;
        }
        else
        {
            return _fbCacheDic[binFileName];
        }
    }
}