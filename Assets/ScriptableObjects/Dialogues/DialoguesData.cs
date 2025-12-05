using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.Dialogues
{
    [CreateAssetMenu(fileName = "DialoguesFile", menuName = "GOTYofTheYear/DialoguesData")]
    public class DialoguesData : ScriptableObject
    {
        public enum Language { Spanish, English }
    
        public Language language;
    
        [TextArea(3, 5)] public List<String> dialogues;
    }
}
