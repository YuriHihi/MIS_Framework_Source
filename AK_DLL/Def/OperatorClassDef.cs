﻿using UnityEngine;
using Verse;

namespace AK_DLL
{
    public class OperatorClassDef : Def
    {
        public int sortingOrder = 0;
        public OperatorSeriesDef series = null;
        public string icon = null;
        public string textureFolder;

        public Texture2D Icon
        {
            get { return ContentFinder<Texture2D>.Get(icon); }
        }
    }
}
