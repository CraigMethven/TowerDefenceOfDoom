namespace TowerDefence
{
    /*
     * Basic class for storing coordinates
     */
    class Cords
    {
        private int x;
        private int y;

        public Cords(int tempx, int tempy)
        {
            x = tempx;
            y = tempy;
        }

        public int getX()
        {
            return x;
        }
        public int getY()
        {
            return y;
        }
    }
}
