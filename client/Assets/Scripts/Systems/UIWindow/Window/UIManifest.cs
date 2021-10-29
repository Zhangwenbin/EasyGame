using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using EG;

namespace UnityEngine.UI
{
	[DisallowMultipleComponent]
	public class UIManifest : MonoBehaviour
	{
		public List<string> CacheAtlasTags = new List<string>();
		public SerializeValueBehaviour serializeValueBehaviour;

		public SerializeValueBehaviour SetupSerializeValueBehaviour()
		{
			serializeValueBehaviour = gameObject.RequireComponent<SerializeValueBehaviour>();
			return serializeValueBehaviour;
		}
		

		/// <summary>
		/// 根据全路径获取UI元素
		/// </summary>
		public GameObject GetUIElement(string path)
		{
			if (string.IsNullOrEmpty(path))
				return null;

			return serializeValueBehaviour.list.GetGameObject(path);
			
		}

		/// <summary>
		/// 根据全路径获取UI组件
		/// </summary>
		public Component GetUIComponent(string path, string typeName)
		{
			var element = GetUIElement(path);
			if (element == null)
				return null;

			Component component = element.GetComponent(typeName);
			if (component == null)
				Debug.LogWarning($"Not found ui component : {path}, {typeName}");
			return component;
		}

		/// <summary>
		/// 根据全路径获取UI组件
		/// </summary>
		public T GetUIComponent<T>(string path) where T : UnityEngine.Component
		{
			var element = GetUIElement(path);
			if (element == null)
				return null;

			Component component = element.GetComponent<T>();
			if (component == null)
				Debug.LogWarning($"Not found ui component : {path}, {typeof(T)}");
			return component as T;
		}
		
	}
}