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
            int xAxisMin = 3;
            int xAxisMax = (Console.WindowWidth - 3);
            int yAxisMin = 0;
            int yAxisMax = (Console.WindowHeight - 1);
            int cursorAmount = 5;
            int paddleSize = 3;
            int paddleVelocity = 1;
            int xPaddle = xAxisMin + paddleSize;
            int yPaddle = yAxisMax - 1;

            int[] xAxis = new int[cursorAmount];
            int[] xVelocity = new int[cursorAmount];
            int[] yAxis = new int[cursorAmount];
            int[] yVelocity = new int[cursorAmount];
            int[] cursorColor = new int[cursorAmount];

            bool isMoving = true;
            bool isRunning = true;

            bool[] showCursor = new bool[cursorAmount];

            Console.CursorVisible = false;  // Gömmer kommandotolkens inbyggda cursor.

            while (isRunning)
            {
                for (int i = 0; i < cursorAmount; i++)
                {
                    xAxis[i] = RanNum.Next(xAxisMin, xAxisMax);
                    yAxis[i] = RanNum.Next(yAxisMin, yAxisMax);
                    cursorColor[i] = RanNum.Next(1, 16);
                    xVelocity[i] = RandomizeVelocity(xVelocity[i]);
                    yVelocity[i] = RandomizeVelocity(yVelocity[i]);
                    showCursor[i] = true;
                }

                for (int i = 0; i <= yAxisMax; i++)     // Ritar "väggarna" i konsolen.
                {
                    Console.SetCursorPosition(xAxisMax, i);
                    Console.Write("|");
                    Console.SetCursorPosition(xAxisMin, i);
                    Console.Write("|");
                }

                while (isMoving)
                {
                    if (((xPaddle + paddleSize) + paddleVelocity) >= xAxisMax)      // Kollar om plattan går out of bounds.
                    {
                        paddleVelocity = paddleVelocity * -1;
                        xPaddle = xAxisMax - paddleSize;
                    }
                    else if (((xPaddle - paddleSize) + paddleVelocity) <= xAxisMin)
                    {
                        paddleVelocity = paddleVelocity * -1;
                        xPaddle = xAxisMin + paddleSize;
                    }

                    xPaddle = xPaddle + paddleVelocity;     // Beräknar plattans position.

                    for (int i = 0; i < cursorAmount; i++)
                    {
                        if (showCursor[i])      // Jag har använt en boolean vid namnet showCursor för att undvika att beräkna tecken som inte visas.
                        {
                            if ((yAxis[i] + yVelocity[i]) >= yAxisMax)
                            {
                                // Kollar om tecknets x värde är inom plattans intervall.
                                if ((xAxis[i] + xVelocity[i]) >= (xPaddle - paddleSize) && (xAxis[i] + xVelocity[i]) <= (xPaddle + paddleSize))
                                {
                                    yVelocity[i] = yVelocity[i] * -1;
                                    yAxis[i] = yAxisMax - 1;
                                }
                                else
                                {
                                    yAxis[i] = xAxisMax - 1;
                                    showCursor[i] = false;
                                }
                            }
                            else if ((yAxis[i] + yVelocity[i]) <= yAxisMin)
                            {
                                yVelocity[i] = yVelocity[i] * -1;
                                yAxis[i] = yAxisMin;
                            }

                            if ((xAxis[i] + xVelocity[i]) >= xAxisMax || (xAxis[i] + xVelocity[i]) <= xAxisMin)
                            {
                                xVelocity[i] = xVelocity[i] * -1;
                            }

                            xAxis[i] += xVelocity[i];
                            yAxis[i] += yVelocity[i];
                        }
                    }



                    for (int i = 0; i <= paddleSize; i++)
                    {
                        Console.SetCursorPosition(xPaddle + i, yPaddle);
                        Console.Write("_");
                        Console.SetCursorPosition(xPaddle - i, yPaddle);
                        Console.Write("_");
                    }

                    for (int i = 0; i < cursorAmount; i++)
                    {
                        if (showCursor[i])
                        {
                            Console.SetCursorPosition(xAxis[i], yAxis[i]);
                            Console.ForegroundColor = (ConsoleColor)cursorColor[i];
                            Console.Write("X");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }

                    Thread.Sleep(30);

                    for (int i = 0; i < cursorAmount; i++)
                    {
                        if (showCursor[i])
                        {
                            Console.SetCursorPosition(xAxis[i], yAxis[i]);
                            Console.Write(" ");
                        }
                    }

                    for (int i = 0; i <= paddleSize; i++)
                    {
                        Console.SetCursorPosition(xPaddle + i, yPaddle);
                        Console.Write(" ");
                        Console.SetCursorPosition(xPaddle - i, yPaddle);
                        Console.Write(" ");
                    }
                }
            }
        }

        int RandomizeVelocity(int RandomVelocity)
        {
            RandomVelocity = RanNum.Next(0, 2) * 2 - 1;

            return RandomVelocity;
        }
    }
}