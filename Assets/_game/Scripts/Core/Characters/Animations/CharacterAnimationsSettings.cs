using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class CharacterAnimationsSettings
    {
        [SerializeField] 
        private AnimationClip _attackClip;
                
        [field: SerializeField] 
        public string SpeedAnimationParameter { get; private set; }  = "Speed";
        
        [field: SerializeField] 
        public string AttackAnimationTrigger { get; private set; } = "Attack";
        
        [field: SerializeField]
        public string DyingAnimationTrigger { get; private set; } = "Dying";

        private float _attackClipLength;

        public float AttackClipLength
        {
            get
            {
                if (_attackClipLength > 0)
                {
                    return _attackClipLength;
                }
                _attackClipLength = _attackClip.length;
                return _attackClipLength;
            }
        }
    }
}