using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MCAssign3Service
{
    [ServiceBehavior]
    public class WordScrambleGame : IWordScrambleGame
    {
        // the maximum number of players allowed playing simultaneously
        private const int MAX_PLAYERS = 5;

        // the user hosting the game. If it’s null nobody is hosting the game.
        private static String userHostingTheGame = null;

        // the Word object that contains the scrambled and unscrambled words
        private static Word gameWords;

        // the list of players playing the game
        private static List<String> activePlayers = new List<string>();

        public bool isGameBeingHosted()
        {
            //If there is no host, return false. Otherwise return true
            if (userHostingTheGame == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public string hostGame(string userName, string hostAddress, string wordToScramble)
        {
            //If someone tries to host a game, but there is already a game being hosted, throw an exception
            if (isGameBeingHosted())
            {
                GameAlreadyBeingHostedException fault = new GameAlreadyBeingHostedException();
                fault.reason = "A user is already hosting a game";
                //Tells the user who is currently hosting the game
                fault.currentHost = userHostingTheGame;
                throw new FaultException<GameAlreadyBeingHostedException>(fault);
            }
            else
            {
                //Sets the user as the host
                userHostingTheGame = userName;
                gameWords = new Word();
                gameWords.unscrambledWord = wordToScramble;
                //Scramble the word that was entered
                gameWords.scrambledWord = scrambleWord(wordToScramble);
                return gameWords.scrambledWord;
            }
        }

        public Word join(string playerName)
        {
            //List is 0 indexed, so subtract 1 from the count
            if (activePlayers.Count -1 >= MAX_PLAYERS)
            {
                //If the max number of players has been met, throw an exception
                MaxmimumPlayersReachedException fault = new MaxmimumPlayersReachedException();
                fault.reason = "The maximum number of players has been reached";
                //Tells the user the maximum number of players allowed in the game
                fault.maxPlayers = MAX_PLAYERS;
                throw new FaultException<MaxmimumPlayersReachedException>(fault);
            }
            else if (!isGameBeingHosted())
            {
                //If the user tries to join a game, but no one is currently hosting a game, throw an exception
                GameNotBeingHostedException fault = new GameNotBeingHostedException();
                fault.reason = "There is no game being hosted to join";
                throw new FaultException<GameNotBeingHostedException>(fault);
            }
            else if (userHostingTheGame == null)
            {
                //I have no idea what would cause the host to fail to join the game, or how to check for it
                //So I'm just putting this here for the sake of it being here
                //If somehow there is a game being hosted, but there is no host, throw an exception
                HostCannotJoinGameException fault = new HostCannotJoinGameException();
                fault.reason = "The host failed to join the game";
                throw new FaultException<HostCannotJoinGameException>(fault);
            }
            else
            {
                //Add the player to the active players list
                activePlayers.Add(playerName);
                return gameWords;
            }            
        }

        public bool guessWord(string playerName, string guessedWord, string unscrambledWord)
        {
            if (activePlayers.Contains(playerName))
            {
                //If the player is in the game, check the guessed word
                return guessedWord == unscrambledWord;
            }
            else
            {
                //If the user tries to guess the word, but they are not in the game, throw an exception
                UserNotInGameException fault = new UserNotInGameException();
                fault.reason = "You cannot make a guess if you are not in the game";
                throw new FaultException<UserNotInGameException>(fault);
            }
        }

        private string scrambleWord(string word)
        {
            char[] chars = word.ToArray();
            Random r = new Random(2011);
            for (int i = 0; i < chars.Length; i++)
            {
                int randomIndex = r.Next(0, chars.Length);
                char temp = chars[randomIndex];
                chars[randomIndex] = chars[i];
                chars[i] = temp;
            }
            return new string(chars);
        }
    }
}