using Hangman.GameSystem;

#nullable enable
namespace Hangman
{
	class Program
	{
		/// <summary>
		/// Main function initializes the game object and opens the main menu
		/// </summary>
		static void Main()
		{
			Game game = new Game();
			game.MainMenu();
		}
	}
}
