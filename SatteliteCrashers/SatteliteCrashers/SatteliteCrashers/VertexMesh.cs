using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SatteliteCrashers
{
    public interface VertexMesh
    {
        VertexPositionColorNormal[] verts { get; }
        int[] indices { get; }
        Matrix world { get; set; }
    }
}
