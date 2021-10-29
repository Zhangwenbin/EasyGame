using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace EG
{
	/// <summary>
	/// 音频管理器
	/// </summary>
	public sealed class FmodManager : MonoSingleton<FmodManager>
	{
		/// <summary>
		/// 音频源封装类
		/// </summary>
		private class AudioSourceWrapper
		{
			public GameObject Go { private set; get; }
			public StudioEventEmitter Source { private set; get; }
			public AudioSourceWrapper(string name, Transform emitter)
			{
				// Create an empty game object
				Go = new GameObject(name);
				Go.transform.position = emitter.position;
				Go.transform.parent = emitter;

				// Create the source
				Source = Go.RequireComponent<StudioEventEmitter>();
			}
		}
		
		
		private readonly Dictionary<EAudioLayer, AudioSourceWrapper> _audioSourceWrappers = new Dictionary<EAudioLayer, AudioSourceWrapper>(200);
		private GameObject _root;

		public override void Initialize()
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

			if (Settings.Instance.ImportType==ImportType.AssetBundle)
			{
				int count = 2;
				AssetManager.Instance.LoadAssetAsync("Master.strings.bytes").callback += (key,obj) =>
				{
					FMODUnity.RuntimeManager.LoadBank((TextAsset)obj);
					count--;
					if (count<=0)
					{
						m_Initialize = true;
						StudioEventEmitter.bankLoaderReady = true;
					}
				};
				AssetManager.Instance.LoadAssetAsync("Master.bytes").callback += (key,obj) =>
				{
					FMODUnity.RuntimeManager.LoadBank((TextAsset)obj);
					count--;
					if (count<=0)
					{
						m_Initialize = true;
						StudioEventEmitter.bankLoaderReady = true;
					}
				};
			}
			else
			{
				m_Initialize = true;
				StudioEventEmitter.bankLoaderReady = true;
			}
		
		}

		public EventInstance PlayAmbient(string _event)
		{
			return PlayInLayer(EAudioLayer.Ambient,_event,true);
		}
		public void StopAmbient()
		{
			StopInternal(EAudioLayer.Ambient);
		}
		
		public EventInstance PlayMusic(string _event)
		{
			return PlayInLayer(EAudioLayer.Music,_event,true);
		}
		public void StopMusic()
		{
			StopInternal(EAudioLayer.Music);
		}
	
		public EventInstance PlayVoice(string _event)
		{
			return	PlayInLayer(EAudioLayer.Voice,_event,true);
		}
		public void StopVoice()
		{
			StopInternal(EAudioLayer.Voice);
		}
		public EventInstance PlaySound(string _event)
		{
			return	PlayInLayer(EAudioLayer.Sound,_event,true);
		}
		public void StopSound()
		{
			StopInternal(EAudioLayer.Sound);
		}

		private void StopInternal(EAudioLayer layer)
		{
			var emmiter = _audioSourceWrappers[layer].Source;
			emmiter.Stop();
			emmiter.EventDescription.unloadSampleData();
		}
		
		public EventInstance PlayInLayer(EAudioLayer layer, string _event, bool isLoop)
		{
			var emmiter = _audioSourceWrappers[layer].Source;
			if (string.IsNullOrEmpty(_event))
				return emmiter.EventInstance;
			if (emmiter.Event!=_event)
			{
				emmiter.Event = _event;
				emmiter.Stop();
				emmiter.Lookup();
			}
			if (layer == EAudioLayer.Music || layer == EAudioLayer.Ambient || layer == EAudioLayer.Voice)
			{
				emmiter.Play();
			}
			else if (layer == EAudioLayer.Sound)
			{
				emmiter.Play();
			}
			else
			{
				throw new NotImplementedException($"{layer}");
			}
			return emmiter.EventInstance;
		}


#if  UNITY_EDITOR
		public void EditorInitialize()
		{
			_root = new GameObject("[AudioManager]");
			foreach (int value in System.Enum.GetValues(typeof(EAudioLayer)))
			{
				EAudioLayer layer = (EAudioLayer)value;
				_audioSourceWrappers.Add(layer, new AudioSourceWrapper(layer.ToString(), _root.transform));
			}
			
			if (Settings.Instance.ImportType==ImportType.AssetBundle)
			{
				int count = 2;
				AssetManager.Instance.LoadAssetAsync("Master.strings.bytes").callback += (key,obj) =>
				{
					FMODUnity.RuntimeManager.LoadBank((TextAsset)obj);
					count--;
					if (count<=0)
					{
						m_Initialize = true;
						StudioEventEmitter.bankLoaderReady = true;
					}
				};
				AssetManager.Instance.LoadAssetAsync("Master.bytes").callback += (key,obj) =>
				{
					FMODUnity.RuntimeManager.LoadBank((TextAsset)obj);
					count--;
					if (count<=0)
					{
						m_Initialize = true;
						StudioEventEmitter.bankLoaderReady = true;
					}
				};
			}
			else
			{
				m_Initialize = true;
				StudioEventEmitter.bankLoaderReady = true;
			}
		}
#endif
		
	}
}