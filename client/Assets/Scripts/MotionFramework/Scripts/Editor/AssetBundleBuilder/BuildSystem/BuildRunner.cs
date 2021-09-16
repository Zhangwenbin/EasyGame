﻿//--------------------------------------------------
// Motion Framework
// Copyright©2021-2021 何冠峰
// Licensed under the MIT license
//--------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MotionFramework.Editor
{
	public class BuildRunner
	{
		/// <summary>
		/// 执行构建流程
		/// </summary>
		/// <returns>如果成功返回TRUE，否则返回FALSE</returns>
		public static bool Run(List<IBuildTask> pipeline, BuildContext context)
		{
			BuildLogger.Log($"Start Build");

			if (pipeline == null)
				throw new ArgumentNullException("pipeline");
			if (context == null)
				throw new ArgumentNullException("context");

			BuildLogger.Log($"Start Build 1");

			bool succeed = true;
			for (int i = 0; i < pipeline.Count; i++)
			{
				IBuildTask task = pipeline[i];
				try
				{
					task.Run(context);
				}
				catch (Exception e)
				{
					Debug.LogError($"Build task {task.GetType().Name} failed : {e}");
					succeed = false;
					break;
				}
			}

			// 返回运行结果
			return succeed;
		}
	}
}