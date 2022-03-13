using System.Collections.Generic;
using System.IO;


namespace TowerDefence
{
    /*
     * The class that stores the map and all on it
     */
    class Map
    {
        //Map size
        private int lengthX;
        private int lengthY;
        //Start and end points
        private Cords startingPoint;
        private Cords defendPoint;
        //The list of all squares of the enemies path (in order)
        private List<Cords> enemyPath;
        //List of all towers on the map
        private List<ITower> myTowers;
        private bool continueEnemyPathSearch = true;
        //
        //List of all squares on the board
        private List<List<MapSquare>> theSquares;
        //General link to the resources folder
        public static string resourcesDirectory = Directory.GetCurrentDirectory() + @"\..\..\..\..\Resources\";
        //point to my main
        private static mainClass myMain;
        //name of the map file
        public static string MapName = "default";

        /*
         * Constructor to set up all the variables how they need to be. It also reads in and saves the map as a list of map squares
         */
        public Map(mainClass tempMain)
        {
            myMain = tempMain;
            MapSquare.setMap(this);
            theSquares = new List<List<MapSquare>>();
            enemyPath = new List<Cords>();
            myTowers = new List<ITower>();
            //Read in map file
            string[] MapFileLines = System.IO.File.ReadAllLines(resourcesDirectory + @"Maps\"+ MapName+".txt");
            lengthY = MapFileLines.Length;

            //initialise variables used in the loop
            bool splitOnce = false;
            string[] splitLine;
            //For all lines of the file
            for(int i = 0; i < lengthY; i++)
            {
                //Split up the x squares
                splitLine = MapFileLines[i].Split(',');
                //If there hasn't been a split before then set the length of x
                if (!splitOnce)
                {
                    lengthX = splitLine.Length;
                    splitOnce = true;
                    //initialiseMapSquares();
                    MapSquare.setupSizes(lengthX,lengthY);
                }
                //Setting up list
                theSquares.Add(new List<MapSquare>());
                
                //For all items in each line of the file
                for(int counter = 0; counter < lengthX; counter++)
                {
                    //Create the Map Square
                    theSquares[i].Add(new MapSquare(counter, i, splitLine[counter]));

                    if(splitLine[counter] == "S")
                    {
                        startingPoint = new Cords(i, counter);
                    }
                    else if(splitLine[counter] == "D")
                    {
                        defendPoint = new Cords(i, counter);
                    }
                }
            }
            setEnemyPath(startingPoint.getX(), startingPoint.getY());
        }

        /*
         * recursive loop to save the enemy path in the order they move along it
         */
        private void setEnemyPath(int currentX, int currentY)
        {
            if (!continueEnemyPathSearch)
            {
                return;
            }
            else if(defendPoint.getX() == currentX && defendPoint.getY() == currentY)
            {
                //System.Diagnostics.Debug.WriteLine("Reached end of enemy list");
                enemyPath.Add(new Cords(currentX, currentY));
                continueEnemyPathSearch = false;
                return;
            }
            else if(currentX < 0 || currentX >= lengthY || currentY < 0 || currentY >= lengthX )
            {
                //System.Diagnostics.Debug.WriteLine("Out of range " + currentX + " " + currentY);
                return;
            }
            else if (theSquares[currentX][currentY].getType() == "L" || theSquares[currentX][currentY].addedToPathList == true)
            {
                //System.Diagnostics.Debug.WriteLine("Not land or already added");
                return;
            }
            else
            {
                //System.Diagnostics.Debug.WriteLine("adding X: " + currentX + " Y: " +currentY);
                enemyPath.Add(new Cords(currentX, currentY));
                theSquares[currentX][currentY].addedToPathList = true;
                setEnemyPath(currentX, currentY + 1);
                setEnemyPath(currentX, currentY - 1);
                setEnemyPath(currentX + 1, currentY);
                setEnemyPath(currentX - 1, currentY);
            }
        }

        /*
         * Moves the enemies 1 space along enemy path and adds the enemies passed in
         */
        public void moveEnemies(int newReg, int newStr)
        {
            MapSquare curSquare;
            MapSquare prevSquare = theSquares[enemyPath[enemyPath.Count - 1].getX()][enemyPath[enemyPath.Count - 1].getY()];
            for(int i = enemyPath.Count-1; i > 0; i--)
            {
                curSquare = prevSquare;
                prevSquare = theSquares[enemyPath[i-1].getX()][enemyPath[i-1].getY()];
                if (curSquare.getType() == "D")
                {
                    myMain.removeHP(prevSquare.getRegEnemies() + (prevSquare.getStrEnemies()*2));
                }
                else
                {
                    curSquare.setEnemies(prevSquare.getRegEnemies(), prevSquare.getStrEnemies());
                }
            }
            theSquares[enemyPath[0].getX()][enemyPath[0].getY()].setEnemies(newReg, newStr);
        }

        //Runs the fire command of all towers on the map
        public void turretsFire()
        {
            int tempInt = myTowers.Count;
            for (int i = 0; i < tempInt; i++)
            {
                myTowers[i].fireshot();
            }
        }
        //Checks to see if there are enemies left on the board
        public bool checkGameEnded()
        {
            int loopLength = enemyPath.Count;
            for (int i = 0; i < loopLength; i++)
            {
                if (theSquares[enemyPath[i].getX()][enemyPath[i].getY()].getIfEnemy())
                {
                    return false;
                }
            }
            return true;
        }
        /*
         * Getters and setters
         */
        public MapSquare getMapSquare(int tempX, int tempY)
        {
            return theSquares[tempX][tempY];
        }

        public int getWidth()
        {
            return lengthX;
        }
        public int getHeight()
        {
            return lengthY;
        }

        public List<Cords> getEnemyPath()
        {
            return enemyPath;
        }

        public void addTower(ITower tempTower)
        {
            myTowers.Add(tempTower);
        }

    }
}
