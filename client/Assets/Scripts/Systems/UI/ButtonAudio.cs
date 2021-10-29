using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class ButtonAudio : MonoBehaviour
{
    private Button _button;
    private StudioEventEmitter _emitter;
    
    // Start is called before the first frame update
    void Start()
    {
        _button = GetComponent<Button>();
        _emitter = GetComponent<StudioEventEmitter>();
        _button.onClick.AddListener(OnClick);
        // if (!FMODUnity.RuntimeManager.HasBankLoaded(_emitter.bank))
        // {
        //    var handle= Addressables.LoadAssetAsync<TextAsset>(_emitter.bank+".bytes");
        //        handle.Completed += (handle) =>
        //     {
        //         Debug.Log(333);
        //         FMODUnity.RuntimeManager.LoadBank(handle.Result);
        //     };
        // }
    }
    void OnClick()
    {
        // if (!FMODUnity.RuntimeManager.HasBankLoaded(_emitter.bank))
        // {
        //     var handle= Addressables.LoadAssetAsync<TextAsset>(_emitter.bank+".bytes");
        //     handle.Completed += (handle) =>
        //     {
        //         Debug.Log(333);
        //         FMODUnity.RuntimeManager.LoadBank(handle.Result);
        //         _emitter.Play();
        //     };
        // }
        // else
        {
            _emitter.Play();
        }
       
    }
}
