using System;
using System.Collections.Generic;
using System.Drawing;

namespace TowerDefence
{
    /*
     * Parent class of all the differnt tower types
     */
    abstract class Tower
    {
        protected static Map myMap;
        protected int myX;
        protected int myY;
        //the ticks left on a towers cooldown
        protected int cooldown = 0;
        protected int kills = 0;
        //The list of map squares that this tower can see in it's range that enemies can be on in reverse over 
        protected List<MapSquare> attackableSquares;

        protected Tower()
        {
            myX = 0;
            myY = 0;
            Console.WriteLine("Give the towers cords dummy");
        }

        /*
         * Basic contructor setting variables to those inputted and finding all the attackable squares
         */
        protected Tower(int x, int y, int tempRange)
        {
            myX = x;
            myY = y;
            attackableSquares = new List<MapSquare>();

            //Local stuff to add all correct squares to the attackable square list
            int minX = myX - tempRange;
            int maxX = myX + tempRange;
            int minY = myY - tempRange;
            int maxY = myY + tempRange;
            List<Cords> enemyPath = myMap.getEnemyPath();

            //Going through enemy path backwards to get the attackable squares in reverse order so towers attack the furthest ahead enemy first
            for(int i = enemyPath.Count-1; i >= 0; i--)
            {
                if(enemyPath[i].getX() <= maxX && enemyPath[i].getX() >=minX && enemyPath[i].getY() <= maxY && enemyPath[i].getY() >= minY)
                {
                    attackableSquares.Add(myMap.getMapSquare(enemyPath[i].getX(), enemyPath[i].getY()));
                }
            }
        }

        protected static void highlightAoE(int range, int myX, int myY)
        {
            colorAoE(Color.Yellow, range, myX, myY);
        }
        protected static void unhighlightAoE(int range, int myX, int myY)
        {
            colorAoE(Color.Transparent, range, myX, myY);
        }

        /*
         * Highlights all squares in the turrets range using the colour passed in
         */
        protected static void colorAoE(Color myColor, int range, int myX, int myY)
        {
            for (int i = range * -1; i <= range; i++)
            {
                if (myX + i >= 0 && myX + i < myMap.getWidth())
                {
                    for (int counter = range * -1; counter <= range; counter++)
                    {
                        if (myY + counter >= 0 && myY + counter < myMap.getHeight())
                        {
                            myMap.getMapSquare(myY + counter, myX + i).setBackcolour(myColor);
                        }
                    }
                }
            }
        }

        /*
         * Getters and setters
         */
        public int getX()
        {
            return myX;
        }
        public int getY()
        {
            return myY;
        }

        public static void setMap(Map tempM)
        {
            myMap = tempM;
        }
        public int getKills()
        {
            return kills;
        }
    }

    /*
     * The interface for towers so I can use their methods without knowing the type
     */
    interface ITower
    {
        //Fires the tower
        void fireshot();
        //Highlights the squares this turret can attack
        void highlightAoE(int dummy, int x, int y);
        //opposite of highlightAoE
        void unhighlightAoE(int dummy, int x, int y);
        //Getters
        int getX();
        int getY();
        int getKills();
        int getPrice();
        Image getImage();
    }

    /*
     * The laser turret: attacks all enemies in a straight line from it in one direction at a time
     */
    class Laser : Tower, ITower
    {
        private static int price = 20;
        private static int range = 2;
        private static int maxCooldown = 30;
        private static Image myImage = Image.FromFile(Map.resourcesDirectory + @"Turrets\laser.png");

        public Laser()
        {

        }
        public Laser(int x, int y)
        {
            myX = y;
            myY = x;
            attackableSquares = new List<MapSquare>();

            //Getting the attackable squares of this tower (It is a + shape)
            //Looping from negative range so that we go to all sides of the tower
            for(int i = range * -1; i<=range; i++)
            {
                //For x's
                //Make sure it's in range
                if (myX+i >= 0 && myX+i < myMap.getWidth())
                {
                    //Get the type of the square checking, if it can have enemies add it to attackable squares
                    String spaceType = myMap.getMapSquare(myY, myX + i).getType();
                    if (spaceType == "S" || spaceType == "P")
                    {
                        attackableSquares.Add(myMap.getMapSquare(myY, myX + i));
                    }
                }
                //For y's
                //same as for x's
                if (myY + i >= 0 && myY + i < myMap.getHeight())
                {
                    String spaceType = myMap.getMapSquare(myY + i, myX).getType();
                    if (spaceType == "S" || spaceType == "P")
                    {
                        attackableSquares.Add(myMap.getMapSquare(myY + i,myX));
                    }
                }
            }
        }

        public new void highlightAoE(int dummy, int x, int y)
        {
            colorAoE(Color.Yellow, range, x, y);
        }
        public new void unhighlightAoE(int dummy, int x, int y)
        {
            colorAoE(Color.Transparent, range, x, y);
        }
        //Overloading colorAoE so that I can get the unique shape
        public static new void colorAoE(Color myColor, int range, int myX, int myY)
        {
            for (int i = range * -1; i <= range; i++)
            {
                if (myX + i >= 0 && myX + i < myMap.getWidth())
                {
                    myMap.getMapSquare(myY, myX + i).setBackcolour(myColor);
                }
            }
            for (int counter = range * -1; counter <= range; counter++)
            {
                if (myY + counter >= 0 && myY + counter < myMap.getHeight())
                {
                    myMap.getMapSquare(myY + counter,myX).setBackcolour(myColor);
                }
            }
        }
        //Unique fireshot 
        public void fireshot()
        {
            //if laser isn't on cooldown
            if (cooldown == 0)
            {
                int tempInt = attackableSquares.Count;
                //Look through all the attackable squares, if there's an enemy in it the check what exis it shares with this turret. If it shares the X axis then attack along it, else attack all squares along the Y axis
                for (int i = 0; i < tempInt; i++)
                {
                    if (attackableSquares[i].getIfEnemy())
                    {
                        if (attackableSquares[i].getX() != myX)
                        {
                            int loopLength= myMap.getWidth();
                            for(int counter = 0; counter< loopLength; counter++)
                            {
                                string typeChecking = myMap.getMapSquare(myY, counter).getType();
                                if (typeChecking == "P" || typeChecking == "S")
                                {
                                    kills += myMap.getMapSquare(myY, counter).takeHit(1, false);
                                }
                            }
                        }
                        else
                        {
                            int loopLength = myMap.getHeight();
                            for (int counter = 0; counter < loopLength; counter++)
                            {
                                string typeChecking = myMap.getMapSquare(counter, myX).getType();
                                if (typeChecking == "P" || typeChecking == "S")
                                {
                                    kills += myMap.getMapSquare(counter, myX).takeHit(1, false);
                                }
                            }
                        }
                        //Set the tower to cooldown
                        cooldown = maxCooldown;
                        return;
                    }
                }
                
            }
            else
            {
                cooldown--;
            }
        }

        public int getPrice()
        {
            return price;
        }

        public Image getImage()
        {
            return myImage;
        }

    }

    /*
     * Class for the cannon tower
     */
    class Cannon : Tower, ITower
    {
        private static int price = 65;
        private static int range = 3;
        private static int maxCooldown = 75;
        private static Image myImage = Image.FromFile(Map.resourcesDirectory + @"Turrets\cannon.png");

        public Cannon(){

        }
        public Cannon(int x, int y) : base(x, y, range)
        {

        }

        public void fireshot()
        {
            if (cooldown == 0)
            {
                int tempInt = attackableSquares.Count;
                for (int i = 0; i < tempInt; i++)
                {
                    if (attackableSquares[i].getIfEnemy())
                    {
                        kills += attackableSquares[i].takeHit(2,true);
                        cooldown = maxCooldown;
                        return;
                    }
                }
            }
            else
            {
                cooldown--;
            }
        }

        public int getPrice()
        {
            return price;
        }

        public new void highlightAoE(int dummy, int x, int y)
        {
            Tower.highlightAoE(range, x, y);
        }

        public new void unhighlightAoE(int dummy, int myX, int myY)
        {
            Tower.unhighlightAoE(range, myX, myY);
        }
        public Image getImage()
        {
            return myImage;
        }
    }

    /*
     * Class for the archer circle tower
     */
    class ArcherCircle : Tower, ITower
    {
        private static int price = 30;
        private static int range = 1;
        private static int maxCooldown = 50;
        private static Image myImage = Image.FromFile(Map.resourcesDirectory + @"Turrets\archerCircle.png");

        public ArcherCircle()
        {

        }
        public ArcherCircle(int x, int y) : base(x, y, range)
        {

        }

        //Attack all attackable squares if one has an enemy in it
        public void fireshot()
        {
            if (cooldown == 0)
            {
                int tempInt = attackableSquares.Count;
                bool shouldAttack = false;

                for (int i = 0; i < tempInt; i++)
                {
                    if (attackableSquares[i].getIfEnemy())
                    {
                        System.Diagnostics.Debug.WriteLine("Should attack "+tempInt + " things");
                        shouldAttack = true;
                        break;
                    }
                }

                if (shouldAttack)
                {
                    for (int i = 0; i < tempInt; i++)
                    {
                        if(attackableSquares[i].getType() == "D")
                        {
                            continue;
                        }
                        kills += attackableSquares[i].takeHit();
                    }
                    cooldown = maxCooldown;
                }
            }
            else
            {
                cooldown--;
            }
        }

        public int getPrice()
        {
            return price;
        }
        public Image getImage()
        {
            return myImage;
        }

        public new void highlightAoE(int dummy, int x, int y){
            Tower.highlightAoE(range, x, y);
        }

        public new void unhighlightAoE(int dummy, int myX, int myY)
        {
            Tower.unhighlightAoE(range, myX, myY);
        }

    }

    /*
     * Class for the baby tower: Can attack 2 enemies per cooldown
     */
    class Baby : Tower, ITower
    {
        private static int price = 55;
        private static int range = 2;
        private static int maxCooldown = 20;
        private static Image myImage = Image.FromFile(Map.resourcesDirectory + @"Turrets\baby.png");

        public Baby()
        {

        }
        public Baby(int x, int y) : base(x, y, range)
        {

        }

        public void fireshot()
        {
            if (cooldown == 0)
            {
                int numberThrown = 0;
                int tempInt = attackableSquares.Count;
                //Attack 2 enemies that are on the path using numberThrown to count how many have been attacked
                for (int i = 0; i < tempInt; i++)
                {
                    if (attackableSquares[i].getIfEnemy())
                    {
                        kills += attackableSquares[i].takeHit(1, false);
                        numberThrown++;
                        cooldown = maxCooldown;
                        if(numberThrown == 2)
                        {
                            return;
                        }
                    }
                }
            }
            else
            {
                cooldown--;
            }
        }

        public int getPrice()
        {
            return price;
        }

        public new void highlightAoE(int dummy, int x, int y)
        {
            Tower.highlightAoE(range, x, y);
        }

        public new void unhighlightAoE(int dummy, int myX, int myY)
        {
            Tower.unhighlightAoE(range, myX, myY);
        }
        public Image getImage()
        {
            return myImage;
        }
    }

    /*
     * Class for the wizard tower: very standard
     */
    class Wizard : Tower, ITower
    {
        private static int price = 30;
        private static int range = 2;
        private static int maxCooldown = 30;
        private static Image myImage = Image.FromFile(Map.resourcesDirectory + @"Turrets\wizard.png");

        public Wizard()
        {

        }
        public Wizard(int x, int y) : base(x, y, range)
        {

        }

        public void fireshot()
        {
            if (cooldown == 0)
            {
                int tempInt = attackableSquares.Count;
                for (int i = 0; i < tempInt; i++)
                {
                    if (attackableSquares[i].getIfEnemy())
                    {
                        kills += attackableSquares[i].takeHit(1, true);
                        cooldown = maxCooldown;
                        return;
                    }
                }
            }
            else
            {
                cooldown--;
            }
        }

        public int getPrice()
        {
            return price;
        }

        public new void highlightAoE(int dummy, int x, int y)
        {
            Tower.highlightAoE(range, x, y);
        }

        public new void unhighlightAoE(int dummy, int myX, int myY)
        {
            Tower.unhighlightAoE(range, myX, myY);
        }
        public Image getImage()
        {
            return myImage;
        }
    }
    
    /*
     * Class for the Faulty Battery turret: It has infinite range
     */
    class FaultyBattery : Tower, ITower
    {
        private static int price = 50;
        private static int range = 0;
        private static int maxCooldown = 110;
        private static Image myImage = Image.FromFile(Map.resourcesDirectory + @"Turrets\faultyBattery.png");

        public FaultyBattery()
        {

        }
        public FaultyBattery(int x, int y)
        {
            myX = x;
            myY = y;
            attackableSquares = new List<MapSquare>();

            List<Cords> enemyPath = myMap.getEnemyPath();
            
            //Faulty batteries attackable squares is the reverse of the enemy path
            for (int i = enemyPath.Count - 1; i >= 0; i--)
            {
                attackableSquares.Add(myMap.getMapSquare(enemyPath[i].getX(), enemyPath[i].getY()));
            }
        }

        public void fireshot()
        {
            if (cooldown == 0)
            {
                int tempInt = attackableSquares.Count;
                for (int i = 0; i < tempInt; i++)
                {
                    if (attackableSquares[i].getIfEnemy())
                    {
                        kills += attackableSquares[i].takeHit(2, true);
                        cooldown = maxCooldown;
                        return;
                    }
                }
            }
            else
            {
                cooldown--;
            }
        }

        public int getPrice()
        {
            return price;
        }

        public new void highlightAoE(int dummy, int x, int y)
        {
            Tower.highlightAoE(range, x, y);
        }

        public new void unhighlightAoE(int dummy, int myX, int myY)
        {
            Tower.unhighlightAoE(range, myX, myY);
        }
        public Image getImage()
        {
            return myImage;
        }
    }
}


