
namespace Gem
{
    public static class EnumExtensions 
    {
        public static Player GetPlayer(this Player aPlayer, string aName)
        {
            switch (aName)
            {
                case "None":
                case "none":
                    return Player.None;
                case "Environment":
                case "environment":
                    return Player.Environment;
                case "Neutral":
                case "neutral":
                    return Player.Neutral;
                case "PlayerOne":
                case "playerOne":
                case "Playerone":
                case "playerone":
                    return Player.PlayerOne;
                case "PlayerTwo":
                case "playerTwo":
                case "Playertwo":
                case "playertwo":
                    return Player.PlayerTwo;
                case "PlayerThree":
                case "playerThree":
                case "Playerthree":
                case "playerthree":
                    return Player.PlayerThree;
                case "PlayerFour":
                case "playerFour":
                case "Playerfour":
                case "playerfour":
                    return Player.PlayerFour;
                case "PlayerFive":
                case "playerFive":
                case "Playerfive":
                case "playerfive":
                    return Player.PlayerFive;
                case "PlayerSix":
                case "playerSix":
                case "Playersix":
                case "playersix":
                    return Player.PlayerSix;
                case "PlayerSeven":
                case "playerSeven":
                case "Playerseven":
                case "playerseven":
                    return Player.PlayerSeven;
                case "PlayerEight":
                case "playerEight":
                case "Playereight":
                case "playereight":
                    return Player.PlayerEight;
            }
            return Player.None;
        }
    }
}