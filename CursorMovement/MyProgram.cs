using System;
using System.Threading;

namespace CursorMovement
{
    class MyProgram
    {
        public void Run()
        {
            CursorMovement();
        }

        void CursorMovement()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            Console.CursorVisible = false;
            const int WIDTH_CONSOLE = 180; // gör bara skärmen lite större
            const int HEIGHT_CONSOLE = 40;
            Console.SetBufferSize(WIDTH_CONSOLE, HEIGHT_CONSOLE);
            Console.SetWindowSize(WIDTH_CONSOLE, HEIGHT_CONSOLE);

            #region Box

            int menuSpeed = 10;
            int xOffset = ((Console.WindowWidth - 1) % 6) / 2;                              //move this junk somewhere else i guess, maybe later when menu
            int yOffset = xOffset / 2;
            int playAreaWidth = Console.WindowWidth - 1 - xOffset * 2;
            int playAreaHeight = Console.WindowHeight - 1 - yOffset * 2;


            Console.BackgroundColor = ConsoleColor.DarkCyan;
            for (int i = 0; i <= playAreaWidth / 2; i++) //menu top
            {
                Console.SetCursorPosition(playAreaWidth / 2 + xOffset - i, yOffset);
                Console.Write("T");
                Console.SetCursorPosition(playAreaWidth / 2 + xOffset + i, yOffset);
                Console.Write("T");
                Thread.Sleep(menuSpeed);
            }

            for (int i = 0; i < HEIGHT_CONSOLE - yOffset; i++) // menu sides
            {
                Console.SetCursorPosition(xOffset, i + yOffset);
                Console.Write("V");
                Console.SetCursorPosition(xOffset + playAreaWidth, i + yOffset);
                Console.Write("V");
                Thread.Sleep(menuSpeed);
            }
            /*
            for (int i = 0; i <= playAreaWidth / 2; i++) // menu bottom
            {
                Console.SetCursorPosition(xOffset + i, playAreaHeight + yOffset);
                Console.Write(" ");
                Console.SetCursorPosition(xOffset + playAreaWidth - i, playAreaHeight + yOffset);
                Console.Write(" ");
                Thread.Sleep(menuSpeed);
            }
            */
            Console.BackgroundColor = ConsoleColor.Black;
            Thread.Sleep(500);

            #endregion Box



            #region Pre-Movement Math

            int cursorAmount = 10;
            int x = 0;                  // ger saker som x och y ett värde som jag kan använde i arrays för att öka läsbarhet
            int y = 1;
            int changeValue = 0;
            int displayPos = 1;
            int actualPos = 2;
            int oldPos = 3;
            int futurePos = 4;
            double[,,] movementValues = new double[cursorAmount, 5, 2];

            bool checkTest = false;

            double movementPerLoop = 0.25;
            for (int i = 0; i < cursorAmount; i++)
            {
                double randomAngle = (int)new Random().Next(0, 91);
                movementValues[i, changeValue, x] = movementPerLoop * Math.Sin(randomAngle * Math.PI / 180) / Math.Sin(90 * Math.PI / 180);
                movementValues[i, changeValue, y] = movementPerLoop * Math.Cos(randomAngle * Math.PI / 180) / Math.Sin(90 * Math.PI / 180) / 2;

                movementValues[i, changeValue, x] = 1;
                movementValues[i, changeValue, y] = 0;


                // delar på 2 eftersom varje text karaktär är dubbelt så hög som bred, allt hade blivit lite platt annars

                if ((int)new Random().Next(0, 2) == 0)
                {
                    movementValues[i, changeValue, x] *= -1;
                }

                if ((int)new Random().Next(0, 2) == 0)
                {
                    movementValues[i, changeValue, y] *= -1; ;
                }


                movementValues[i, displayPos, x] = (int)new Random().Next(xOffset + playAreaWidth / 10, xOffset + playAreaWidth - playAreaWidth / 10);
                movementValues[i, displayPos, y] = (int)new Random().Next(yOffset + playAreaHeight / 4, yOffset + playAreaHeight - playAreaHeight / 4);
                movementValues[i, actualPos, x] = movementValues[i, displayPos, x];
                movementValues[i, actualPos, y] = movementValues[i, displayPos, y];
                movementValues[i, oldPos, x] = movementValues[i, displayPos, x];
                movementValues[i, oldPos, y] = movementValues[i, displayPos, y];
                movementValues[i, futurePos, x] = Math.Round(movementValues[i, actualPos, x] + movementValues[i, changeValue, x], 0, MidpointRounding.AwayFromZero);
                movementValues[i, futurePos, y] = Math.Round(movementValues[i, actualPos, y] + movementValues[i, changeValue, y], 0, MidpointRounding.AwayFromZero);
            }

            #region Paddle

            double[,] paddle = new double[3, 2];
            int paddleLength = playAreaWidth / 6;
            String paddleString = "";
            for (int i = 0; i < paddleLength; i++)
            {
                paddleString += " ";
            }
            paddle[changeValue, x] = 1;
            paddle[changeValue, y] = 1;       // kommer använda den här sen        //kanske

            paddle[displayPos, x] = xOffset + playAreaWidth / 2 - paddleLength / 2;
            paddle[displayPos, y] = yOffset + playAreaHeight - playAreaHeight / 5;
            paddle[actualPos, x] = paddle[displayPos, x];
            paddle[actualPos, y] = paddle[displayPos, y];

            for (int i = 0; i < paddleLength; i++)
            {
                Console.SetCursorPosition(Convert.ToInt32(paddle[displayPos, x]) + i, Convert.ToInt32(paddle[displayPos, y]));
                Console.BackgroundColor = ConsoleColor.White;
                Console.Write(" ");
                Console.BackgroundColor = ConsoleColor.Black;
                Thread.Sleep(10);
            }

            #endregion Paddle

            #endregion Pre-Movement Math


            #region Movement Loops

            bool isRunning = true;
            while (isRunning)
            {
                //moveBar
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey();
                    if (keyInfo.Key == ConsoleKey.RightArrow && paddle[displayPos, x] != xOffset + playAreaWidth - paddleLength)
                    {
                        Console.SetCursorPosition(Convert.ToInt32(paddle[displayPos, x]), Convert.ToInt32(paddle[displayPos, y]));
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write(paddleString);

                        paddle[actualPos, x] += paddle[changeValue, x];
                        paddle[displayPos, x] = Math.Round(paddle[actualPos, x], 0, MidpointRounding.AwayFromZero);

                        Console.SetCursorPosition(Convert.ToInt32(paddle[displayPos, x]), Convert.ToInt32(paddle[displayPos, y]));
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.Write(paddleString);
                    }
                    if (keyInfo.Key == ConsoleKey.LeftArrow && paddle[displayPos, x] != xOffset + 1)
                    {
                        Console.SetCursorPosition(Convert.ToInt32(paddle[displayPos, x]), Convert.ToInt32(paddle[displayPos, y]));
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write(paddleString);

                        paddle[actualPos, x] -= paddle[changeValue, x];
                        paddle[displayPos, x] = Math.Round(paddle[actualPos, x], 0, MidpointRounding.AwayFromZero);

                        Console.SetCursorPosition(Convert.ToInt32(paddle[displayPos, x]), Convert.ToInt32(paddle[displayPos, y]));
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.Write(paddleString);
                    }
                }

                Console.BackgroundColor = ConsoleColor.Black;
                //Cursor movement loop
                for (int i = 0; i < cursorAmount; i++)
                {
                    if (movementValues[i, displayPos, y] != paddle[displayPos, y] && movementValues[i, futurePos, y] == paddle[displayPos, y] && movementValues[i, futurePos, x] >= paddle[displayPos, x] && movementValues[i, futurePos, x] <= paddle[displayPos, x] + paddleLength)
                    {
                        movementValues[i, changeValue, y] *= -1;
                    }

                    if (movementValues[i, displayPos, y] == paddle[displayPos, y] && movementValues[i, futurePos, y] == paddle[displayPos, y] && (movementValues[i, futurePos, x] == paddle[displayPos, x] || movementValues[i, futurePos, x] == paddle[displayPos, x] + paddleLength))
                    {
                        movementValues[i, changeValue, x] *= -1;
                    }


                    if (movementValues[i, futurePos, x] >= xOffset + playAreaWidth || movementValues[i, futurePos, x] <= xOffset) // kollar om den träffar en vägg
                    {
                        movementValues[i, changeValue, x] *= -1;
                    }


                    if (movementValues[i, futurePos, y] <= yOffset) // kollar om den träffar taket
                    {
                        movementValues[i, changeValue, y] *= -1;
                    }

                    //Om cursor "dör" eller hamnar utanför spelområdet.   // borde göra om till method
                    if (movementValues[i, futurePos, y] >= HEIGHT_CONSOLE || movementValues[i, displayPos, x] <= yOffset || movementValues[i, displayPos, x] <= xOffset || movementValues[i, displayPos, x] >= xOffset + playAreaWidth)
                    {
                        Console.SetCursorPosition(Convert.ToInt32(movementValues[i, displayPos, x]), Convert.ToInt32(movementValues[i, displayPos, y]));
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write("G");


                        double randomAngle = (int)new Random().Next(0, 91);
                        movementValues[i, changeValue, x] = movementPerLoop * Math.Sin(randomAngle * Math.PI / 180) / Math.Sin(90 * Math.PI / 180);
                        movementValues[i, changeValue, y] = movementPerLoop * Math.Cos(randomAngle * Math.PI / 180) / Math.Sin(90 * Math.PI / 180) / 2;

                        if ((int)new Random().Next(0, 2) == 0)
                        {
                            movementValues[i, changeValue, x] *= -1;
                        }

                        if ((int)new Random().Next(0, 2) == 0)
                        {
                            movementValues[i, changeValue, y] *= -1; ;
                        }
                        movementValues[i, displayPos, x] = (int)new Random().Next(xOffset + playAreaWidth / 10, xOffset + playAreaWidth - playAreaWidth / 10);
                        movementValues[i, displayPos, y] = (int)new Random().Next(yOffset + playAreaHeight / 4, yOffset + playAreaHeight - playAreaHeight / 4);
                        movementValues[i, actualPos, x] = movementValues[i, displayPos, x];
                        movementValues[i, actualPos, y] = movementValues[i, displayPos, y];
                    }
                    if (!(movementValues[i, oldPos, x] == movementValues[i, displayPos, x] && movementValues[i, oldPos, y] == movementValues[i, displayPos, y]))
                    {
                        Console.SetCursorPosition(Convert.ToInt32(movementValues[i, displayPos, x]), Convert.ToInt32(movementValues[i, displayPos, y]));

                        switch (i % 6)
                        {
                            case 0:
                                Console.ForegroundColor = ConsoleColor.Red;
                                break;
                            case 1:
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                break;
                            case 2:
                                Console.ForegroundColor = ConsoleColor.Green;
                                break;
                            case 3:
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                break;
                            case 4:
                                Console.ForegroundColor = ConsoleColor.Blue;
                                break;
                            case 5:
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                break;
                            default:
                                Console.ForegroundColor = ConsoleColor.White;
                                break;


                        }

                        Console.Write("O");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }

                Thread.Sleep(5);


                for (int i = 0; i < cursorAmount; i++)  // Cursormovement loop
                {
                    if (!(movementValues[i, futurePos, x] == movementValues[i, displayPos, x] && movementValues[i, futurePos, y] == movementValues[i, displayPos, y]))
                    {
                        Console.SetCursorPosition(Convert.ToInt32(movementValues[i, displayPos, x]), Convert.ToInt32(movementValues[i, displayPos, y]));
                        Console.Write("K");
                    }

                    movementValues[i, oldPos, x] = movementValues[i, displayPos, x];
                    movementValues[i, oldPos, y] = movementValues[i, displayPos, y];

                    movementValues[i, actualPos, x] += movementValues[i, changeValue, x];
                    movementValues[i, actualPos, y] += movementValues[i, changeValue, y];

                    movementValues[i, displayPos, x] = Math.Round(movementValues[i, actualPos, x], 0, MidpointRounding.AwayFromZero);
                    movementValues[i, displayPos, y] = Math.Round(movementValues[i, actualPos, y], 0, MidpointRounding.AwayFromZero);

                    movementValues[i, futurePos, x] = Math.Round(movementValues[i, actualPos, x] + movementValues[i, changeValue, x], 0, MidpointRounding.AwayFromZero);
                    movementValues[i, futurePos, y] = Math.Round(movementValues[i, actualPos, y] + movementValues[i, changeValue, y], 0, MidpointRounding.AwayFromZero);
                }

            }
            #endregion Movement Loops


        }



    }
}
