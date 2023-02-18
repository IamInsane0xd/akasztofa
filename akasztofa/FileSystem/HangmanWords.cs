using System;
using System.Collections.Generic;
using System.IO;

using Hangman.GameSystem;

#nullable enable
namespace Hangman.FileSystem
{
	/// <summary>
	/// A child class of <see cref="File" /> to handle reading and parsing the words file for the game
	/// </summary>
	public class HangmanWords : File
	{
		private readonly List<string> m_lines;
		private readonly List<string> m_easy;
		private readonly List<string> m_medium;
		private readonly List<string> m_hard;
		private readonly List<string> m_extreme;

		public HangmanWords(string path) : base(path, FileMode.Open)
		{
			m_lines = ReadAllLines();
			m_easy = new List<string>();
			m_medium = new List<string>();
			m_hard = new List<string>();
			m_extreme = new List<string>();

			SeperateWords();
		}

		/// <summary>
		/// Parses the words file and seperates the words into their difficulties
		/// </summary>
		private void SeperateWords()
		{
			Game.Difficulty? currentWordDif = null;

			foreach (string line in m_lines)
			{
				if (line.StartsWith("//"))
					continue;

				else if (currentWordDif == null && line.StartsWith("#easy"))
					currentWordDif = Game.Difficulty.Easy;

				else if (currentWordDif == null && line.StartsWith("#medium"))
					currentWordDif = Game.Difficulty.Medium;

				else if (currentWordDif == null && line.StartsWith("#hard"))
					currentWordDif = Game.Difficulty.Hard;

				else if (currentWordDif == null && line.StartsWith("#extreme"))
					currentWordDif = Game.Difficulty.Extreme;

				else if (line.StartsWith("#end"))
					currentWordDif = null;

				else
				{
					switch (currentWordDif)
					{
					case Game.Difficulty.Easy:
						m_easy.Add(line.ToLower());
						break;

					case Game.Difficulty.Medium:
						m_medium.Add(line.ToLower());
						break;

					case Game.Difficulty.Hard:
						m_hard.Add(line.ToLower());
						break;

					case Game.Difficulty.Extreme:
						m_extreme.Add(line.ToLower());
						break;

					case null:
						break;
					}
				}
			}
		}

		/// <summary>
		/// Provides a random word from the given difficulty
		/// </summary>
		public string? GetRandomWord(Game.Difficulty diff)
		{
			Random rand = new Random();

			return diff switch
			{
				Game.Difficulty.Easy => m_easy[rand.Next(0, m_easy.Count)],
				Game.Difficulty.Medium => m_medium[rand.Next(0, m_medium.Count)],
				Game.Difficulty.Hard => m_hard[rand.Next(0, m_hard.Count)],
				Game.Difficulty.Extreme => m_extreme[rand.Next(0, m_extreme.Count)],
				_ => null,
			};
		}
	}
}
