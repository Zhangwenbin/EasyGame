using System;
using System.Collections;
using System.Collections.Generic;
using MotionFramework.Config;
using UnityEngine;
using Newtonsoft.Json;

public class DataPlayer : DataBase
{
    private const string STORAGE_KEY = "STORAGE_DATA_PLAYER";
    
    public override void OnCreate()
    {

    }
    public override void OnDestroy()
    {
    }

    protected override string GetStorageKey()
    {
        return STORAGE_KEY;
    }

    public static DataPlayer ReadFromLocal()
    {
        DataPlayer instance;
        var jsonStr = PlayerPrefs.GetString(DataPlayer.STORAGE_KEY, "");
        if (!string.IsNullOrEmpty(jsonStr))
        {
            try
            {
                instance = Newtonsoft.Json.JsonConvert.DeserializeObject<DataPlayer>(jsonStr);
            }
            catch (Exception e)
            {
                instance = new DataPlayer();
            }
        }
        else
        {
            instance = new DataPlayer();
        }

        return instance;
    }

    
}