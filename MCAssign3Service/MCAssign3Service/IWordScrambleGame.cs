using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace MCAssign3Service
{
    [ServiceContract]

    public interface IWordScrambleGame
    {
        // Returns true if the game is already being hosted or false otherwise
        [OperationContract]
        bool isGameBeingHosted();


        // User ‘userName’ tries to host the game with word ‘wordToScramble’
        // The function returns the name of the person hosting the game
        // Exception: game is already being hosted by someone else
        [FaultContract(typeof(GameAlreadyBeingHostedException))]
        [OperationContract]
        string hostGame(String userName, string hostAddress, String wordToScramble);
        

        // Player ‘playerName’ tries to join the game
        // The function returns a Word object containing the host’s (un)scrambled words
        // Exception: maximum number of players reached
        // Exception: host cannot join the game
        // Exception: nobody is hosting the game
        [FaultContract(typeof(MaxmimumPlayersReachedException))]
        [FaultContract(typeof(HostCannotJoinGameException))]
        [FaultContract(typeof(GameNotBeingHostedException))]
        [OperationContract]
        Word join(string playerName);


        // Player ‘playerName’ guesses word ‘guessedWord’ compared with word ‘unscrambledWord’
        // Returns true if ‘guessedWord’ is identical to ‘unscrambledWord’ or false otherwise
        // The function returns the name of the person hosting the game
        // Exception: user is not playing the game
        [FaultContract(typeof(UserNotInGameException))]
        [OperationContract]
        bool guessWord(string playerName, string guessedWord, string unscrambledWord);

    }

    [DataContract]
    public class Word
    {
        [DataMember]
        public string unscrambledWord; // word typed by the game’s host

        [DataMember]
        public string scrambledWord;
    }

    [DataContract]
    public class GameAlreadyBeingHostedException
    {
        //Tell the user who is already hosting the game
        [DataMember]
        public string currentHost;

        [DataMember]
        public string reason;
    }

    [DataContract]
    public class GameNotBeingHostedException
    {
        [DataMember]
        public string reason;
    }

    [DataContract]
    public class MaxmimumPlayersReachedException
    {
        //Tell the user how many players are allowed in a game
        [DataMember]
        public int maxPlayers;

        [DataMember]
        public string reason;
    }

    [DataContract]
    public class HostCannotJoinGameException
    {
        [DataMember]
        public string reason;
    }

    [DataContract]
    public class UserNotInGameException
    {
        [DataMember]
        public string reason;
    }
}