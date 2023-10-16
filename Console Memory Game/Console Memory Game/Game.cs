using System;

namespace Ex02
{
    using System.Threading;

    internal class Game
    {
        private readonly string r_PlayerOneName;
        private readonly bool r_PlayerTwoAi;
        private readonly string r_PlayerTwoName;

        private GameBoard m_ActiveGameBoard;
        private bool m_StopGame;

        public Game(string i_PlayerOneName, bool i_PlayerTwoAi, string i_PlayerTwoName)
        {
            this.r_PlayerOneName = i_PlayerOneName;
            this.r_PlayerTwoAi = i_PlayerTwoAi;
            this.r_PlayerTwoName = i_PlayerTwoName;
            this.m_StopGame = false;
        }

        public void Start()
        {
            this.m_ActiveGameBoard = new GameBoard(inputRowAmount(), inputColumnAmount()); // 4

            Player playerOne = new Player(this.r_PlayerOneName, false);
            Player playerTwo = new Player(this.r_PlayerTwoName, r_PlayerTwoAi);

            Player[] players = { playerOne, playerTwo };

            int activePlayerIndex = 0;
            int? aiPlayerIndex = null;

            if (r_PlayerTwoAi)
            {
                aiPlayerIndex = 1;
            }

            while (!this.m_ActiveGameBoard.IsGameOver() && !this.m_StopGame)
            {
                ConsoleUtils.Screen.Clear();

                if (activePlayerIndex >= players.Length)
                {
                    activePlayerIndex = 0;
                }

                Index indexA;
                Index indexB;
                bool getsAPoint;

                System.Console.WriteLine(players[activePlayerIndex].GetDisplayName() + " Is up next!");

                if (!players[activePlayerIndex].GetAiMode())
                {
                    this.m_ActiveGameBoard.ShowAll(); // 5
                    indexA = this.getIndexFromUser(); // 6
                    ConsoleUtils.Screen.Clear();

                    this.m_ActiveGameBoard.ShowRevealOne(indexA);
                    Thread.Sleep(2000);
                    ConsoleUtils.Screen.Clear();

                    indexB = this.getIndexFromUser();
                    this.m_ActiveGameBoard.ShowRevealTwo(indexA, indexB);
                    Thread.Sleep(2000);
                    ConsoleUtils.Screen.Clear();

                    getsAPoint = this.m_ActiveGameBoard.AttemptSolve(indexA, indexB);
                }
                else
                {
                    System.Console.WriteLine("AI IS PLAYING");
                    indexA = players[activePlayerIndex].GetAi().GenerateMove(ref this.m_ActiveGameBoard);
                    this.m_ActiveGameBoard.ShowRevealOne(indexA);
                    Thread.Sleep(2000);
                    ConsoleUtils.Screen.Clear();

                    players[activePlayerIndex].GetAi().CompleteMove(
                        ref this.m_ActiveGameBoard,
                        indexA,
                        this.m_ActiveGameBoard.GetValueAtIndex(indexA));
                    indexB = players[activePlayerIndex].GetAi().GenerateMove(ref this.m_ActiveGameBoard);
                    this.m_ActiveGameBoard.ShowRevealTwo(indexA, indexB);
                    Thread.Sleep(2000);
                    ConsoleUtils.Screen.Clear();

                    getsAPoint = this.m_ActiveGameBoard.AttemptSolve(indexA, indexB);
                }

                if (aiPlayerIndex.HasValue && !this.m_StopGame)
                {
                    players[aiPlayerIndex.GetValueOrDefault()].GetAi().ConcludeRound(
                        indexA,
                        this.m_ActiveGameBoard.GetValueAtIndex(indexA),
                        indexB,
                        this.m_ActiveGameBoard.GetValueAtIndex(indexB),
                        getsAPoint);
                }

                if (getsAPoint)
                {
                    System.Console.WriteLine("WHOA! {0} GETS A POINT!", players[activePlayerIndex].GetDisplayName());
                    players[activePlayerIndex].AddPoint();
                    Thread.Sleep(2000);
                }

                activePlayerIndex += 1; // incrementation
            }
        }

        public bool IsStopped()
        {
            return this.m_StopGame;
        }

        private int getIntInput()
        {
            System.Console.Write("Please input the number:");
            int resultIntInput;
            string strUserInput = getUserInput();

            // while user input can't be parsed to int, will try again.
            while (!int.TryParse(strUserInput, out resultIntInput) && !this.m_StopGame)
            {
                System.Console.Write(
                    "{0}Your input wasn't an integer number. Try again:   ",
                    System.Environment.NewLine);
                strUserInput = getUserInput();
            }

            return resultIntInput;
        }

        private string getUserInput()
        {
            string inputFromUser = Console.ReadLine();
            if (inputFromUser == "Q")
            {
                this.m_StopGame = true;
                this.m_ActiveGameBoard.Stop();
            }
            return inputFromUser;
        }

        private char getCharInput()
        {
            System.Console.Write("Please input a single character:");
            char resultCharUserInput;

            string strUserInput = getUserInput();

            while (!char.TryParse(strUserInput, out resultCharUserInput))
            {
                System.Console.WriteLine("Your input wasn't a single letter!"); // MAKE THIS OUTPUT BETTER
                strUserInput = getUserInput();
            }

            return resultCharUserInput;
        }

        private Index getIndexFromUser()
        {
            System.Console.WriteLine("Please Input An index (Column and Row)");
            Index resultingIndex = GameBoard.sr_PassIndex;

            if (!this.m_StopGame)
            {
                char charInput = this.getCharInput();
                if(!this.m_StopGame)
                {
                    int intInput = this.getIntInput();
                    resultingIndex = new Index(charInput, intInput);
                }
            }
            else
            {
                resultingIndex = GameBoard.sr_PassIndex;
            }

            while (!this.m_ActiveGameBoard.IsValidIndexToShow(resultingIndex) && !this.m_StopGame)
            {
                System.Console.WriteLine("No such index exsists. Let's try that again!");
                char charInput = this.getCharInput();
                if (!this.m_StopGame)
                {
                    int intInput = this.getIntInput();
                    resultingIndex = new Index(charInput, intInput);
                }
            }
            System.Console.WriteLine("Got it!");
            return resultingIndex;
        }

        private int inputRowAmount()
        {
            System.Console.WriteLine("Please enter column amount:");

            int inputColumnAmount = this.getIntInput();

            while (!GameBoard.IsValidRowAmount(inputColumnAmount))
            {
                System.Console.WriteLine("Your input wasn't with in bounds. Try again:");  // MAKE THIS OUTPUT BETTER
                inputColumnAmount = getIntInput();
            }

            return inputColumnAmount;
        }

        private int inputColumnAmount()
        {
            System.Console.WriteLine("Please enter row amount:");

            int inputRowAmount = getIntInput();

            while (!GameBoard.IsValidColumnAmount(inputRowAmount))
            {
                System.Console.WriteLine("Your input wasn't with in bounds. Try again:"); // MAKE THIS OUTPUT BETTER
                inputRowAmount = getIntInput();
            }

            return inputRowAmount;
        }
    }
}