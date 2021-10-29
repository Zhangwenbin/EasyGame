using Ludiq;
using System;
using System.Collections;
using EG;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Bolt
{
    /// <summary>Delays flow by waiting while a condition is true.</summary>
    [UnitTitle("Custom WaitForFrameBolt")]
    [UnitShortTitle("WaitForFrameBolt")]
    [UnitOrder(3)]
    public class WaitForFrameBolt : WaitUnit
    {
        public ValueInput key { get; private set; }
        
        /// <summary>The sum of A and B.</summary>
        [DoNotSerialize]
        [PortLabel("obj")]
        public ValueOutput obj { get; private set; }

        protected override void Definition()
        {
            base.Definition();
            this.key = this.ValueInput<int>("key");
            this.Requirement(this.key, this.enter);
        }

        protected override IEnumerator Await(Flow flow)
        {
            WaitForFrameBolt waitWhileUnit = this;
            
            yield return (object) waitWhileUnit.exit;
        }
    }
}