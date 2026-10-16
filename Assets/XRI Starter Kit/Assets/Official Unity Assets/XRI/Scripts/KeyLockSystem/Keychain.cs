using System.Collections.Generic;
using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    [DisallowMultipleComponent]
    public class Keychain : MonoBehaviour, IKeychain
    {
        [SerializeField]
        List<Key> m_Keys = new List<Key>();

        public void AddKey(Key key)
        {
            if (key == null || Contains(key))
                return;

            m_Keys.Add(key);
        }

        public void RemoveKey(Key key)
        {
            if (key != null)
                m_Keys.Remove(key);
        }

        public bool Contains(Key key)
        {
            return key != null && m_Keys.Contains(key);
        }
    }
}