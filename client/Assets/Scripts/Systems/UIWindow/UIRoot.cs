using System;
using System.Collections;
using System.Collections.Generic;
using EG;
using UnityEngine;

namespace EG
{
	public abstract class UIRoot : IEnumerator
	{
		private bool _isLoadAsset = false;

		/// <summary>
		/// 实例化对象
		/// </summary>
		public GameObject Go { private set; get; }

		/// <summary>
		/// 是否加载完毕
		/// </summary>
		public bool IsDone { get; private set; }

		/// <summary>
		/// 是否准备完毕
		/// </summary>
		public bool IsPrepare { get { return Go != null; } }

		/// <summary>
		/// UI桌面
		/// </summary>
		public abstract GameObject UIDesktop { protected set; get; }

		/// <summary>
		/// UI相机
		/// </summary>
		public abstract Camera UICamera { protected set; get; }


		internal void InternalLoad(string location)
		{
			if (_isLoadAsset)
				return;

			_isLoadAsset = true;
			 AssetManager.Instance.GetAsset(location,Handle_Completed);
		}
		internal void InternalDestroy()
		{
			if (Go != null)
			{
				GameObject.Destroy(Go);
				Go = null;
			}
			
		}
		private void Handle_Completed(string key,UnityEngine.Object obj)
		{
			if (obj == null)
				return;

			// 实例化对象
			Go = obj as GameObject;
			GameObject.DontDestroyOnLoad(Go);

			// 调用重载函数
			OnAssetLoad(Go);
			IsDone = true;
		}
		protected abstract void OnAssetLoad(GameObject go);

		#region 异步相关
		bool IEnumerator.MoveNext()
		{
			return !IsDone;
		}
		void IEnumerator.Reset()
		{
		}
		object IEnumerator.Current
		{
			get { return null; }
		}
		#endregion
	}
}