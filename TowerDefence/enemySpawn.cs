namespace TowerDefence
{
    /*
     * Basic class for storing number of regular enemies and number of strong enemies
     */
    class enemySpawn
    {
        private int regEnemy;
        private int strEnemy;
        public enemySpawn(int reg, int str)
        {
            regEnemy = reg;
            strEnemy = str;
        }
        public int getReg()
        {
            return regEnemy;
        }
        public int getStr()
        {
            return strEnemy;
        }
    }
}
