using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AndreStuff
{
    public class FishManager : MonoBehaviour
    {

        private FishData _fishData = null;
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
            float weight = 1f;
            _fishData = new FishData(fishes[Random.Range(0, fishes.Count)], weight);
        }
    }

    public class FishData
    {
        private float _weight = 0f;
        private FishType _fishType;
        
        public float GetWeight() => _weight;
        public FishType GetFishType() => _fishType;
        public void UpdateFishType(FishType fishType) => _fishType = fishType;
        public void UpdateWeight(float value) => _weight = value;

        public FishData(FishType fishType, float weight)
        {
            _fishType = fishType;
            _weight = weight;
        }
    }
}