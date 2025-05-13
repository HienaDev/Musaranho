using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AndreStuff
{
    public class FishManager : MonoBehaviour
    {

        public string _debug;
        private FishData _fishData;
        public FishData GetFishData() => _fishData;
        private void Start()
        {
            // created function cause not sure if needed to generate fish after Start.
            GenerateFish();
        }

        private void GenerateFish()
        {
            // change later for different chances per type of fish maybe?
            List<FishType> fishes = Enum.GetValues(typeof(FishType)).Cast<FishType>().ToList();

            // change later weight depending on rarity.
            int weightChance = Random.Range(0, 100);
            FishWeight weightType = GetFishWeightType(weightChance);
            (float min, float max) weightRange = GetFishWeightRange(weightType);
            float weight = Random.Range(weightRange.min, weightRange.max);
            FishType fishType = fishes[Random.Range(0, fishes.Count)];
            _fishData = new FishData(fishType, weightType, weight);
            _debug = $"{fishType} {weightType} {weight}";
        }
        
        private FishWeight GetFishWeightType(int weightChance)
        {
            return weightChance switch
            {
                <= 40 => FishWeight.LIGHT,
                    <= 70 => FishWeight.MEDIUM,
                    <= 90 => FishWeight.HEAVY,
                _ => FishWeight.GIANT
            };
        }

        private (float, float) GetFishWeightRange(FishWeight weightType)
        {
            return weightType switch
            {
                FishWeight.LIGHT => (0.1f, 1.5f),
                FishWeight.MEDIUM => (1.5f, 10f),
                FishWeight.HEAVY => (10f, 50f),
                _ => (50f, 300f)
            };
        }
    }

    public class FishData
    {
        public float Weight { get; }
        public FishWeight WeighType { get; }
        public FishType FishType { get; }

        public FishData(FishType fishType, FishWeight weightType, float weight)
        {
            FishType = fishType;
            WeighType = weightType;
            Weight = weight;
        }
    }
}