using logic;
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
        public int mainLevelId;
        public int levelId;
        public logic.LevelData levelData { get; private set; }
        public Queue<int> nextLevels = new Queue<int>();
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
        public event Action onClearLevel;
        public event Action onFinish;
        public event Action<string, logic.BoardRuleLogicBase.JudgeState> onUpdatePartialText;

        // List of cards that are currently selected.
        private List<int> currentlyFlipped = new List<int>();

        // List of cards that are already removed, mentioned in groups. Used mainly for 
        private List<List<int>> historicalModifyGroup = new List<List<int>>();
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
            historicalModifyGroup = new List<List<int>>();
            currentlyFlipped.Clear();

            isReady = false;
            pendingCheck = false;

            timerState = TimerState.STOPPED;
            onClearLevel();
        }

        public void SetInfinityLevel(List<int> range)
        {
            Debug.Log(range);
            int[] random_mapping = BoardRuleLogicUtil.GetRandomShuffler(range.Count);
            levelId = range[random_mapping[0]];
            for (int i = 1; i < range.Count; i++)
            {
                nextLevels.Enqueue(range[random_mapping[i]]);
            }
            LevelManager.Instance.ResetTimer();
        }
        public void SetLevel(int i, bool reset_timer = true)
        {
            levelId = i;
            if (reset_timer)
            {
                LevelManager.Instance.ResetTimer();
            }
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

        public int SwitchLevel()
        {
            if (nextLevels.Count != 0) {
                return nextLevels.Dequeue();
            } else {
                return -1;
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
                boardRuleLogic = (logic.BoardRuleLogicBase)Activator.CreateInstance(type, levelData);

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
            }
            else if (status == logic.BoardRuleLogicBase.JudgeState.SPECIAL)
            {
                StartCoroutine(WaitAndModify());
            }
            else
            {
                pendingCheck = false;
            }
        }

        IEnumerator WaitAndForceFlipBack()
        {
            yield return new WaitForSeconds(0.25f);
            foreach (int cardId in currentlyFlipped)
            {
                boardRuleLogic.cardDeck[cardId].cardStatus = logic.CardStatus.NORMAL;
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
                boardRuleLogic.cardDeck[cardId].ReduceCard();
            }
            historicalModifyGroup.Add(new List<int>(currentlyFlipped));
            currentlyFlipped.Clear();
            if (boardRuleLogic.CheckCompletion(historicalModifyGroup))
            {
                timerState = TimerState.STOPPED;
                onFinish();
                DataLoader.Instance.UpdateLevelProgress(levelId);
            }
            pendingCheck = false;
            yield return 0;
        }

        IEnumerator WaitAndModify()
        {
            // remove cards
            yield return new WaitForSeconds(0.25f);
            boardRuleLogic.SpecialModify(currentlyFlipped);
            historicalModifyGroup.Add(new List<int>(currentlyFlipped));
            currentlyFlipped.Clear();
            if (boardRuleLogic.CheckCompletion(historicalModifyGroup))
            {
                timerState = TimerState.STOPPED;
                onFinish();
                DataLoader.Instance.UpdateLevelProgress(levelId);
            }
            pendingCheck = false;
            yield return 0;
        }

        public void UndoRemove()
        {
            Debug.Log("UndoRemove");
            if (historicalModifyGroup.Count == 0)
            {
                return;
            }
            Debug.Log(historicalModifyGroup[historicalModifyGroup.Count - 1].Count);
            // Need to flip back the current temporary cards, if any.
            foreach (int cardId in currentlyFlipped)
            {
                boardRuleLogic.cardDeck[cardId].cardStatus = logic.CardStatus.NORMAL;
            }
            currentlyFlipped.Clear();
            onUpdatePartialText("", logic.BoardRuleLogicBase.JudgeState.PENDING);

            foreach (int cardId in historicalModifyGroup[historicalModifyGroup.Count - 1])
            {
                Debug.Log("UndoRemove:" + cardId);
                boardRuleLogic.cardDeck[cardId].RevertCard();
            }
            historicalModifyGroup.RemoveAt(historicalModifyGroup.Count - 1);
            boardRuleLogic.UndoRemove();

            pendingCheck = false;
            return;
        }
    }
}
