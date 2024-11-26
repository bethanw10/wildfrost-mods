using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HadesFrost.Setup
{
    public static class Eyes
    {
        public static void Setup()
        {
            var list = new List<EyeData>
            {
                Create("bethanw10.hadesfrost.Frinos", (0.18f,0.91f,1.00f,1.00f,360f)),
                Create("bethanw10.hadesfrost.Toula", (0.35f,1.04f,1.00f,1.00f,2f), (0.72f,1.06f,1.00f,1.00f,2f)),
                Create("bethanw10.hadesfrost.Aphrodite", (0.32f,1.44f,1.00f,1.00f,2f), (0.19f,1.50f,1.00f,1.00f,2f)),
                Create("bethanw10.hadesfrost.Zeus", (0.47f,1.65f,1.00f,1.00f,0f), (0.30f,1.67f,1.00f,1.00f,0f)),
                Create("bethanw10.hadesfrost.Poseidon", (-0.06f,0.96f,1.00f,1.00f,0f), (-0.23f,0.93f,1.00f,1.00f,0f)),
                Create("bethanw10.hadesfrost.Hestia", (0.35f,1.33f,1.00f,1.00f,0f), (0.15f,1.35f,1.00f,1.00f,0f)),
                Create("bethanw10.hadesfrost.Hermes", (-0.10f,1.39f,1.00f,1.00f,0f), (-0.26f,1.43f,1.00f,1.00f,0f)),
                Create("bethanw10.hadesfrost.Hera", (0.29f,1.65f,1.00f,1.00f,0f), (0.11f,1.67f,1.00f,1.00f,0f)),
                Create("bethanw10.hadesfrost.Hephaestus", (-0.07f,1.18f,1.00f,1.00f,0f), (-0.26f,1.19f,1.00f,1.00f,0f)),
                Create("bethanw10.hadesfrost.Dionysus", (0.32f,1.33f,1.00f,1.00f,0f), (0.12f,1.29f,1.00f,1.00f,0f)),
                Create("bethanw10.hadesfrost.Demeter", (-0.14f,1.72f,1.00f,1.00f,0f), (0.10f,1.71f,1.00f,1.00f,0f)),
                Create("bethanw10.hadesfrost.Athena", (-0.06f,1.57f,1.00f,1.00f,0f), (-0.29f,1.56f,1.00f,1.00f,0f), (-0.86f,1.74f,1.00f,1.00f,0f), (-1.01f,1.75f,1.00f,1.00f,0f)),
                Create("bethanw10.hadesfrost.Artemis", (0.26f,1.40f,1.00f,1.00f,0f), (0.11f,1.36f,1.00f,1.00f,0f)),
                Create("bethanw10.hadesfrost.Ares", (0.21f,1.61f,1.00f,1.00f,0f), (0.07f,1.61f,1.00f,1.00f,0f)),
                Create("bethanw10.hadesfrost.Apollo", (-0.10f,1.78f,1.00f,1.00f,0f), (0.08f,1.77f,1.00f,1.00f,0f)),
            };
            //WARNING: The EyeData will NOT be removed upon unload. Call Eyes() underneath CreateModAssets() in the Load method. 

            AddressableLoader.AddRangeToGroup("EyeData", list);
        }

        private static EyeData Create(string cardName, params (float, float, float, float, float)[] data)
        {
            var eyeData = ScriptableObject.CreateInstance<EyeData>();
            eyeData.cardData = cardName;
            eyeData.name = eyeData.cardData + "_EyeData";
            eyeData.eyes = data.Select(e => new EyeData.Eye
            {
                position = new Vector2(e.Item1, e.Item2),
                scale = new Vector2(e.Item3, e.Item4),
                rotation = e.Item5
            }).ToArray();

            return eyeData;
        }
    }
}