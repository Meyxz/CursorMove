using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CursorMove
{
    class MyProgram
    {
        Random RanNum = new Random();

        public void Run()
        {
            /* Först kollar jag dimensionerna av konsol applikationen och stoppar max och minvärde som liknar ett matematisk koordinatsystem eller en schackbräda.
             * (x går från 0 till maxvärde (Console.WindowWidth) som representerar vänster till höger i konsolen.)
             * (y går från 0 till maxvärde (Console.WindowHeight) som representerar höjden där 0 är längst upp och när värdet stiger så flyttas den neråt.)
             */
            int xAxisMax = (Console.WindowWidth - 3);
            int yAxisMax = (Console.WindowHeight - 1);
            int xAxisMin = 3;
            int yAxisMin = 0;
            int cursorAmount = 5;
            int paddleSize = 3;
            int xPaddle = xAxisMin;
            int[] xAxis = new int[cursorAmount];
            int[] yAxis = new int[cursorAmount];
            int[] cursorColor = new int[cursorAmount];
            bool isMoving = true;
            bool isRunning = true;
            bool isPaddleReverse = false;
            bool[] reverseXAxis = new bool[cursorAmount];
            bool[] reverseYAxis = new bool[cursorAmount];
            bool[] paddleHit = new bool[cursorAmount];

            Console.CursorVisible = false;  // Gömmer kommandotolkens inbyggda cursor.

            while (isRunning)
            {
                for (int i = 0; i < cursorAmount; i++) // Random x och y värde, färg och riktning.
                {
                    xAxis[i] = RanNum.Next(xAxisMin, xAxisMax);
                    yAxis[i] = RanNum.Next(yAxisMin, yAxisMax);
                    cursorColor[i] = RanNum.Next(1, 16);
                    reverseXAxis[i] = RandomDirection(reverseXAxis[i]);
                    reverseYAxis[i] = RandomDirection(reverseYAxis[i]);
                }

                for (int i = 0; i <= yAxisMax; i++)
                {
                    Console.SetCursorPosition(xAxisMax, i);
                    Console.WriteLine("|");
                    Console.SetCursorPosition((xAxisMin - 1), i);
                    Console.WriteLine("|");
                }

                while (isMoving)
                {
                    for (int i = 0; i < cursorAmount; i++)
                    {
                        reverseYAxis[i] = IsVerticalReversed(yAxis[i], yAxisMax, yAxisMin, reverseYAxis[i]);
                        reverseXAxis[i] = IsHorizontalReversed(xAxis[i], xAxisMax, xAxisMin, reverseXAxis[i]);

                        if (reverseYAxis[i])
                        {
                            yAxis[i]--;
                        }
                        else
                        {
                            yAxis[i]++;
                        }

                        if (reverseXAxis[i])
                        {
                            xAxis[i]--;
                        }
                        else
                        {
                            xAxis[i]++;
                        }

                        if (isPaddleReverse)
                        {
                            xPaddle--;
                        }
                        else
                        {
                            xPaddle++;
                        }
                    }

                    for (int i = 0; i <= paddleSize; i++)
                    {
                        Console.SetCursorPosition((xPaddle + i), yAxisMax);
                        Console.WriteLine("_");
                        Console.SetCursorPosition((xPaddle - i), yAxisMax);
                        Console.WriteLine("_");
                    }

                    for (int print = 0; print < cursorAmount; print++)
                    {
                        Console.SetCursorPosition(xAxis[print], yAxis[print]);
                        Console.ForegroundColor = (ConsoleColor)cursorColor[print];
                        Console.Write("X");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    Thread.Sleep(30);

                    for (int print = 0; print < cursorAmount; print++)
                    {
                        Console.SetCursorPosition(xAxis[print], yAxis[print]);
                        Console.Write(" ");
                    }

                    for (int i = 0; i <= paddleSize; i++)
                    {
                        Console.SetCursorPosition((xPaddle + i), yAxisMax);
                        Console.WriteLine(" ");
                        Console.SetCursorPosition((xPaddle - i), yAxisMax);
                        Console.WriteLine(" ");
                    }
                }
            }
        }

        bool IsVerticalReversed(int yPos, int yPosMax, int yPosMin, bool verticalReversed)
        {
            if (yPos == yPosMax)
            {
                verticalReversed = true;
            }
            else if (yPos == yPosMin)
            {
                verticalReversed = false;
            }

            return verticalReversed;
        }

        bool IsHorizontalReversed(int xPos, int xPosMax, int xPosMin, bool horizontalReversed)
        {
            if (xPos == (xPosMax - 1))
            {
                horizontalReversed = true;
            }
            else if (xPos == xPosMin)
            {
                horizontalReversed = false;
            }

            return horizontalReversed;
        }

        bool RandomDirection(bool RandomBool)
        {
            if (RanNum.NextDouble() >= 0.5)
            {
                RandomBool = true;
            }
            else
            {
                RandomBool = false;
            }

            return RandomBool;
        }
    }
}