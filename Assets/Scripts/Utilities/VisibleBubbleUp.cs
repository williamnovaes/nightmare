using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Nightmare
{
    public class VisibleBubbleUp : MonoBehaviour
    {
        public System.Action<VisibleBubbleUp> objectBecameVisible;

        private void OnBecameVisible()
        {
            objectBecameVisible(this);
        }
    }
}
