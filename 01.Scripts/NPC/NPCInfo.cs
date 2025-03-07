using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NPCInfo
{
    public string _npcName;
    [Serializable]
    public class _2DArrary
    {
        public string[] _npcDialog;
    }
    public _2DArrary[] _npcDialogArrary = new _2DArrary[3];
}


