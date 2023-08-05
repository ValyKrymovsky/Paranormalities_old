using System;

namespace MyCode.GameData.GameSettings
{
    [Serializable]
    public struct XYStructure
    {
        public int x;
        public int y;

        public XYStructure(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
