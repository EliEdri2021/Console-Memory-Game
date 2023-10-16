using System;

namespace Ex02
{
    internal class Program
    {
        public static void Main()
        {
            System.Console.WriteLine("Please enter Player 1's name");
            string playerOneName = System.Console.ReadLine();

            System.Console.WriteLine("Will you be playing against another huamn? (true/false)");
            bool isSeccondPlayerHuman = getBoolInputFromUser();

            string seccondPlayerName = null;
            if (isSeccondPlayerHuman)
            {
                System.Console.WriteLine("Please enter Player 2's name");
                seccondPlayerName = System.Console.ReadLine();
            }

            Game game = new Game(playerOneName, !isSeccondPlayerHuman, seccondPlayerName);

            game.Start();
            bool wantToPlay = true;
            while (!game.IsStopped() && wantToPlay)
            {
                Console.WriteLine("Do you want to play again?");
                wantToPlay = getBoolInputFromUser();

                if (wantToPlay)
                {
                    game.Start();
                }
            }

            System.Console.WriteLine("Program finished!");
            System.Console.ReadLine();
        }

        private static bool getBoolInputFromUser()
        {
            string strBoolInput = Console.ReadLine();
            bool resultBool;
            while(!bool.TryParse(strBoolInput, out resultBool))
            {
                System.Console.WriteLine("I didn't get that. let's try again.");
                strBoolInput = System.Console.ReadLine();
            }

            return resultBool;
        }
    }
}
