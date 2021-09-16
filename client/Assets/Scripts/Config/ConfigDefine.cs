using MotionFramework.IO;
using System.Collections.Generic;
using UnityEngine;

public class VectorI3
{
    public int x = 0;
    public int y = 0;
    public int z = 0;

    public static List<VectorI3> ParseList(ByteBuffer byteBuf)
    {
        List<VectorI3> v3s = new List<VectorI3>();
        string listStr = byteBuf.ReadUTF();
        string[] strs = listStr.Split(';');

        foreach (var str in strs)
        {
            v3s.Add(ParseStr(str));
        }

        return v3s;
    }

    public static VectorI3 Parse(ByteBuffer byteBuf)
    {
        string str = byteBuf.ReadUTF();
        return ParseStr(str);
    }

    protected static VectorI3 ParseStr(string str)
    {
        string[] sps = str.Split('_');
        VectorI3 v3 = new VectorI3();
        if (sps.Length > 0) int.TryParse(sps[0], out v3.x);
        if (sps.Length > 1) int.TryParse(sps[1], out v3.y);
        if (sps.Length > 2) int.TryParse(sps[2], out v3.z);
        return v3;
    }
}

public class VectorF3
{
    public float x = 0;
    public float y = 0;
    public float z = 0;

    public Vector3  ToVector3()
    {
        return new Vector3(x, y, z);
    }

    public static List<VectorF3> ParseList(ByteBuffer byteBuf)
    {
        List<VectorF3> v3s = new List<VectorF3>();
        string listStr = byteBuf.ReadUTF();
        string[] strs = listStr.Split(';');

        foreach (var str in strs)
        {
            v3s.Add(ParseStr(str));
        }

        return v3s;
    }

    public static VectorF3 Parse(ByteBuffer byteBuf)
    {
        string str = byteBuf.ReadUTF();
        return ParseStr(str);
    }

    protected static VectorF3 ParseStr(string str)
    {
        string[] sps = str.Split('_');
        VectorF3 v3 = new VectorF3();
        if (sps.Length > 0) float.TryParse(sps[0], out v3.x);
        if (sps.Length > 1) float.TryParse(sps[1], out v3.y);
        if (sps.Length > 2) float.TryParse(sps[2], out v3.z);
        return v3;
    }
}

