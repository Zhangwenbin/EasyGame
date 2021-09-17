﻿//--------------------------------------------------
// Motion Framework
// Copyright©2018-2020 何冠峰
// Licensed under the MIT license
//--------------------------------------------------
using System;
using System.Collections;
using EG;
using UnityEngine;

namespace MotionFramework.Window
{
	public abstract class UIWindow : IEnumerator
	{
		private System.Action<UIWindow> _prepareCallback;
		private bool _isLoadAsset = false;

		/// <summary>
		/// 是否已经创建
		/// </summary>
		protected bool IsCreate { private set; get; } = false;
		
		/// <summary>
		/// 窗口名称
		/// </summary>
		public string WindowName { private set; get; }

		/// <summary>
		/// 窗口层级
		/// </summary>
		public int WindowLayer { private set; get; }

		/// <summary>
		/// 是否是全屏窗口
		/// </summary>
		public bool FullScreen { private set; get; }

		/// <summary>
		/// 实例化对象
		/// </summary>
		public GameObject Go { private set; get; }

		/// <summary>
		/// 自定义数据
		/// </summary>
		public System.Object UserData { private set; get; }

		/// <summary>
		/// 是否加载完毕
		/// </summary>
		public bool IsDone { get; private set; }

		/// <summary>
		/// 是否准备完毕
		/// </summary>
		public bool IsPrepare { get { return Go != null; } }

		/// <summary>
		/// 窗口深度值
		/// </summary>
		public abstract int Depth { get; set; }

		/// <summary>
		/// 窗口可见性
		/// </summary>
		public abstract bool Visible { get; set; }


		public void Init(string name, int layer, bool fullScreen)
		{
			WindowName = name;
			WindowLayer = layer;
			FullScreen = fullScreen;
		}
		public abstract void OnCreate();
		public abstract void OnRefresh();
		public abstract void OnUpdate();
		public abstract void OnDestroy();

		internal void TryInvoke(System.Action<UIWindow> prepareCallback, System.Object userData)
		{
			UserData = userData;
			if (IsPrepare)
				prepareCallback?.Invoke(this);
			else
				_prepareCallback = prepareCallback;
		}
		internal void InternalLoad(string location, System.Action<UIWindow> prepareCallback, System.Object userData)
		{
			if (_isLoadAsset)
				return;

			UserData = userData;
			_isLoadAsset = true;
			_prepareCallback = prepareCallback;
			 AssetManager.Instance.GetAsset(location,Handle_Completed);
		}
		internal void InternalCreate()
		{
			if(IsCreate == false)
			{
				IsCreate = true;
				OnCreate();
			}
		}
		internal void InternalRefresh()
		{
			OnRefresh();
		}
		internal void InternalUpdate()
		{
			OnUpdate();
		}
		internal void InternalDestroy()
		{
			// 注销回调函数
			_prepareCallback = null;

			// 销毁面板对象
			IsCreate = false;
			if (Go != null)
			{
				OnDestroy();
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

			// 设置UI桌面
			if (WindowManager.Instance.Root == null)
				throw new Exception($"{nameof(UIRoot)} is not create. Use WindowManager.Instance.CreateUIRoot().");
			if (WindowManager.Instance.Root.IsPrepare == false)
				throw new Exception($"{nameof(UIRoot)} is not prepare.");
			GameObject uiDesktop = WindowManager.Instance.Root.UIDesktop;
			Go.transform.SetParent(uiDesktop.transform, false);

			// 扩展类的逻辑处理
			OnAssetLoad(Go);

			// 通知UI管理器
			_prepareCallback?.Invoke(this);
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