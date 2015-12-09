using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WordScrambleClient.WordScrambleGameServiceReference;
using System.ServiceModel;

namespace WordScrambleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            WordScrambleGameClient proxy = new WordScrambleGameClient();
            bool canPlayGame = true;
            Console.WriteLine("Enter your name");
            String playerName = Console.ReadLine();

            if (!proxy.isGameBeingHosted())
            {
                Console.WriteLine("Welcome " + playerName + "! Do you want to host the game?");

                if (Console.ReadLine().ToLower().CompareTo("yes") == 0)
                {
                    Console.WriteLine("Type the word to scramble.");
                    string inputWord = Console.ReadLine();

                    //If the user wants to host, try to host a game
                    try
                    {
                        string scrambledWord = proxy.hostGame(playerName, "", inputWord);
                        canPlayGame = false;
                        Console.WriteLine("You're hosting the game with word '" + inputWord + "' scrambled as '" + scrambledWord + "'");
                        Console.ReadKey();
                    }
                    catch (FaultException<GameAlreadyBeingHostedException> e)
                    {
                        //If there is already a game being hosted by someone else, tell the user who is hosting the game
                        Console.WriteLine("A game is already being hosted by: {0}", e.Detail.currentHost);
                        Console.ReadLine();
                        return;
                    }
                }
            }

            if (canPlayGame)
            {
                Console.WriteLine("Do you want to play the game?");
                if (Console.ReadLine().ToLower().CompareTo("yes") == 0)
                {
                    //Have to declare outside try statement, so it can be used after
                    Word gameWords;
                    
                    try
                    {
                        //Try to join the game
                        gameWords = proxy.join(playerName);
                        Console.WriteLine("Can you unscramble this word? => " + gameWords.scrambledWord);
                    }
                    catch (FaultException<MaxmimumPlayersReachedException> e)
                    {
                        //If the max number of players has been reached, tell the user the max number of players allowed in the game
                        Console.WriteLine("The maxmimum number of players of " + e.Detail.maxPlayers + " has been reached");
                        Console.ReadLine();
                        return;
                    }
                    catch (FaultException<GameNotBeingHostedException>)
                    {
                        //If there is no game being hosted, tell the user that there is no game being hosted
                        Console.WriteLine("There is no game currently being hosted");
                        Console.ReadLine();
                        return;
                    }
                    catch (FaultException<HostCannotJoinGameException>)
                    {
                        //If the host is not able to join the game, tell the user
                        Console.WriteLine("The host is unable to join the game");
                        Console.ReadLine();
                        return;
                    }

                    String guessedWord;
                    bool gameOver = false;
                    while (!gameOver)
                    {
                        guessedWord = Console.ReadLine();

                        try
                        {
                            //The user tries to guess the word
                            gameOver = proxy.guessWord(playerName, guessedWord, gameWords.unscrambledWord);
                        }
                        catch (FaultException<UserNotInGameException>)
                        {
                            //If the user is not in the game, tell them they are not allowed to guess
                            Console.WriteLine("You cannot guess if you are not in the game");
                            Console.ReadLine();
                            return;
                        }

                        if (!gameOver)
                        {
                            Console.WriteLine("Nope, try again...");
                        }
                    }
                    Console.WriteLine("You WON!!!");
                    Console.ReadLine();
                }
            }
        }
    }
}