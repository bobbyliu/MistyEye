using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using common;
//using UnityEngine.UI.Extension;

namespace mgr
{
    public class GameGlobal : Singleton<GameGlobal>
    {
//        private HUDText hUDText;
//        private GameObject canvasNotice;

        private void Awake()
        {
            var mgrs = new List<MonoBehaviour>();
            // Add mgrs
            mgrs.Add(CreateManager<SaveFile>());
            mgrs.Add(CreateManager<DataLoader>());
            mgrs.Add(CreateManager<LevelManager>());
            mgrs.Add(CreateManager<MenuManager>());
            mgrs.ForEach(m => m.gameObject.SetActive(true));

            //            CreateNoticeCanvas();

            Debug.Log($"Game Start!");
        }

        public T CreateManager<T>() where T : ManagerBase<T>
        {
            var obj = new GameObject(typeof(T).Name);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            var t = obj.AddComponent<T>();
            t.InitBeforeAwake();
            return t;
        }

        // TODO: what is this? 
        public void CreateNoticeCanvas()
        {
/*            canvasNotice = Instantiate(GameObject.FindGameObjectWithTag("MainCanvas"));
            canvasNotice.name = "CanvasNotice";
            canvasNotice.transform.SetParent(transform);

            var childs = canvasNotice.GetComponentsInChildren<UIBase>();
            foreach (var item in childs)
            {
                Destroy(item.gameObject);
            }

            //  提示框
            var prefab = ResourceMgr.Instance.GetUIPanel("HUDText");
            var p = Instantiate(prefab);
            p.gameObject.SetActive(true);
            p.transform.SetParent(canvasNotice.transform, false);
            var canvas = p.GetComponent<Canvas>();
            canvas.sortingLayerName = "UIFrontLayer";
            canvas.sortingOrder = 100;
            hUDText = p.GetComponent<HUDText>();

            //  淡入淡出
            SceneMgr.Instance.InitFade();
            //SceneMgr.Instance.fadeInObj.transform.SetParent(canvasNotice.transform);
            //SceneMgr.Instance.fadeOutObj.transform.SetParent(canvasNotice.transform);
            SceneMgr.Instance.fadeInOutObj.transform.SetParent(canvasNotice.transform, false);
            SceneMgr.Instance.fadeInOutObj.SetActive(false);
            */
        }

        // TODO: what is this? 
        private Color topColor = new Color(239 / 255f, 253 / 255f, 255 / 255f);
        private Color bottomColor = new Color(239 / 255f, 253 / 255f, 255 / 255f);
        public void Notice(string str)
        {
/*            //  错误提示
            if (hUDText == null)
            {
                throw new Exception("Notice error! hUDText is null");
            }
            hUDText.Add(str, topColor, bottomColor, 0.5f);*/
        }
    }
}