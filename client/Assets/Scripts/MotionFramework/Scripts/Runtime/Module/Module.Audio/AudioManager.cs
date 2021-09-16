﻿//--------------------------------------------------
// Motion Framework
// Copyright©2018-2020 何冠峰
// Licensed under the MIT license
//--------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using MotionFramework.Resource;
using MotionFramework.Console;

namespace MotionFramework.Audio
{
	/// <summary>
	/// 音频管理器
	/// </summary>
	public sealed class AudioManager : ModuleSingleton<AudioManager>, IModule
	{
		/// <summary>
		/// 音频源封装类
		/// </summary>
		private class AudioSourceWrapper
		{
			public GameObject Go { private set; get; }
			public AudioSource Source { private set; get; }
			public AudioSourceWrapper(string name, Transform emitter)
			{
				// Create an empty game object
				Go = new GameObject(name);
				Go.transform.position = emitter.position;
				Go.transform.parent = emitter;

				// Create the source
				Source = Go.AddComponent<AudioSource>();
				Source.volume = 1.0f;
				Source.pitch = 1.0f;
			}
		}

		private readonly Dictionary<string, AssetAudio> _assets = new Dictionary<string, AssetAudio>(500);
		private readonly Dictionary<EAudioLayer, AudioSourceWrapper> _audioSourceWrappers = new Dictionary<EAudioLayer, AudioSourceWrapper>(200);
		private GameObject _root;

		private Tweener musicTweener;


		void IModule.OnCreate(System.Object param)
		{
			_root = new GameObject("[AudioManager]");
			UnityEngine.Object.DontDestroyOnLoad(_root);

			foreach (int value in System.Enum.GetValues(typeof(EAudioLayer)))
			{
				EAudioLayer layer = (EAudioLayer)value;
				_audioSourceWrappers.Add(layer, new AudioSourceWrapper(layer.ToString(), _root.transform));
			}

#if LONGRIVER_OHAYOO
			OhayooSdkUtils.enableAudio += PauseMusic;
#endif
		}
		void IModule.OnUpdate()
		{
		}
		void IModule.OnGUI()
		{
			ConsoleGUI.Lable($"[{nameof(AudioManager)}] Audio total count : {_assets.Count}");
		}

		/// <summary>
		/// 获取音频源
		/// </summary>
		public AudioSource GetAudioSource(EAudioLayer layer)
		{
			return _audioSourceWrappers[layer].Source;
		}

		/// <summary>
		/// 预加载音频资源
		/// </summary>
		public void Preload(string location, EAudioLayer audioLayer)
		{
			if (_assets.ContainsKey(location) == false)
			{
				AssetAudio asset = new AssetAudio(location, audioLayer);
				_assets.Add(location, asset);
				asset.Load(null);
			}
		}

		/// <summary>
		/// 释放所有音频资源
		/// </summary>
		public void ReleaseAll()
		{
			foreach (KeyValuePair<string, AssetAudio> pair in _assets)
			{
				pair.Value.UnLoad();
			}
			_assets.Clear();
		}

		/// <summary>
		/// 释放指定层级的音频资源
		/// </summary>
		/// <param name="audioLayer">音频层级</param>
		public void Release(EAudioLayer audioLayer)
		{
			List<string> removeList = new List<string>();
			foreach (KeyValuePair<string, AssetAudio> pair in _assets)
			{
				if (pair.Value.AudioLayer == audioLayer)
					removeList.Add(pair.Key);
			}

			for (int i = 0; i < removeList.Count; i++)
			{
				string key = removeList[i];
				_assets[key].UnLoad();
				_assets.Remove(key);
			}
		}

		/// <summary>
		/// 播放背景音乐
		/// </summary>
		/// <param name="location">资源地址</param>
		/// <param name="loop">是否循环播放</param>
		public void PlayMusic(string location, bool loop, bool fade = true)
		{
			if (string.IsNullOrEmpty(location))
				return;
			if (!fade)
			{
				PlayAudioClip(EAudioLayer.Music, location, loop);
			}
			else
			{
				float fadeToVolume = 1f;
				float fadeOutDuration = 1;
				float fadeInDuration = 1;
				AudioSource musicAudioSource = GetAudioSource(EAudioLayer.Music);
				if (musicAudioSource == null || musicAudioSource.clip == null || musicAudioSource.volume < 0.1f)
				{
					musicAudioSource.volume = 0f;
					PlayAudioClip(EAudioLayer.Music, location, loop);
					musicTweener.Kill();
					musicTweener = DOTween.To(() => musicAudioSource.volume, value => musicAudioSource.volume = value,
						fadeToVolume, fadeInDuration);
				}
				else
				{
					musicTweener.Kill();
					musicTweener = DOTween.To(() => musicAudioSource.volume, value => musicAudioSource.volume = value, 0, fadeOutDuration)
						.OnComplete(() =>
						{
							PlayAudioClip(EAudioLayer.Music, location, loop);
							musicTweener = DOTween.To(() => musicAudioSource.volume, value => musicAudioSource.volume = value,
								fadeToVolume, fadeInDuration);
						});
				}
			}
		}

		/// <summary>
		/// 播放环境音效
		/// </summary>
		/// <param name="location">资源地址</param>
		/// <param name="loop">是否循环播放</param>
		public void PlayAmbient(string location, bool loop)
		{
			if (string.IsNullOrEmpty(location))
				return;

			PlayAudioClip(EAudioLayer.Ambient, location, loop);
		}

		/// <summary>
		/// 播放语音
		/// </summary>
		/// <param name="location">资源地址</param>
		public void PlayVoice(string location)
		{
			if (string.IsNullOrEmpty(location))
				return;

			// 如果静音状态直接跳过播放
			if (IsMute(EAudioLayer.Voice))
				return;

			PlayAudioClip(EAudioLayer.Voice, location, false);
		}

		/// <summary>
		/// 播放音效
		/// </summary>
		/// <param name="location">资源地址</param>
		public void PlaySound(string location)
		{
			if (string.IsNullOrEmpty(location))
				return;

			// 如果静音状态直接跳过播放
			if (IsMute(EAudioLayer.Sound))
				return;

			PlayAudioClip(EAudioLayer.Sound, location, false);
		}

		/// <summary>
		/// 使用外部音频源播放音效
		/// </summary>
		/// <param name="audioSource">外部的音频源</param>
		/// <param name="location">资源地址</param>
		public void PlaySound(AudioSource audioSource, string location)
		{
			if (audioSource == null) return;
			if (audioSource.isActiveAndEnabled == false) return;
			if (string.IsNullOrEmpty(location)) return;

			// 如果静音状态直接跳过播放
			if (IsMute(EAudioLayer.Sound))
				return;

			if (_assets.ContainsKey(location))
			{
				if (_assets[location].Clip != null)
					audioSource.PlayOneShot(_assets[location].Clip);
			}
			else
			{
				// 新建加载资源
				AssetAudio assetAudio = new AssetAudio(location, EAudioLayer.Sound);
				_assets.Add(location, assetAudio);
				assetAudio.Load((AudioClip clip) =>
				{
					if (clip != null)
					{
						if (audioSource != null) //注意：在加载过程中音频源可能被销毁，所以需要判空
							audioSource.PlayOneShot(_assets[location].Clip);
					}
				});
			}
		}

		private void PauseMusic(bool pause)
		{
			if (pause)
			{
				_audioSourceWrappers[EAudioLayer.Music].Source.Pause();
			}
			else
			{
				_audioSourceWrappers[EAudioLayer.Music].Source.UnPause();
			}
		}

		/// <summary>
		/// 暂停播放
		/// </summary>
		public void Stop(EAudioLayer layer)
		{
			_audioSourceWrappers[layer].Source.Stop();
		}

		public void StopMusic(bool fade = true)
		{
			if (!fade)
			{
				Stop(EAudioLayer.Music);
			}
			else
			{
				float fadeDuration = 1;
				var musicAudioSource = GetAudioSource(EAudioLayer.Music);
				musicTweener.Kill();
				musicTweener = DOTween.To(() => musicAudioSource.volume, value => musicAudioSource.volume = value, 0, fadeDuration)
					.OnComplete(() =>
					{
						Stop(EAudioLayer.Music);
					});
			}
		}

		public void FadeOutMusicVolume(int volPercent)
		{
			float fadeDuration = 1;
			var musicAudioSource = GetAudioSource(EAudioLayer.Music);
			musicTweener.Kill();
			musicTweener = DOTween.To(() => musicAudioSource.volume, value => musicAudioSource.volume = value, volPercent / 100f, fadeDuration)
				.OnComplete(() =>
				{
					Stop(EAudioLayer.Music);
				});
		}

		/// <summary>
		/// 设置所有频道静音
		/// </summary>
		public void Mute(bool isMute)
		{
			foreach (KeyValuePair<EAudioLayer, AudioSourceWrapper> pair in _audioSourceWrappers)
			{
				pair.Value.Source.mute = isMute;
			}
		}

		/// <summary>
		/// 设置频道静音
		/// </summary>
		public void Mute(EAudioLayer layer, bool isMute)
		{
			_audioSourceWrappers[layer].Source.mute = isMute;
		}

		/// <summary>
		/// 查询频道是否静音
		/// </summary>
		public bool IsMute(EAudioLayer layer)
		{
			return _audioSourceWrappers[layer].Source.mute;
		}

		/// <summary>
		/// 设置所有频道音量
		/// </summary>
		public void Volume(float volume)
		{
			foreach (KeyValuePair<EAudioLayer, AudioSourceWrapper> pair in _audioSourceWrappers)
			{
				pair.Value.Source.volume = volume;
			}
		}

		/// <summary>
		/// 设置频道音量
		/// </summary>
		public void Volume(EAudioLayer layer, float volume)
		{
			volume = Mathf.Clamp01(volume);
			_audioSourceWrappers[layer].Source.volume = volume;
		}

		/// <summary>
		/// 查询频道音量
		/// </summary>
		public float GetVolume(EAudioLayer layer)
		{
			return _audioSourceWrappers[layer].Source.volume;
		}

		private void PlayAudioClip(EAudioLayer layer, string location, bool isLoop)
		{
			if (_assets.ContainsKey(location))
			{
				if (_assets[location].Clip != null)
					PlayAudioClipInternal(layer, _assets[location].Clip, isLoop);
			}
			else
			{
				// 新建加载资源
				AssetAudio assetAudio = new AssetAudio(location, layer);
				_assets.Add(location, assetAudio);
				assetAudio.Load((AudioClip clip) =>
				{
					if (clip != null)
						PlayAudioClipInternal(layer, clip, isLoop);
				});
			}
		}
		private void PlayAudioClipInternal(EAudioLayer layer, AudioClip clip, bool isLoop)
		{
			if (clip == null)
				return;

			if (layer == EAudioLayer.Music || layer == EAudioLayer.Ambient || layer == EAudioLayer.Voice)
			{
				_audioSourceWrappers[layer].Source.clip = clip;
				_audioSourceWrappers[layer].Source.loop = isLoop;
				_audioSourceWrappers[layer].Source.Play();
			}
			else if (layer == EAudioLayer.Sound)
			{
				_audioSourceWrappers[layer].Source.PlayOneShot(clip);
			}
			else
			{
				throw new NotImplementedException($"{layer}");
			}
		}
	}
}