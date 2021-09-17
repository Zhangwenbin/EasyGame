using System;
using System.Collections;
using System.Collections.Generic;
using EG;
using UnityEngine;

public class DataManager : MonoSingleton<DataManager>
{
    private readonly List<DataBase> _allDatas = new List<DataBase>();

    private DataPlayer _player;
    private DataRuntime _runtime;

    public DataPlayer Player => _player;
    public DataRuntime Runtime => _runtime;

    public override void Initialize()
    {
        _player = new DataPlayer();
        _runtime = new DataRuntime();
        _allDatas.Add(_player);
        _allDatas.Add(_runtime);
        for (int i = 0; i < _allDatas.Count; i++)
        {
            _allDatas[i].OnCreate();
        }
        base.Initialize();
    }
    
    public void ClearAll()
    {
        for (int i = 0; i < _allDatas.Count; i++)
        {
            _allDatas[i].OnDestroy();
        }
        _allDatas.Clear();
    }

    public T GetData<T>() where T : DataBase
    {
        for (int i = 0; i < _allDatas.Count; i++)
        {
            if (_allDatas[i].GetType() == typeof(T))
            {
                return _allDatas[i] as T;
            }
        }
        return null;
    }


}