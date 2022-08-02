using System;
using UnityEngine;

namespace Modelling.UI
{
    public abstract class LoadingElement : MonoBehaviour
    {
        public abstract void Show(Action callback = null);
        public abstract void Hide(Action callback = null);
    }
}