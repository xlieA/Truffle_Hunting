using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;



    [System.Serializable]
    public class TextureUpdateEvent : UnityEvent<Texture> { }

    public class TextureUpdater : MonoBehaviour {

        [SerializeField] protected TextureUpdateEvent textureUpdateEvent;

    }




