using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace mgr
{
    public class LevelManager : ManagerBase<LevelManager>
    {
        public int levelId;
        public logic.LevelData levelData { get; private set; }
        public bool isReady { get; private set; }  // True: level data is loaded. Not sure if we still need it.
        public logic.BoardRuleLogicBase boardRuleLogic;

        public float timer = 0.0f;
        private enum TimerState
        {
            STOPPED,
            PLUS,
            MINUS
        }
        private TimerState timerState = TimerState.STOPPED;

        public event Action onLoadLevelData;
        public event Action<int/*cardId*/> onForceFlipBack;
        public event Action<int/*cardId*/> onRemove;
        public event Action onClearLevel;
        public event Action onFinish;
        public event Action<string, logic.BoardRuleLogicBase.JudgeState> onUpdatePartialText;

        private List<int> currentlyFlipped = new List<int>();

        private List<List<int>> alreadyRemoved = new List<List<int>>();
        public bool pendingCheck { get; private set; }  // True: some cards are flipped. Unable to take other clicks.

        // TODO: Where do we do this? 
        protected override void _InitBeforeAwake()
        {
            base._InitBeforeAwake();
        }

        public void ClearLevel()
        {
            boardRuleLogic = null;
            levelData = null;
            alreadyRemoved = new List<List<int>>();
            currentlyFlipped.Clear();

            isReady = false;
            pendingCheck = false;

            timerState = TimerState.STOPPED;
            onClearLevel();
        }

        public void SetLevel(int i)
        {
            levelId = i;
        }

        public void Update()
        {
            if (timerState == TimerState.MINUS)
            {
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                }
                else
                {
                    Debug.Log("Time has run out!");
                    timer = 0;
                    timerState = TimerState.STOPPED;
                    // TODO: add timeout alert.
                }
            }
            if (timerState == TimerState.PLUS)
            {
                timer += Time.deltaTime;
            }
        }

        public string GetCurrentTimer()
        {
            if (timer == 0)
            {
                return "0:00";
            }
            float minutes = Mathf.FloorToInt((timer+1) / 60);
            float seconds = Mathf.FloorToInt((timer+1) % 60);

            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        // Load level based on levelId. Requires SetLevel call first.
        public void LoadLevel()
        {
            isReady = false;
//            levelId = i;
            string level_name = DataLoader.Instance.levelList.levelInfo[levelId].levelName;
            // level_name sample: "LevelList.json".
            Addressables.LoadAssetAsync<TextAsset>("Assets/Data/Levels/" + level_name + ".json").Completed 
                += LevelList_Completed;
        }

        private void LevelList_Completed(AsyncOperationHandle<TextAsset> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                levelData = JsonUtility.FromJson<logic.LevelData>(handle.Result.text);

                Type type = Type.GetType("logic." + levelData.boardRuleLogicName);
                boardRuleLogic = (logic.BoardRuleLogicBase)System.Activator.CreateInstance(type, levelData);

                isReady = true;
                // The texture is ready for use.

                onLoadLevelData();
            } else
            {
                // TODO: maybe retry? This is essentially a crash.
            }
        }

        public void ResetTimer()
        {
            timer = 0.0f;
        }
        public void StartTimer()
        {
            timer = 0.0f;
            timerState = TimerState.PLUS;
        }

        public void FlipCard(int card_id)
        {
            pendingCheck = true;
            currentlyFlipped.Add(card_id);
            var status = boardRuleLogic.JudgeAndFlip(currentlyFlipped);

            string partial_text = boardRuleLogic.GetPartialText(currentlyFlipped);
            onUpdatePartialText(partial_text, status);

            Debug.Log("JudgeAndFlip=" + status + "cardId=" + card_id);
            Debug.Log(currentlyFlipped.Count);
            if (status == logic.BoardRuleLogicBase.JudgeState.INVALID)
            {
                StartCoroutine(WaitAndForceFlipBack());
            }
            else if (status == logic.BoardRuleLogicBase.JudgeState.VALID)
            {
                StartCoroutine(WaitAndRemove());
            } else
            {
                pendingCheck = false;
            }
        }

        IEnumerator WaitAndForceFlipBack()
        {
            yield return new WaitForSeconds(0.25f);
            foreach (int cardId in currentlyFlipped)
            {
                onForceFlipBack(cardId);
            }
            currentlyFlipped.Clear();
            pendingCheck = false;
            yield return 0;
        }

        IEnumerator WaitAndRemove()
        {
            // remove cards
            yield return new WaitForSeconds(0.25f);
            foreach (int cardId in currentlyFlipped)
            {
                Debug.Log("OnRemove:" + cardId);
                onRemove(cardId);
            }
            alreadyRemoved.Add(new List<int>(currentlyFlipped));
            Debug.Log("OnRemoveA" + alreadyRemoved[alreadyRemoved.Count - 1].Count);
            currentlyFlipped.Clear();
            Debug.Log("OnRemoveA" + alreadyRemoved[alreadyRemoved.Count - 1].Count);
            if (boardRuleLogic.CheckCompletion(alreadyRemoved))
            {
                onFinish();
//                ClearLevel();
                // TODO: finish
            }
            pendingCheck = false;
            yield return 0;
        }

        public void UndoRemove()
        {
            Debug.Log("UndoRemove");
            if (alreadyRemoved.Count == 0)
            {
                return;
            }
            Debug.Log(alreadyRemoved[alreadyRemoved.Count - 1].Count);
            // Need to flip back the current temporary cards, if any.
            foreach (int cardId in currentlyFlipped)
            {
                onForceFlipBack(cardId);
            }
            currentlyFlipped.Clear();
            onUpdatePartialText("", logic.BoardRuleLogicBase.JudgeState.PENDING);

            foreach (int cardId in alreadyRemoved[alreadyRemoved.Count - 1])
            {
                Debug.Log("UndoRemove:" + cardId);
                // Technically, undo remove and flip back is the same status.
                onForceFlipBack(cardId);
            }
            alreadyRemoved.RemoveAt(alreadyRemoved.Count - 1);
            boardRuleLogic.UndoRemove();

            pendingCheck = false;
            return;
        }
    }
}
