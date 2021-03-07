using System;
using System.Threading;
using System.Linq;


namespace CursorMove
{
    class MyProgram
    {
        const byte X = 0;
        const byte Y = 1;
        const byte MIN = 0;
        const byte MAX = 1;
        readonly Random rnd = new Random();
        private readonly int windowWidth = Console.WindowWidth;
        private readonly int windowHeight = Console.WindowHeight;

        //  System.Diagnostics.Debugger.Break(); - Sparar den så jag inte glömmer bort.

        public void Run()
        {
            BreakoutGame();
        }

        private void BreakoutGame()
        {
            #region Variables

            /* Först kollar jag dimensionerna av konsol applikationen och stoppar max och minvärde som liknar ett matematisk koordinatsystem eller en schackbräda.
             * (x går från 0 till maxvärde (Console.WindowWidth) som representerar vänster till höger i konsolen.)
             * (y går från 0 till maxvärde (Console.WindowHeight) som representerar höjden där 0 är längst upp och när värdet stiger så flyttas den neråt.)
             */
            int xBorderMin = 19;
            int xBorderMax = windowWidth - 1;
            int yBorderMin = 0;
            int yBorderMax = windowHeight - 1;

            int[,] windowLimit = new int[2, 2];

            windowLimit[X, MIN] = xBorderMin + 1;
            windowLimit[X, MAX] = xBorderMax - 1;
            windowLimit[Y, MIN] = yBorderMin + 1;
            windowLimit[Y, MAX] = yBorderMax - 1;

            byte cursorAmount = 1;
            byte paddleSize = (byte)(windowLimit[X, MIN] + ((windowLimit[X, MAX] - windowLimit[X, MIN]) / 20));
            byte xBrickSize = 5;
            byte xBrickSpacing = 0;
            byte brickColumns = 3;


            int paddleVelocity = 2;
            int xPaddle = windowLimit[X, MIN] + ((windowLimit[X, MAX] - windowLimit[X, MIN]) / 2);
            int yPaddle = windowLimit[Y, MAX] - 1;
            int colorStep = 0;

            int brickRows = (windowLimit[X, MAX] - windowLimit[X, MIN]) / (xBrickSize + xBrickSpacing);
            int brickAmount = brickRows * brickColumns;

            int freeSpace = ((windowLimit[X, MAX] + 1) - windowLimit[X, MIN]) - ((xBrickSpacing * (brickRows - 1)) + (brickRows * xBrickSize));

            string brickTextStr = new string('\u2588', xBrickSize);
            string brickClearText = new string(' ', xBrickSize);
            string paddleText = new string('\u2588', paddleSize);
            string paddleClearText = new string(' ', paddleSize);

            char cursorChar = '\u25A0';

            int[,] posAxis = new int[cursorAmount, 2];
            int[,] cursorVelocity = new int[cursorAmount, 2];
            int[,] scoreBricks = new int[brickAmount, 2];
            int[] cursorColor = new int[cursorAmount];
            int[] brickColor = new int[brickAmount];

            string[] brickText = new string[brickAmount];

            bool isActive = true;
            bool isRunning = true;

            bool[] inactiveCursor = new bool[cursorAmount];
            bool?[] hitBricks = new bool?[brickAmount];

            int playerScore = 0;

            Console.CursorVisible = false;  // Gömmer kommandotolkens inbyggda cursor.
            Console.TreatControlCAsInput = true;

            #endregion Variables

            while (isRunning)
            {
                #region Initializing
                PrintGameArea(yBorderMin, yBorderMax, xBorderMin, xBorderMax, windowLimit[X, MIN], windowLimit[X, MAX], windowLimit[Y, MIN], windowLimit[Y, MAX]);
                isActive = true;
                Console.BackgroundColor = ConsoleColor.Blue;

                for (int i = 0; i < cursorAmount; i++)
                {
                    inactiveCursor[i] = true;
                }

                for (int i = 0; i < brickAmount; i++)
                {
                    hitBricks[i] = false;
                }

                int yBrickAxis = windowLimit[Y, MIN];
                for (int i = 0; i < brickAmount; i++)   //  Behöver fixas och optimeras, skapar problem för loop av isRunning om inte yBrickAxis blir reset före - Känns ineffektivt.
                {
                    if (i == 0)
                    {
                        scoreBricks[i, X] = windowLimit[X, MIN] + (freeSpace / 2);
                        scoreBricks[i, Y] = yBrickAxis;
                    }

                    if (i % brickRows == 0 && i != 0)
                    {
                        yBrickAxis++;
                        scoreBricks[i, X] = scoreBricks[0, X];
                        scoreBricks[i, Y] = yBrickAxis;
                    }
                    else if (i != 0)
                    {
                        scoreBricks[i, X] = scoreBricks[(i - 1), X] + xBrickSize + xBrickSpacing;
                        scoreBricks[i, Y] = yBrickAxis;
                    }

                    colorStep++;
                    switch (colorStep)
                    {
                        case 1:
                            brickColor[i] = 12;
                            break;
                        case 2:
                            brickColor[i] = 6;
                            break;
                        case 3:
                            brickColor[i] = 14;
                            break;
                        case 4:
                            brickColor[i] = 10;
                            break;
                        case 5:
                            brickColor[i] = 13;
                            break;
                        case 6:
                            brickColor[i] = 5;
                            break;
                        default:
                            break;
                    }

                    if (colorStep == 6)
                    {
                        colorStep = 0;
                    }
                }

                for (int i = 0; i < brickAmount; i++)
                {
                    brickText[i] = brickTextStr;
                    Console.SetCursorPosition(scoreBricks[i, X], scoreBricks[i, Y]);
                    Console.ForegroundColor = (ConsoleColor)brickColor[i];
                    Console.WriteLine(brickText[i]);
                }
                #endregion Initializing

                while (isActive)
                {
                    for (int i = 0; i < cursorAmount; i++)
                    {
                        if (inactiveCursor[i])      //  Om tecknet "försvinner" d.v.s. går till y-gränsen så blir den inactive. Denna if-sats ger den en nya värden och "startar" om.
                        {
                            posAxis[i, X] = rnd.Next(windowLimit[X, MIN], windowLimit[X, MAX]);
                            posAxis[i, Y] = rnd.Next((windowLimit[Y, MIN] + brickColumns), windowLimit[Y, MAX]);
                            cursorColor[i] = rnd.Next(1, 16);
                            cursorVelocity[i, X] = RandomizeVelocity();
                            cursorVelocity[i, Y] = -1;
                            inactiveCursor[i] = false;
                        }

                        if (cursorVelocity[i, Y] == 0)
                        {
                            inactiveCursor[i] = true;
                        }
                    }

                    if (!hitBricks.Contains(false))
                    {
                        isActive = false;
                    }

                    for (int i = 0; i < brickAmount; i++)
                    {
                        if (hitBricks[i] == true)
                        {
                            Console.SetCursorPosition(scoreBricks[i, X], scoreBricks[i, Y]);
                            Console.Write(brickClearText);
                            playerScore++;
                            ScoreCounter(playerScore);
                            hitBricks[i] = null;
                        }
                    }

                    #region Keyboardinput
                    while (Console.KeyAvailable)   //  Det funkar men är inte så responsiv. Mest troligt pga tangentbordsbuffern. Vet ej hur det kan fixas i consoleapp.
                    {
                        ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                        if (keyInfo.Key == ConsoleKey.LeftArrow)
                        {
                            if (xPaddle - paddleVelocity <= windowLimit[X, MIN])
                            {
                                xPaddle = windowLimit[X, MIN];
                            }
                            else
                            {
                                xPaddle -= paddleVelocity;
                            }
                        }
                        else if (keyInfo.Key == ConsoleKey.RightArrow)
                        {
                            if ((xPaddle + paddleSize) + paddleVelocity > windowLimit[X, MAX])
                            {
                                xPaddle = windowLimit[X, MAX] - (paddleSize - 1);
                            }
                            else
                            {
                                xPaddle += paddleVelocity;
                            }
                        }
                    }
                    #endregion Keyboardinput

                    #region Hit-detection
                    for (int i = 0; i < cursorAmount; i++)
                    {
                        if (!inactiveCursor[i])
                        {
                            for (int j = 0; j < brickAmount; j++)
                            {
                                if (hitBricks[j] == false)
                                {
                                    if (posAxis[i, Y] + cursorVelocity[i, Y] == scoreBricks[j, Y])
                                    {
                                        if (posAxis[i, X] >= scoreBricks[j, X] && (posAxis[i, X] <= (scoreBricks[j, X]) + xBrickSize - 1))
                                        {
                                            hitBricks[j] = true;
                                            cursorVelocity[i, Y] = -cursorVelocity[i, Y];
                                        }
                                        else if (posAxis[i, X] + cursorVelocity[i, X] == (scoreBricks[j, X] + xBrickSize) || posAxis[i, X] == (scoreBricks[j, X] - 1))
                                        {
                                            cursorVelocity[i, X] = -cursorVelocity[i, X];
                                        }
                                    }
                                }
                            }

                            // Kollar om tecknets x värde är inom plattans intervall.
                            if ((posAxis[i, Y] + cursorVelocity[i, Y]) >= windowLimit[Y, MAX] - 1)
                            {
                                if (((posAxis[i, X] + cursorVelocity[i, X]) >= xPaddle) && (posAxis[i, X] + cursorVelocity[i, X]) <= (xPaddle + paddleSize))
                                {
                                    cursorVelocity[i, Y] = -cursorVelocity[i, Y];
                                    posAxis[i, Y] = windowLimit[Y, MAX] - 2;
                                }
                                else
                                {
                                    cursorVelocity[i, Y] = 0;
                                    posAxis[i, Y] = windowLimit[Y, MAX];
                                }
                            }
                            else if ((posAxis[i, Y] + cursorVelocity[i, Y]) < windowLimit[Y, MIN])
                            {
                                cursorVelocity[i, Y] = -cursorVelocity[i, Y];
                                posAxis[i, Y] = windowLimit[Y, MIN];
                            }

                            if ((posAxis[i, X] + cursorVelocity[i, X]) >= windowLimit[X, MAX] || (posAxis[i, X] + cursorVelocity[i, X]) <= windowLimit[X, MIN])
                            {
                                cursorVelocity[i, X] = -cursorVelocity[i, X];
                            }

                            posAxis[i, X] += cursorVelocity[i, X];
                            posAxis[i, Y] += cursorVelocity[i, Y];
                        }
                    }
                    #endregion Hit-detection

                    Console.SetCursorPosition(xPaddle, yPaddle);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(paddleText);

                    for (int i = 0; i < cursorAmount; i++)
                    {
                        if (!inactiveCursor[i])
                        {
                            Console.SetCursorPosition(posAxis[i, X], posAxis[i, Y]);
                            Console.ForegroundColor = (ConsoleColor)cursorColor[i];
                            Console.Write(cursorChar);
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }

                    Thread.Sleep(30);

                    for (int i = 0; i < cursorAmount; i++)
                    {
                        if (!inactiveCursor[i])
                        {
                            Console.SetCursorPosition(posAxis[i, X], posAxis[i, Y]);
                            Console.Write(" ");
                        }
                    }

                    Console.SetCursorPosition(xPaddle, yPaddle);
                    Console.Write(paddleClearText);
                }
            }
            return;
        }

        int RandomizeVelocity()
        {
            int RandomVelocity = rnd.Next(2) * 2 - 1;

            return RandomVelocity;
        }

        private void PrintGameArea(int yBorderMin, int yBorderMax, int xBorderMin, int xBorderMax, int windowLimitXMin, int windowLimitXMax, int windowLimitYMin, int windowLimitYMax)
        {
            string backgroundColor = new string(' ', (windowLimitXMax + 1) - windowLimitXMin);
            string verticalText = new string('\u2588', (xBorderMax + 2) - windowLimitXMin);

            ScoreCounter(0);
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(windowLimitXMin - 1, yBorderMin);
            Console.Write(verticalText);
            Console.SetCursorPosition(windowLimitXMin - 1, yBorderMin + 1);
            Console.Write(verticalText);
            Console.MoveBufferArea(windowLimitXMin - 1, yBorderMin + 1, verticalText.Length, 1, windowLimitXMin - 1, yBorderMax);

            for (int i = yBorderMin + 1; i < yBorderMax; i++)     // Ritar fönstret i konsolen.
            {
                Console.SetCursorPosition(xBorderMax, i);
                Console.Write('\u2588');
                Console.SetCursorPosition(xBorderMin, i);
                Console.Write('\u2588');
            }
            Console.BackgroundColor = ConsoleColor.Blue;
            for (int i = windowLimitYMin; i <= windowLimitYMax; i++)
            {
                Console.SetCursorPosition(windowLimitXMin, i);
                Console.Write(backgroundColor);
            }
            return;
        }

        void ScoreCounter(int playerScore)
        {
            Console.SetCursorPosition(0, 0);
            Console.Write("Score: {0}", playerScore);
        }
    }
}