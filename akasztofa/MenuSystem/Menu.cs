using System;
using System.Collections.Generic;

using Hangman.GameSystem;

#nullable enable
namespace Hangman.MenuSystem
{
	/* Menu layout
	 * 
	 * {optional injected code}
	 * ====  MENU NAME  ====
	 * 1. option1
	 * 2. option2
	 * 3. option3
	 * ...
	 * ====  ---------  ====
	 * {optional injected code}
	 * 
	 * ? <input>
	 */

	/// <summary>
	/// Basic menu template
	/// </summary>
	public abstract class Menu
	{
		protected readonly string				m_menuName;
		protected readonly List<string> m_options;
		protected readonly Game					m_game;
		protected int										m_selected;

		public Menu(string menuName, List<string> options, Game game)
		{
			m_menuName = menuName;
			m_options = options;
			m_game = game;
		}

		/// <summary>
		/// Draws the menu, with the optional injected code before or after the menu drawing
		/// </summary>
		public virtual void Draw()
		{
			bool noError = false;
			bool fisrtPass = true;

			while (!noError)
			{
				Console.Clear();

				InjectBeforeMenu();

				Console.WriteLine($"====  {m_menuName}  ====");

				for (int i = 0; i < m_options.Count; i++)
					Console.WriteLine($"{i + 1}. {m_options[i]}");

				Console.Write("====  ");

				for (int i = 0; i < m_menuName.Length; i++)
					Console.Write("-");

				Console.WriteLine("  ====\n");

				InjectAfterMenu();

				if (!fisrtPass)
					Console.WriteLine("Error!");

				Console.Write("? ");

				noError = int.TryParse(Console.ReadLine(), out m_selected) && m_selected > 0 && m_selected <= m_options.Count;
				fisrtPass = false;
			}

			OptionHandler();
		}

		/// <summary>
		/// Virtual function to inject code before the menu is drawn
		/// </summary>
		protected virtual void InjectBeforeMenu() { }
		/// <summary>
		/// Virtual function to inject code inbetween the menu being drawn and the user input is asked
		/// </summary>
		protected virtual void InjectAfterMenu() { }
		/// <summary>
		/// Abstract function to handle the menu choices
		/// </summary>
		protected abstract void OptionHandler();
	}
}
