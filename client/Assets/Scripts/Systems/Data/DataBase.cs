using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataBase
{
	public int ArchiveVersion = 1;

	protected bool _isDirty = false;

	public abstract void OnCreate();
	public abstract void OnDestroy();
	protected abstract string GetStorageKey();
	

	public virtual void OnUpdate()
	{
		if (_isDirty)
		{
			//TODO: Upload to CDN
			
			SaveLocal();
			_isDirty = false;
		}
	}
	
	public virtual void SetDirty()
	{
		_isDirty = true;
	}

	protected virtual void SaveLocal()
	{
		ArchiveVersion++;
		string jsonStr = ConvertToJson();
		PlayerPrefs.SetString(GetStorageKey(), jsonStr);
		// PlayerPrefs.Save();
	}

	protected virtual string ConvertToJson()
	{
		return Newtonsoft.Json.JsonConvert.SerializeObject(this);
	}
}