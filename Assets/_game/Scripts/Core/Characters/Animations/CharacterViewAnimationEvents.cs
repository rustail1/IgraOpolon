using System;
using UnityEngine;

namespace Game
{
    public class CharacterViewAnimationEvents : MonoBehaviour
    {
        public event Action OnPunchEnded;

        public event Action OnPunchHit;

        public void NotifyPunchEnded()
        {
            OnPunchEnded?.Invoke();
        }

        public void NotifyPunchHit()
        {
            OnPunchHit?.Invoke();
        }
    }
}
