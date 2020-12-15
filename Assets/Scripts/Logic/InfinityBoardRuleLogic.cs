using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace logic
{
    public class TargetSumLogic : TargetMatchingLogic
    {
        public TargetSumLogic(LevelData level_data)
            : base(level_data)
        { }

        protected override int Calculate(List<int> materials)
        {
            int sum = 0;
            for (int i = 0; i < materials.Count; i++)
            {
                sum += materials[i];
            }
            return sum;
        }
    }

    public class RandomSumLogic : TargetMatchingLogic
    {
        public RandomSumLogic(LevelData level_data)
            : base(level_data)
        { }

        protected override int Calculate(List<int> materials)
        {
            if (materials.Count == 0)
            {
                return -1;
            }
            int sum = 0;
            for (int i = 0; i < materials.Count; i++)
            {
                sum += materials[i];
            }
            return sum;
        }

        protected override bool PrematureFail(List<int> materials)
        {
            if (materials.Count > 3)
            {
                return true;
            }
            return false;
        }

        // Start is called before the first frame update
        public override void Generator(List<int> param)
        {
            int material_count = param[0];
            target_count = param[1];
            int[] random_mapping = BoardRuleLogicUtil.GetRandomShuffler(material_count);

            cardDeck = new List<logic.CardData>(new logic.CardData[target_count + material_count]);
            int material_initialized = 0;

            // TODO: change this to card type?
            for (int i = 0; i < target_count; i++)
            {
                // 2 or 3, basically.
                int matgroup = UnityEngine.Random.Range(2, 4);
                if (material_count - material_initialized <= 2 * (target_count - i))
                {
                    matgroup = 2;
                }
                int partial_sum = 0;
                for (int j = 0; j < matgroup; j++)
                {
                    int temp = UnityEngine.Random.Range(1, 99);
                    partial_sum += temp;
                    var new_card = CardData.MaterialCard(temp);
                    Debug.Log("cardDeck.add " + temp + " at " + random_mapping[material_initialized]);
                    cardDeck[random_mapping[material_initialized]] = new_card;
                    material_initialized++;
                }
                var new_target = CardData.TargetCard(partial_sum);
                Debug.Log("cardDeck.add " + partial_sum + " at " + (material_count + i));
                cardDeck[material_count + i] = new_target;
            }
            for (int j = material_initialized; j < material_count; j++)
            {
                int temp = UnityEngine.Random.Range(1, 99);
                var new_card = CardData.MaterialCard(temp);
                Debug.Log("cardDeck.add " + temp + " at " + random_mapping[material_initialized]);
                cardDeck[random_mapping[material_initialized]] = new_card;
                material_initialized++;
            }
        }
    }

    public class RandomProductLogic : TargetMatchingLogic
    {
        public RandomProductLogic(LevelData level_data)
            : base(level_data)
        { }

        protected override int Calculate(List<int> materials)
        {
            if (materials.Count == 0)
            {
                return -1;
            }
            int product = 1;
            for (int i = 0; i < materials.Count; i++)
            {
                product *= materials[i];
            }
            return product;
        }

        protected override bool PrematureFail(List<int> materials)
        {
            if (materials.Count > 3)
            {
                return true;
            }
            return false;
        }

        // Start is called before the first frame update
        public override void Generator(List<int> param)
        {
            int material_count = param[0];
            target_count = param[1];
            int[] random_mapping = BoardRuleLogicUtil.GetRandomShuffler(material_count);

            cardDeck = new List<logic.CardData>(new logic.CardData[target_count + material_count]);
            int material_initialized = 0;

            // TODO: change this to card type?
            for (int i = 0; i < target_count; i++)
            {
                // 2 or 3, basically.
                int matgroup = UnityEngine.Random.Range(2, 4);
                if (material_count - material_initialized <= 2 * (target_count - i))
                {
                    matgroup = 2;
                }
                int partial_product = 1;
                for (int j = 0; j < matgroup; j++)
                {
                    int temp = UnityEngine.Random.Range(1, 9);
                    partial_product *= temp;
                    var new_card = CardData.MaterialCard(temp);
                    Debug.Log("cardDeck.add " + temp + " at " + random_mapping[material_initialized]);
                    cardDeck[random_mapping[material_initialized]] = new_card;
                    material_initialized++;
                }
                var new_target = CardData.TargetCard(partial_product);
                Debug.Log("cardDeck.add " + partial_product + " at " + (material_count + i));
                cardDeck[material_count + i] = new_target;
            }
            for (int j = material_initialized; j < material_count; j++)
            {
                int temp = UnityEngine.Random.Range(1, 9);
                var new_card = CardData.MaterialCard(temp);
                Debug.Log("cardDeck.add " + temp + " at " + random_mapping[material_initialized]);
                cardDeck[random_mapping[material_initialized]] = new_card;
                material_initialized++;
            }
        }
    }
}
