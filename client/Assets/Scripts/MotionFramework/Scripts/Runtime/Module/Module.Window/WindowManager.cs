﻿//--------------------------------------------------
// Motion Framework
// Copyright©2018-2020 何冠峰
// Licensed under the MIT license
//--------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using MotionFramework.Console;

namespace MotionFramework.Window
{
	public class WindowManager : ModuleSingleton<WindowManager>, IModule
	{
		private readonly List<UIWindow> _stack = new List<UIWindow>(100);

		/// <summary>
		/// 反射服务接口
		/// </summary>
		public IActivatorServices ActivatorServices { get; set; }

		void IModule.OnCreate(object createParam)
		{
		}
		void IModule.OnUpdate()
		{
			int count = _stack.Count;
			for (int i = 0; i < _stack.Count; i++)
			{
				if (_stack.Count != count)
					break;
				UIWindow window = _stack[i];
				if (window.IsPrepare)
					window.InternalUpdate();
			}
		}
		void IModule.OnGUI()
		{
			ConsoleGUI.Lable($"[{nameof(WindowManager)}] Window total count : {_stack.Count}");
		}

		/// <summary>
		/// UI根节点
		/// </summary>
		public UIRoot Root { private set; get; }

		/// <summary>
		/// 是否有任意窗口正在加载
		/// </summary>
		public bool IsLoading()
		{
			for (int i = 0; i < _stack.Count; i++)
			{
				UIWindow window = _stack[i];
				if (window.IsDone == false)
					return true;
			}
			return false;
		}

		/// <summary>
		/// 检测窗口是否加载完毕
		/// </summary>
		public bool IsLoadDone<T>()
		{
			return IsLoadDone(typeof(T));
		}
		public bool IsLoadDone(Type type)
		{
			UIWindow window = GetWindow(type.FullName);
			if (window == null)
				return false;
			return window.IsDone;
		}

		/// <summary>
		/// 查询顶端窗口
		/// </summary>
		public bool IsTop<T>(int layer)
		{
			return IsTop(typeof(T), layer);
		}
		public bool IsTop(Type type, int layer)
		{
			UIWindow lastOne = null;
			for (int i = 0; i < _stack.Count; i++)
			{
				if (_stack[i].WindowLayer == layer)
					lastOne = _stack[i];
			}

			if (lastOne == null)
				return false;

			string windowName = type.FullName;
			return lastOne.WindowName == windowName;
		}

		/// <summary>
		/// 查询顶端窗口
		/// </summary>
		public bool IsTop<T>()
		{
			return IsTop(typeof(T));
		}
		public bool IsTop(Type type)
		{
			if (_stack.Count == 0)
				return false;

			string windowName = type.FullName;
			UIWindow lastOne = _stack[_stack.Count - 1];
			return lastOne.WindowName == windowName;
		}

		/// <summary>
		/// 查询层级窗口是否存在
		/// </summary>
		public bool HasWindow(int layer)
		{
			for (int i = 0; i < _stack.Count; i++)
			{
				if (_stack[i].WindowLayer == layer)
					return true;
			}
			return false;
		}

		/// <summary>
		/// 查询窗口是否存在
		/// </summary>
		public bool HasWindow<T>()
		{
			return HasWindow(typeof(T));
		}
		public bool HasWindow(Type type)
		{
			return IsContains(type.FullName);
		}

		public T GetWindow<T>() where T : CanvasWindow
		{
			return GetWindow(typeof(T).FullName) as T;
		}

		public UIWindow GetWindow(Type type)
		{
			return GetWindow(type.FullName);
		}

		/// <summary>
		/// 创建UIRoot
		/// </summary>
		public UIRoot CreateUIRoot<T>(string location) where T : UIRoot
		{
			return CreateUIRoot(typeof(T), location);
		}
		public UIRoot CreateUIRoot(Type type, string location)
		{
			if (Root != null)
				throw new Exception("UIRoot has been created.");

			Root = Activator.CreateInstance(type) as UIRoot;
			if (Root == null)
				throw new Exception($"UIRoot {type.FullName} create instance failed.");
			Root.InternalLoad(location);
			return Root;
		}

		/// <summary>
		/// 打开窗口
		/// </summary>
		/// <param name="location">资源路径</param>
		/// <param name="userData">用户数据</param>
		public UIWindow OpenWindow<T>(string location, System.Object userData = null) where T : UIWindow
		{
			return OpenWindow(typeof(T), location, userData);
		}
		public UIWindow OpenWindow(Type type, string location, System.Object userData = null)
		{
			string windowName = type.FullName;

			// 如果窗口已经存在
			if (IsContains(windowName))
			{
				UIWindow window = GetWindow(windowName);
				Pop(window); //弹出窗口
				Push(window); //重新压入
				window.TryInvoke(OnWindowPrepare, userData);
				return window;
			}
			else
			{
				UIWindow window = CreateInstance(type);
				Push(window); //首次压入
				window.InternalLoad(location, OnWindowPrepare, userData);
				return window;
			}
		}

		/// <summary>
		/// 关闭窗口
		/// </summary>
		public void CloseWindow<T>() where T : UIWindow
		{
			CloseWindow(typeof(T));
		}
		public void CloseWindow(Type type)
		{
			string windowName = type.FullName;
			UIWindow window = GetWindow(windowName);
			if (window == null)
				return;

			window.InternalDestroy();
			Pop(window);
			OnSortWindowDepth(window.WindowLayer);
			OnSetWindowVisible();
		}

		public void CloseWindow(string windowName)
		{
			UIWindow window = GetWindow(windowName);
			if (window == null)
				return;

			window.InternalDestroy();
			Pop(window);
			OnSortWindowDepth(window.WindowLayer);
			OnSetWindowVisible();
		}
		/// <summary>
		/// 关闭所有窗口
		/// </summary>
		public void CloseAll()
		{
			for (int i = 0; i < _stack.Count; i++)
			{
				UIWindow window = _stack[i];
				window.InternalDestroy();
			}
			_stack.Clear();
		}

		private void OnWindowPrepare(UIWindow window)
		{
			OnSortWindowDepth(window.WindowLayer);
			window.InternalCreate();
			window.InternalRefresh();
			OnSetWindowVisible();
		}
		private void OnSortWindowDepth(int layer)
		{
			int depth = layer;
			for (int i = 0; i < _stack.Count; i++)
			{
				if (_stack[i].WindowLayer == layer)
				{
					_stack[i].Depth = depth;
					depth += 100; //注意：每次递增100深度
				}
			}
		}
		private void OnSetWindowVisible()
		{
			bool isHideNext = false;
			for (int i = _stack.Count - 1; i >= 0; i--)
			{
				UIWindow window = _stack[i];
				if (isHideNext == false)
				{
					window.Visible = true;
					if (window.IsPrepare && window.FullScreen)
						isHideNext = true;
				}
				else
				{
					window.Visible = false;
				}
			}
		}

		private UIWindow CreateInstance(Type type)
		{
			UIWindow window;
			WindowAttribute attribute;

			if (ActivatorServices != null)
			{
				window = ActivatorServices.CreateInstance(type) as UIWindow;
				attribute = ActivatorServices.GetAttribute(type) as WindowAttribute;
			}
			else
			{
				window = Activator.CreateInstance(type) as UIWindow;
				attribute = Attribute.GetCustomAttribute(type, typeof(WindowAttribute)) as WindowAttribute;
			}

			if (window == null)
				throw new Exception($"Window {type.FullName} create instance failed.");
			if (attribute == null)
				throw new Exception($"Window {type.FullName} not found {nameof(WindowAttribute)} attribute.");

			window.Init(type.FullName, attribute.WindowLayer, attribute.FullScreen);
			return window;
		}
		public UIWindow GetWindow(string name)
		{
			for (int i = 0; i < _stack.Count; i++)
			{
				UIWindow window = _stack[i];
				if (window.WindowName == name)
					return window;
			}
			return null;
		}
		private bool IsContains(string name)
		{
			for (int i = 0; i < _stack.Count; i++)
			{
				UIWindow window = _stack[i];
				if (window.WindowName == name)
					return true;
			}
			return false;
		}
		private void Push(UIWindow window)
		{
			// 如果已经存在
			if (IsContains(window.WindowName))
				throw new System.Exception($"Window {window.WindowName} is exist.");

			// 获取插入到所属层级的位置
			int insertIndex = -1;
			for (int i = 0; i < _stack.Count; i++)
			{
				if (window.WindowLayer == _stack[i].WindowLayer)
					insertIndex = i + 1;
			}

			// 如果没有所属层级，找到相邻层级
			if (insertIndex == -1)
			{
				for (int i = 0; i < _stack.Count; i++)
				{
					if (window.WindowLayer > _stack[i].WindowLayer)
						insertIndex = i + 1;
				}
			}

			// 如果是空栈或没有找到插入位置
			if (insertIndex == -1)
			{
				insertIndex = 0;
			}

			// 最后插入到堆栈
			_stack.Insert(insertIndex, window);
		}
		private void Pop(UIWindow window)
		{
			// 从堆栈里移除
			_stack.Remove(window);
		}
	}
}