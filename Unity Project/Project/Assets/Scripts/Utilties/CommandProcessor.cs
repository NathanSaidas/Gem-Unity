using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#region CHANGE LOG
/* December, 7, 2014 - Nathan Hanlan - Added interface ICommandProcessor and class CommandProcessor
 * 
 */
#endregion

namespace Gem
{
    /// <summary>
    /// Main Interface for Command Processors
    /// </summary>
    public interface ICommandProcessor
    {
        void Process(List<string> aWords, List<string> aLowerWords);
    }

    /// <summary>
    /// An implementation example of ICommandProcessor
    /// </summary>
    public class CommandProcessor : ICommandProcessor
    {
#region COMMAND KEYWORDS
        //3
        public const string COMMAND_SET = "set";
        public const string COMMAND_LOG = "log";
        //4
        public const string COMMAND_SHOW = "show";
        public const string COMMAND_HIDE = "hide";
        public const string COMMAND_HELP = "help";
        public const string COMMAND_LOAD = "load";
        public const string COMMAND_QUIT = "quit";
        public const string COMMAND_KILL = "kill";
        //5
        public const string COMMAND_CLEAR = "clear";
        //6
        public const string COMMAND_RELOAD = "reload";
        //7
        public const string COMMAND_RESTART = "restart";
        public const string COMMAND_CONSOLE = "console";
#endregion
#region CONTEXT KEYWORDS
        //4
        public const string CONTEXT_UNIT = "unit";
        //5
        public const string CONTEXT_WORLD = "world";
        //6
        public const string CONTEXT_PLAYER = "player";
        //11 
        public const string CONTEXT_INTERACTIVE = "interactive";

#endregion


        public virtual void Process(List<string> aWords, List<string> aLowerWords)
        {
                string firstWord = aWords[0];
                switch(firstWord.Length)
                {
                    case 3:
                        HandleCommand3(aWords,aLowerWords);
                        break;
                    case 4:
                        HandleCommand4(aWords,aLowerWords);
                        break;
                    case 5:
                        HandleCommand5(aWords,aLowerWords);
                        break;
                    case 6:
                        HandleCommand6(aWords, aLowerWords);
                        break;
                    case 7:
                        HandleCommand7(aWords, aLowerWords);
                        break;
                    case 8:
                        HandleCommand8(aWords, aLowerWords);
                        break;
                        
                }
        }

        #region HANDLE COMMANDS
        protected virtual void HandleCommand3(List<string> aWords, List<string> aLowerWords)
        {
            
        }
        protected virtual void HandleCommand4(List<string> aWords, List<string> aLowerWords)
        {

        }
        protected virtual void HandleCommand5(List<string> aWords, List<string> aLowerWords)
        {

        }
        protected virtual void HandleCommand6(List<string> aWords, List<string> aLowerWords)
        {

        }
        protected virtual void HandleCommand7(List<string> aWords, List<string> aLowerWords)
        {

        }
        protected virtual void HandleCommand8(List<string> aWords, List<string> aLowerWords)
        {

        }
        #endregion
    }

}