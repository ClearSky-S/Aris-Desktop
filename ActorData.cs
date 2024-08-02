using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assistant
{
    [Serializable]
    public struct VoiceData
    {
        public AudioClip Clip;
        public string Text;
    }
    
    [CreateAssetMenu(fileName = "ActorData", menuName = "ActorData" )]
    public class ActorData : ScriptableObject
    {
        public string actorName;
        public VoiceData[] LoginVoice;
        public VoiceData[] TouchVoice;

        private int _lastTouchIndex = -1;
        
        public VoiceData GetLoginVoice()
        {
            // 로그인 대사
            return LoginVoice[Random.Range(0, LoginVoice.Length)];
        }
        public VoiceData GetTouchVoice()
        {
            // 터치 대사
            int index = Random.Range(0, TouchVoice.Length);
            while (TouchVoice.Length>1 && index == _lastTouchIndex)
            {
                index = Random.Range(0, TouchVoice.Length);
            }
            _lastTouchIndex = index;
            return TouchVoice[index];
        }
    }
}