
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UAM
{
    public interface GameObjectAttachable
    {
        public GameObject gameObject { set; get; }
    }


    public interface IMonoBehaviour
    {
        GameObject gameObject { get; }
        Transform transform { get; }

        #region *** Component
        T GetComponent<T>();
        Component GetComponent(string type);
        Component GetComponentInChildren(System.Type t, bool includeInactive);
        Component GetComponentInChildren(System.Type t);
        T GetComponentInChildren<T>(bool includeInactive);
        T GetComponentInChildren<T>();
        Component GetComponentInParent(System.Type t);
        T GetComponentInParent<T>();
        Component[] GetComponents(System.Type type);
        void GetComponents(System.Type type, List<Component> results);
        void GetComponents<T>(List<T> results);
        T[] GetComponents<T>();
        Component[] GetComponentsInChildren(System.Type t);
        void GetComponentsInChildren<T>(List<T> results);
        T[] GetComponentsInChildren<T>();
        void GetComponentsInChildren<T>(bool includeInactive, List<T> result);
        T[] GetComponentsInChildren<T>(bool includeInactive);
        Component[] GetComponentsInChildren(System.Type t, bool includeInactive);
        T[] GetComponentsInParent<T>(bool includeInactive);
        Component[] GetComponentsInParent(System.Type t, bool includeInactive);
        T[] GetComponentsInParent<T>();
        void GetComponentsInParent<T>(bool includeInactive, List<T> results);
        bool TryGetComponent<T>(out T component);
        bool TryGetComponent(System.Type type, out Component component);

        #endregion
    }

    public interface IDebugable
    {
        bool useDebug { get; }
    }

    public interface IDestroyable
    {
        [Serializable]
        public class DestroyEvent : UnityEvent<IDestroyable> { }
        DestroyEvent onDestroy { get; }
    }

    public interface IStartHandler
    {
        [Serializable]
        public class StartEvent : UnityEvent { }

        public StartEvent onStart { get; }
    }

    public interface IBehavior : IMonoBehaviour, IDestroyable
    {
        public void DestroyThis();
    }

    public interface IInteractable
    {
        bool interactable { set; get; }
    }


    public interface IValueContainer
    {
        public string value { set; get; }
    }
   
    public interface IDraggable
    {
        public struct DragData
        {
            public object item;
            public string description;

            public DragData(object item, string description)
            {
                this.item = item;
                this.description = description;
            }
        }

        /// <summary>
        /// 드래그 할 데이터를 가져옵니다.
        /// Intent에 포함되는 데이터 키
        /// item : 대상 데이터
        /// description : 대상에 대한 설명
        /// </summary>
        /// <returns></returns>
        public IDraggable.DragData GetDragData();
    }


    public interface ITogglable
    {
        public bool isPicked { set; get; }
    }

    public interface IPickable
    {
        public bool isPicked { get; }

        public void Pick();
    }

    public interface IPicker
    {
        public object target { get; }
    }


    public interface IActivatable
    {

        bool isActivated { set; get; }
    }

    public interface IKeyContainer<KEY>
    {
        KEY key { get; }
    }

    public interface IXElementUpdatable
    {
        [Serializable]
        public class XElementEvent : UnityEvent<XElement> { }

        XElementEvent onElementChanged { get; }
    }

    public interface IXElementContainer
    {
        XElement element { set; get; }
    }

    public interface IXElementItem
    {
        public string EName { set; get; }
        public string EID { set; get; }
        public XElement element { get; }
    }

    public interface IKeyPair<KEY, VALUE> : IKeyContainer<KEY>
    {
        VALUE value { set; get; }
    }


}


namespace UAM.BundleSystem
{
    public interface IHandleableXML
    {
        bool isValidated { get; }
        void Validate();

        bool SaveXML();
        bool LoadXML();
    }

    public interface IScriptConvertable
    {
        string ToScript();
    }
}

namespace UAM.RealtimeMonitors
{

    public interface IAppearable
    {
        [Serializable]
        public class AppearEvent : UnityEvent<bool> { }
        AppearEvent onChangeAppeared { get; }
        bool isAppeared { set; get; }
    }

    public interface ICursorable
    {
        bool isCursored { set; get; }
    }


    public interface IFocusable
    {
        bool isFocused { set; get; }
        float cameraSize { get; }
        Vector3 anchorPosition { get; }
    }
}

namespace UAM.UI
{

    public interface IRectContents
    {
        float width { get; }
        float height { get; }
    }

    public interface ISelector
    {
        object selecting { get; }
        void Select(object target);
    }


}
