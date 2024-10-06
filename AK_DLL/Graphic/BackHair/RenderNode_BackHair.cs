﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    public class RenderNode_BackHair : PawnRenderNode_Hair
    {
        public RenderNode_BackHair(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
        }

        public override Graphic GraphicFor(Pawn pawn)
        {
            Ext_BackHair ext_BackHair = pawn.story?.hairDef?.GetModExtension<Ext_BackHair>();
            if (ext_BackHair == null) return null;
            return ext_BackHair.graphicData.GraphicColoredFor(pawn);
        }
    }
}
