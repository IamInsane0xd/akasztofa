using System;
using System.Collections.Generic;
using System.Threading;

using Hangman.GameSystem;

#nullable enable
namespace Hangman.MenuSystem
{
	/// <summary>
	/// Child class for the main menu
	/// </summary>
	public class MainMenu : Menu
	{
		public MainMenu(Game game) : base("Main Menu", new List<string> { "Start", "Change Difficulty", "Exit" }, game)
		{
		}

		protected override void OptionHandler()
		{
			switch (m_selected)
			{
			case 1:
				break;

			case 2:
				m_game.DifficultySelect();
				break;

			case 3:
				Console.Clear();

				Console.ForegroundColor = ConsoleColor.DarkGreen;
				Console.WriteLine("Goodbye!");
				Console.ResetColor();

				Thread.Sleep(2000);
				Console.Clear();
				Environment.Exit(0);
				break;
			}
		}
	}
}
