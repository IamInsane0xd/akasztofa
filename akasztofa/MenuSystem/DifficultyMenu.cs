using System;
using System.Collections.Generic;

using Hangman.GameSystem;

#nullable enable
namespace Hangman.MenuSystem
{
	/// <summary>
	/// Child class for the difficulty selection menu
	/// </summary>
	public class DifficultyMenu : Menu
	{
		public DifficultyMenu(Game game) : base("Select Difficulty", new List<string> { "Easy", "Medium", "Hard", "Extreme", "Back" }, game)
		{
		}

		protected override void InjectBeforeMenu()
		{
			Console.Write("Current difficulty: ");

			switch (m_game.CurrentDifficulty)
			{
			case Game.Difficulty.Easy:
				Console.ForegroundColor = ConsoleColor.Green;
				break;

			case Game.Difficulty.Medium:
				Console.ForegroundColor = ConsoleColor.Yellow;
				break;

			case Game.Difficulty.Hard:
				Console.ForegroundColor = ConsoleColor.Red;
				break;

			case Game.Difficulty.Extreme:
				Console.ForegroundColor = ConsoleColor.Magenta;
				break;
			}

			Console.WriteLine($"{m_game.CurrentDifficulty}\n");
			Console.ResetColor();
		}

		protected override void OptionHandler()
		{
			switch (m_selected)
			{
			case 1:
				m_game.CurrentDifficulty = Game.Difficulty.Easy;
				m_game.DifficultySelect();
				break;

			case 2:
				m_game.CurrentDifficulty = Game.Difficulty.Medium;
				m_game.DifficultySelect();
				break;

			case 3:
				m_game.CurrentDifficulty = Game.Difficulty.Hard;
				m_game.DifficultySelect();
				break;

			case 4:
				m_game.CurrentDifficulty = Game.Difficulty.Extreme;
				m_game.DifficultySelect();
				break;

			case 5:
				if (m_game.PreviousState == null)
					break;

				m_game.ChangeCurrentState((Game.State)m_game.PreviousState);
				break;
			}
		}
	}
}
