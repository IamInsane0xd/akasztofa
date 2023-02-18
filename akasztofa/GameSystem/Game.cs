using System;
using System.Collections.Generic;

using Hangman.MenuSystem;
using Hangman.FileSystem;
using System.Threading;

#nullable enable
namespace Hangman.GameSystem
{
	/* "Animation" frames
	* 
	* ============================ 1
	* 
	* 
	* 
	* 
	* 
	* 
	*  ────┴───────────
	* ============================ 2
	*      ┌
	*      │
	*      │
	*      │
	*      │
	*      │
	*  ────┴───────────
	* ============================ 3
	*      ┌─────┐
	*      │
	*      │
	*      │
	*      │
	*      │
	*  ────┴───────────
	* ============================ 4
	*      ┌─────┐
	*      │     O
	*      │
	*      │
	*      │
	*      │
	*  ────┴───────────
	* ============================ 5
	*      ┌─────┐
	*      │     O
	*      │     |
	*      │
	*      │
	*      │
	*  ────┴───────────
	* ============================ 6
	*      ┌─────┐
	*      │     O
	*      │    /|
	*      │
	*      │
	*      │
	*  ────┴───────────
	* ============================ 7
	*      ┌─────┐
	*      │     O
	*      │    /|\
	*      │
	*      │
	*      │
	*  ────┴───────────
	* ============================ 8
	*      ┌─────┐
	*      │     O
	*      │    /|\
	*      │     |
	*      │
	*      │
	*  ────┴───────────
	* ============================ 9
	*      ┌─────┐
	*      │     O
	*      │    /|\
	*      │     |
	*      │    /
	*      │
	*  ────┴───────────
	* ============================ 10
	*      ┌─────┐
	*      │     O
	*      │    /|\
	*      │     |
	*      │    / \
	*      │
	*  ────┴───────────
	* ============================
	*/

	/// <summary>
	/// "Main" class containing the game's core logic
	/// </summary>
	public partial class Game
	{
		public static readonly string m_allowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

		private readonly Dictionary<State, Menu?> m_gameStates;
		private State?														m_currentState;
		private State?														m_previousState;
		private Difficulty                        m_currentDifficulty;
		private readonly HangmanWords							m_words;
		private string														m_correctWord;
		private string                            m_displayedWord;
		private readonly List<char>								m_guessed;
		private readonly List<char>								m_wrong;
		private readonly List<char>								m_correct;
		private bool                              m_error;
		private string                            m_errorMsg;
		private bool                              m_gameEnd;
		private bool                              m_win;

		public Game()
		{
			m_gameStates = new Dictionary<State, Menu?>
			{
				{ State.MainMenu, new MainMenu(this) },
				{ State.DifficultyMenu, new DifficultyMenu(this) },
				{ State.Game, null }
			};

			m_currentState = null;
			m_previousState = null;
			m_currentDifficulty = Difficulty.Medium;
			m_words = new HangmanWords("Words.txt");
			m_correctWord = "";
			m_displayedWord = "";
			m_guessed = new List<char>();
			m_wrong = new List<char>();
			m_correct = new List<char>();
			m_error = false;
			m_errorMsg = "";
			m_gameEnd = false;
			m_win = false;
		}

		public State? PreviousState { get { return m_previousState; } }
		public Difficulty CurrentDifficulty { get { return m_currentDifficulty; } set { m_currentDifficulty = value; } }

		/// <summary>
		/// Changes the current state to the main menu
		/// </summary>
		public void MainMenu() => ChangeCurrentState(State.MainMenu);
		/// <summary>
		/// Changes the current state to the difficulty select menu
		/// </summary>
		public void DifficultySelect() => ChangeCurrentState(State.DifficultyMenu);

		/// <summary>
		/// Change the state to any given state, <see cref="null" /> being the game itself
		/// </summary>
		public void ChangeCurrentState(State state)
		{
			if (m_currentState != state)
				m_previousState = m_currentState;

			m_currentState = state;

			Menu? newState = m_gameStates[state];

			if (newState != null)
				newState.Draw();

			if (!m_gameEnd)
				Start();

			MainMenu();
		}

		/// <summary>
		/// Start the game loop and handle win and lose screens
		/// </summary>
		private void Start()
		{
			m_correctWord = m_words.GetRandomWord(m_currentDifficulty)!;

			while (!m_gameEnd)
			{
				Draw();
				Guess();
				CheckEnd();

				if (!m_gameEnd)
					continue;

				Thread.Sleep(1000);
				Console.Clear();
				Console.WriteLine("---  GAME OVER  ---");

				if (!m_win)
				{
					Console.ForegroundColor = ConsoleColor.DarkRed;
					Console.WriteLine("\n\n\tYOU LOSE!");
					Console.ResetColor();
					Console.WriteLine($"\n\nThe word was: {m_correctWord}");
					continue;
				}

				Console.ForegroundColor = ConsoleColor.DarkGreen;
				Console.WriteLine("\n\n\tYOU WIN!");
				Console.ResetColor();
			}

			Thread.Sleep(3000);

			Reset();
		}

		/// <summary>
		/// Reset the game so it can be played again
		/// </summary>
		private void Reset()
		{
			m_guessed.Clear();
			m_wrong.Clear();
			m_correct.Clear();
			m_error = false;
			m_gameEnd = false;
			m_win = false;
		}

		/// <summary>
		/// Checks if any game end condition is met and decides if the player won or not
		/// </summary>
		private void CheckEnd()
		{
			if (m_wrong.Count >= 10)
			{
				m_gameEnd = true;
				return;
			}

			if (!m_displayedWord.Contains('_'))
			{
				m_gameEnd = true;
				m_win = true;
			}
		}

		/// <summary>
		/// Inputs a character and checks if it's a valid guess then decides if it's correct or not
		/// </summary>
		private void Guess()
		{
			if (!char.TryParse(Console.ReadLine(), out char guess))
			{
				m_error = true;
				m_errorMsg = "Only input one letter at a time.";
				return;
			}

			if (!m_allowedCharacters.Contains(guess))
			{
				m_error = true;
				m_errorMsg = "Only letters from the latin alphabet are allowed.";
				return;
			}

			if (m_guessed.Contains(guess))
			{
				m_error = true;
				m_errorMsg = "You already used that letter.";
				return;
			}

			if (m_correctWord.Contains(guess))
				m_correct.Add(guess);

			else
				m_wrong.Add(guess);

			m_guessed.Add(guess);
		}

		/// <summary>
		/// Draws the game to the console, and any error messages if any
		/// </summary>
		private void Draw()
		{
			int numOfWrong = m_wrong.Count;
			string wrongLetters = string.Join(", ", m_wrong);
			List<char> letters = new List<char>();

			foreach (char letter in m_correctWord)
				letters.Add(m_correct.Contains(letter) ? letter : '_');

			m_displayedWord = string.Join(" ", letters);

			Console.Clear();
			Console.WriteLine("\n");

			switch (numOfWrong)
			{
			case 0:
				Console.WriteLine("\t                 ");
				Console.WriteLine("\t                 ");
				Console.WriteLine($"\t                 \talready guessed but wrong: {wrongLetters}");
				Console.WriteLine("\t                 \t");
				Console.WriteLine("\t                 ");
				Console.WriteLine($"\t                 \tyour word: {m_displayedWord}");
				Console.WriteLine("\t                 ");
				break;

			case 1:
				Console.WriteLine("\t                 ");
				Console.WriteLine("\t                 ");
				Console.WriteLine($"\t                 \talready guessed but wrong: {wrongLetters}");
				Console.WriteLine("\t                 ");
				Console.WriteLine("\t                 ");
				Console.WriteLine($"\t                 \tyour word: {m_displayedWord}");
				Console.WriteLine("\t ────┴───────────");
				break;

			case 2:
				Console.WriteLine("\t     ┌           ");
				Console.WriteLine("\t     │           ");
				Console.WriteLine($"\t     │           \talready guessed but wrong: {wrongLetters}");
				Console.WriteLine("\t     │           ");
				Console.WriteLine("\t     │           ");
				Console.WriteLine($"\t     │           \tyour word: {m_displayedWord}");
				Console.WriteLine("\t ────┴───────────");
				break;

			case 3:
				Console.WriteLine("\t     ┌─────┐     ");
				Console.WriteLine("\t     │           ");
				Console.WriteLine($"\t     │           \talready guessed but wrong: {wrongLetters}");
				Console.WriteLine("\t     │           ");
				Console.WriteLine("\t     │           ");
				Console.WriteLine($"\t     │           \tyour word: {m_displayedWord}");
				Console.WriteLine("\t ────┴───────────");
				break;

			case 4:
				Console.WriteLine("\t     ┌─────┐     ");
				Console.WriteLine("\t     │     O     ");
				Console.WriteLine($"\t     │           \talready guessed but wrong: {wrongLetters}");
				Console.WriteLine("\t     │           ");
				Console.WriteLine("\t     │           ");
				Console.WriteLine($"\t     │           \tyour word: {m_displayedWord}");
				Console.WriteLine("\t ────┴───────────");
				break;

			case 5:
				Console.WriteLine("\t     ┌─────┐     ");
				Console.WriteLine("\t     │     O     ");
				Console.WriteLine($"\t     │     |     \talready guessed but wrong: {wrongLetters}");
				Console.WriteLine("\t     │           ");
				Console.WriteLine("\t     │           ");
				Console.WriteLine($"\t     │           \tyour word: {m_displayedWord}");
				Console.WriteLine("\t ────┴───────────");
				break;

			case 6:
				Console.WriteLine("\t     ┌─────┐     ");
				Console.WriteLine("\t     │     O     ");
				Console.WriteLine($"\t     │    /|     \talready guessed but wrong: {wrongLetters}");
				Console.WriteLine("\t     │           ");
				Console.WriteLine("\t     │           ");
				Console.WriteLine($"\t     │           \tyour word: {m_displayedWord}");
				Console.WriteLine("\t ────┴───────────");
				break;

			case 7:
				Console.WriteLine("\t     ┌─────┐     ");
				Console.WriteLine("\t     │     O     ");
				Console.WriteLine($"\t     │    /|\\    \talready guessed but wrong: {wrongLetters}");
				Console.WriteLine("\t     │           ");
				Console.WriteLine("\t     │           ");
				Console.WriteLine($"\t     │           \tyour word: {m_displayedWord}");
				Console.WriteLine("\t ────┴───────────");
				break;

			case 8:
				Console.WriteLine("\t     ┌─────┐     ");
				Console.WriteLine("\t     │     O     ");
				Console.WriteLine($"\t     │    /|\\    \talready guessed but wrong: {wrongLetters}");
				Console.WriteLine("\t     │     |     ");
				Console.WriteLine("\t     │           ");
				Console.WriteLine($"\t     │           \tyour word: {m_displayedWord}");
				Console.WriteLine("\t ────┴───────────");
				break;

			case 9:
				Console.WriteLine("\t     ┌─────┐     ");
				Console.WriteLine("\t     │     O     ");
				Console.WriteLine($"\t     │    /|\\    \talready guessed but wrong: {wrongLetters}");
				Console.WriteLine("\t     │     |     ");
				Console.WriteLine("\t     │    /      ");
				Console.WriteLine($"\t     │           \tyour word: {m_displayedWord}");
				Console.WriteLine("\t ────┴───────────");
				break;

			case 10:
			default:
				Console.WriteLine("\t     ┌─────┐     ");
				Console.WriteLine("\t     │     O     ");
				Console.WriteLine($"\t     │    /|\\    \talready guessed but wrong: {wrongLetters}");
				Console.WriteLine("\t     │     |     ");
				Console.WriteLine("\t     │    / \\    ");
				Console.WriteLine($"\t     │           \tyour word: {m_displayedWord}");
				Console.WriteLine("\t ────┴───────────");
				break;
			}

			if (m_error)
				Console.WriteLine($"\n\terror: {m_errorMsg}");

			Console.Write("\n\n\t\tguess: ");

			m_error = false;
		}
	}
}
