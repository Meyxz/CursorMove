using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CursorMove
{
    class MyProgram
    {

        const byte X = 0;
        const byte Y = 1;
        const byte MIN = 0;
        const byte MAX = 1;

        Random rnd = new Random();

        public void Run()
        {
            /* Först kollar jag dimensionerna av konsol applikationen och stoppar max och minvärde som liknar ett matematisk koordinatsystem eller en schackbräda.
             * (x går från 0 till maxvärde (Console.WindowWidth) som representerar vänster till höger i konsolen.)
             * (y går från 0 till maxvärde (Console.WindowHeight) som representerar höjden där 0 är längst upp och när värdet stiger så flyttas den neråt.)
             */

            int xBorder = Console.WindowWidth - 1;
            int yBorder = Console.WindowHeight - 1;

            int[,] windowLimit = new int[2, 2];

            windowLimit[X, MIN] = 1;
            windowLimit[X, MAX] = xBorder - 1;
            windowLimit[Y, MIN] = 0;
            windowLimit[Y, MAX] = yBorder - 1;

            byte cursorAmount = 1;
            byte paddleSize = 10;
            byte xBrickSize = 5;
            byte xBrickSpacing = 2;
            byte brickColumns = 3;

            byte yBrickAxis = 0;

            int paddleVelocity = 1;
            int xPaddle = windowLimit[X, MIN];
            int yPaddle = windowLimit[Y, MAX] - 1;

            int brickRows = (windowLimit[X, MAX] - windowLimit[X, MIN]) / (xBrickSize + xBrickSpacing);
            int brickAmount = brickRows * brickColumns;

            int freeSpace = (windowLimit[X, MAX] - windowLimit[X, MIN]) - ((xBrickSpacing * (brickRows - 2)) + (brickRows * xBrickSize));

            string brickText = string.Empty;
            string brickClearText = string.Empty;

            char cursorChar = 'O'; 

            int[,] posAxis = new int[cursorAmount, 2];
            int[,] cursorVelocity = new int[cursorAmount, 2];
            int[,] scoreBricks = new int[brickAmount, 2];
            int[] cursorColor = new int[cursorAmount];

            bool isActive = true;
            bool isRunning = true;

            bool[] inactiveCursor = new bool[cursorAmount];
            bool[] hitBricks = new bool[brickAmount];

            ConsoleKeyInfo keyInfo = new ConsoleKeyInfo((char)0, (ConsoleKey)0, false, false, false);

            Console.CursorVisible = false;  // Gömmer kommandotolkens inbyggda cursor.
            Console.TreatControlCAsInput = true;

            while (isRunning)
            {
                for (int i = 0; i < cursorAmount; i++)
                {
                    inactiveCursor[i] = true;
                }

                for (int i = 0; i < brickAmount; i++)
                {
                    hitBricks[i] = false;
                }

                for (int i = 0; i <= yBorder; i++)     // Ritar "väggarna" i konsolen.
                {
                    Console.SetCursorPosition(xBorder, i);
                    Console.Write("|");
                    Console.SetCursorPosition(windowLimit[X, MIN] - 1, i);
                    Console.Write("|");
                }


                for (int i = 0; i < brickColumns; i++)
                {
                    scoreBricks[brickRows * i, X] = windowLimit[X, MIN] + (freeSpace / 2);
                    scoreBricks[brickRows * i, Y] = i;
                }

                for (int i = 1; i < brickAmount; i++)
                {
                    if (i % brickRows == 0)
                    {
                        yBrickAxis++;
                        i++;
                    }

                    scoreBricks[i, X] = scoreBricks[(i - 1), X] + xBrickSize + xBrickSpacing;
                    scoreBricks[i, Y] = yBrickAxis;
                }

                for (int i = 1; i <= xBrickSize; i++)
                {
                    brickText += "-";
                    brickClearText += " ";
                }

                for (int i = 0; i < brickAmount; i++)
                {
                    Console.SetCursorPosition(scoreBricks[i, X], scoreBricks[i, Y]);
                    Console.WriteLine(brickText);
                }

                while (isActive)
                {
                    for (int i = 0; i < cursorAmount; i++)
                    {
                        if (inactiveCursor[i])      //  Om tecknet "försvinner" d.v.s. går till y-gränsen så blir den inactive. Denna if-sats ger den en nya värden och "startar" om.
                        {
                            posAxis[i, X] = rnd.Next(windowLimit[X, MIN], windowLimit[X, MAX]);
                            posAxis[i, Y] = rnd.Next((windowLimit[Y, MIN] + brickColumns), windowLimit[Y, MAX]);
                            cursorColor[i] = rnd.Next(1, 16);
                            cursorVelocity[i, X] = RandomizeVelocity(cursorVelocity[i, X]);
                            cursorVelocity[i, Y] = RandomizeVelocity(cursorVelocity[i, Y]);
                            inactiveCursor[i] = false;
                        }

                        if (cursorVelocity[i, Y] == 0)      //  Om hastigheten är == 0, så blir den inactive.
                        {
                            inactiveCursor[i] = true;
                        }

                    }

                    for (int i = 0; i < brickAmount; i++)
                    {
                        if (hitBricks[i])
                        {
                            Console.SetCursorPosition(scoreBricks[i, X], scoreBricks[i, Y]);
                            Console.Write(brickClearText);
                        }
                    }

                    if (Console.KeyAvailable)   //  Behöver fixas, den stannar inte när man släpper tangenten.
                    {
                        keyInfo = WaitForKey(10);

                        if (keyInfo.Key == ConsoleKey.LeftArrow)
                        {
                            paddleVelocity = -1;
                        }
                        else if (keyInfo.Key == ConsoleKey.RightArrow)
                        {
                            paddleVelocity = 1;
                        }
                        else
                        {
                            paddleVelocity = 0;
                        }
                    }

                    if (((xPaddle + paddleSize) + paddleVelocity) > windowLimit[X, MAX])      // Kollar om plattan går out of bounds.
                    {
                        paddleVelocity = 0;
                        xPaddle = windowLimit[X, MAX] - paddleSize;
                    }
                    else if ((xPaddle + paddleVelocity) <= windowLimit[X, MIN])
                    {
                        paddleVelocity = 0;
                        xPaddle = windowLimit[X, MIN];
                    }

                    xPaddle += paddleVelocity;     // Beräknar plattans position.

                    for (int i = 0; i < cursorAmount; i++)
                    {
                        if (!inactiveCursor[i])      // Jag har använt en boolean vid namnet showCursor för att undvika att beräkna tecken som inte visas.
                        {
                            for (int j = 0; j < brickAmount; j++)
                            {
                                if (!hitBricks[j])
                                {
                                    if (posAxis[i, X] >= scoreBricks[j, X] && (posAxis[i, X] <= (scoreBricks[j, X]) + xBrickSize))
                                    {
                                        if (posAxis[i, Y] == scoreBricks[j, Y])
                                        {
                                            hitBricks[j] = true;
                                            cursorVelocity[i, Y] = -cursorVelocity[i, Y];
                                        }
                                    }
                                }
                            }

                            if ((posAxis[i, Y] + cursorVelocity[i, Y]) >= windowLimit[Y, MAX])
                            {
                                // Kollar om tecknets x värde är inom plattans intervall.
                                if (((posAxis[i, X] + cursorVelocity[i, X]) >= xPaddle) && (posAxis[i, X] + cursorVelocity[i, X]) <= (xPaddle + paddleSize))
                                {
                                    cursorVelocity[i, Y] = -cursorVelocity[i, Y];
                                    posAxis[i, Y] = windowLimit[Y, MAX];
                                }
                                else
                                {
                                    cursorVelocity[i, Y] = 0;
                                    posAxis[i, Y] = yBorder;
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

                    for (int i = 0; i <= paddleSize; i++)
                    {
                        Console.SetCursorPosition(xPaddle + i, yPaddle);
                        Console.Write("_");
                    }

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

                    for (int i = 0; i <= paddleSize; i++)
                    {
                        Console.SetCursorPosition(xPaddle + i, yPaddle);
                        Console.Write(" ");
                    }
                }
            }
        }

        int RandomizeVelocity(int RandomVelocity)
        {
            RandomVelocity = rnd.Next(0, 2) * 2 - 1;

            return RandomVelocity;
        }

        ConsoleKeyInfo WaitForKey(int ms)
        {
            int delay = 0;
            while (delay < ms)
            {
                if (Console.KeyAvailable)
                {
                    return Console.ReadKey();
                }
                Thread.Sleep(ms);
                delay += ms;
            }
            return new ConsoleKeyInfo((char)0, (ConsoleKey)0, false, false, false);
        }
    }
}