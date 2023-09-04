using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "New Wave Container", menuName = "Tower Defense/Enemies/Wave Container")]
    public class SWaveContainer : ScriptableObject
    {
        public SWaveData[] waves;
    }
}
