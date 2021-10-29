using Ludiq;
using System;
using System.Collections;
using EG;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Bolt
{
    [UnitTitle("Custom LoadAsset")]
    [UnitShortTitle("LoadAsset")]
    [UnitOrder(1)]
    [UnitCategory("Custom")]
    public class LoadAssetBolt : Unit
    {
        public ValueInput assetName { get; private set; }
        
        /// <summary>The sum of A and B.</summary>
        [DoNotSerialize]
        [PortLabel("gameobject")]
        public ValueOutput obj { get; private set; }

        private GameObject res;
        
        /// <summary>The moment at which to start the delay.</summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput enter { get; private set; }

        /// <summary>The action to execute after the delay has elapsed.</summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput exit { get; private set; }

        protected override void Definition()
        {
            this.enter = this.ControlInputCoroutine("enter", new Func<Flow, IEnumerator>(this.Await));
            this.exit = this.ControlOutput("exit");
            this.Succession(this.enter, this.exit);
            
            this.assetName = this.ValueInput<string>("assetName");
            this.obj = this.ValueOutput<Object>("gameobject", new Func<Flow, Object>((flow) =>
            {
                return res;
            }));
            this.Requirement(this.assetName, this.enter);
        }

        protected  IEnumerator Await(Flow flow)
        {
            LoadAssetBolt waitWhileUnit = this;
            var asset_key= flow.GetValue<string>(this.assetName);
            var req= AssetManager.Instance.LoadAssetAsyncQueue(asset_key);
            yield return req;
            res =req.IsValid?(GameObject)req.Result:null;
            yield return (object) waitWhileUnit.exit;
        }
    }
}