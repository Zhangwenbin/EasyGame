﻿//--------------------------------------------------
// Motion Framework
// Copyright©2020-2020 何冠峰
// Licensed under the MIT license
//--------------------------------------------------

namespace MotionFramework.Editor
{
	/// <summary>
	/// 资源打包规则接口
	/// </summary>
	public interface IPackRule
	{
		/// <summary>
		/// 获取资源的打包标签
		/// </summary>
		string GetAssetBundleLabel(string assetPath);
	}
}