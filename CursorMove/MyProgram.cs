using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CursorMove
{
    class MyProgram
    {
        const byte x = 0;
        const byte y = 1;
        const byte min = 0;
        const byte max = 1;

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

            windowLimit[x, min] = 0;
            windowLimit[x, max] = xBorder - 1;
            windowLimit[y, min] = 0;
            windowLimit[y, max] = yBorder - 1;


            const byte cursorAmount = 5;
            const byte paddleSize = 10;
            const byte xBrickSize = 5;
            const byte xBrickSpacing = 2;
            const byte brickColumns = 3;

            byte yBrickAxis = 0;
            
            int paddleVelocity = 1;
            int xPaddle = windowLimit[x, min];
            int yPaddle = windowLimit[y, max] - 1;

            int brickRows = (windowLimit[x, max] - windowLimit[x, min]) / (xBrickSize + xBrickSpacing);
            int brickAmount = brickRows * brickColumns;

            int freeSpace = (windowLimit[x, max] - windowLimit[x, min]) - ((brickRows * xBrickSize) + (xBrickSpacing * (brickRows - 1)));

            string brickText = string.Empty;

            int[,] posAxis = new int[cursorAmount, 2];
            int[,] cursorVelocity = new int[cursorAmount, 2];
            int[,] scoreBricks = new int[brickAmount, 2];
            int[] cursorColor = new int[cursorAmount];

            bool isActive = true;
            bool isRunning = true;

            bool[] inactiveCursor = new bool[cursorAmount];
            bool[] hitBricks = new bool[brickAmount];

            Console.CursorVisible = false;  // Gömmer kommandotolkens inbyggda cursor.

            while (isRunning)
            {
                for (int i = 0; i < cursorAmount; i++)
                {
                    inactiveCursor[i] = true;
                }

                for (int i = 0; i <= yBorder; i++)     // Ritar "väggarna" i konsolen.
                {
                    Console.SetCursorPosition(xBorder, i);
                    Console.Write("|");
                    Console.SetCursorPosition(windowLimit[x, min], i);
                    Console.Write("|");
                }

                for (int i = 0; i < brickColumns; i++)
                {
                    scoreBricks[brickRows * i, x] = windowLimit[x, min] + (freeSpace / 2);
                    scoreBricks[brickRows * i, y] = i;
                }

                for (int i = 1; i < brickAmount; i++)
                {
                    if (i % brickRows == 0)
                    {
                        yBrickAxis++;
                        i++;
                    }

                    scoreBricks[i, x] = scoreBricks[(i-1), x] + xBrickSize + xBrickSpacing;
                    scoreBricks[i, y] = yBrickAxis;
                }

                for (int i = 1; i <= xBrickSize; i++)
                {
                    brickText += "-";
                }

                for (int i = 0; i < brickAmount; i++)
                {
                    Console.SetCursorPosition(scoreBricks[i, x], scoreBricks[i, y]);
                    Console.WriteLine(brickText);
                }

                while (isActive)
                {
                    for (int i = 0; i < cursorAmount; i++)
                    {

                        if (inactiveCursor[i])
                        {
                            posAxis[i, x] = rnd.Next(windowLimit[x, min], windowLimit[x, max]);
                            posAxis[i, y] = rnd.Next((windowLimit[y, min] + brickColumns), windowLimit[y, max]);
                            cursorColor[i] = rnd.Next(1, 16);
                            cursorVelocity[i, x] = RandomizeVelocity(cursorVelocity[i, x]);
                            cursorVelocity[i, y] = RandomizeVelocity(cursorVelocity[i, y]);
                            inactiveCursor[i] = false;
                        }

                        if (cursorVelocity[i, y] == 0)
                        {
                            inactiveCursor[i] = true;
                        }
                    }

                    if (((xPaddle + paddleSize) + paddleVelocity) > windowLimit[x, max])      // Kollar om plattan går out of bounds.
                    {
                        paddleVelocity = -paddleVelocity;
                        xPaddle = windowLimit[x, max] - paddleSize;
                    }
                    else if ((xPaddle + paddleVelocity) <= windowLimit[x, min])
                    {
                        paddleVelocity = -paddleVelocity;
                        xPaddle = windowLimit[x, min];
                    }

                    xPaddle += paddleVelocity;     // Beräknar plattans position.

                    for (int i = 0; i < cursorAmount; i++)
                    {
                        if (!inactiveCursor[i])      // Jag har använt en boolean vid namnet showCursor för att undvika att beräkna tecken som inte visas.
                        {
                            if ((posAxis[i, y] + cursorVelocity[i, y]) >= windowLimit[y, max])
                            {
                                // Kollar om tecknets x värde är inom plattans intervall.
                                if (((posAxis[i, x] + cursorVelocity[i, x]) >= xPaddle) && (posAxis[i, x] + cursorVelocity[i, x]) <= (xPaddle + paddleSize))
                                {
                                    cursorVelocity[i, y] = -cursorVelocity[i, y];
                                    posAxis[i, y] = windowLimit[y, max];
                                }
                                else
                                {
                                    cursorVelocity[i, y] = 0;
                                    posAxis[i, y] = yBorder;
                                }
                            }
                            else if ((posAxis[i, y] + cursorVelocity[i, y]) < windowLimit[y, min])
                            {
                                cursorVelocity[i, y] = -cursorVelocity[i, y];
                                posAxis[i, y] = windowLimit[y, min];
                            }

                            if ((posAxis[i, x] + cursorVelocity[i, x]) >= windowLimit[x, max] || (posAxis[i, x] + cursorVelocity[i, x]) <= windowLimit[x, min])
                            {
                                cursorVelocity[i, x] = -cursorVelocity[i, x];
                            }

                            posAxis[i, x] += cursorVelocity[i ,x];
                            posAxis[i, y] += cursorVelocity[i, y];
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
                            Console.SetCursorPosition(posAxis[i, x], posAxis[i, y]);
                            Console.ForegroundColor = (ConsoleColor)cursorColor[i];
                            Console.Write("X");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }

                    Thread.Sleep(30);

                    for (int i = 0; i < cursorAmount; i++)
                    {
                        if (!inactiveCursor[i])
                        {
                            Console.SetCursorPosition(posAxis[i, x], posAxis[i, y]);
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
    }
}