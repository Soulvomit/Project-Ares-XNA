using Krypton.Common;
using System.Collections.Generic;

namespace Krypton.Lights
{
    public interface ILight2D
    {
        BoundingRect Bounds { get; }

        void Draw(KryptonRenderHelper renderHelper, List<ShadowHull> hulls);
    }
}
