using System;
using System.Collections.Generic;
using System.IO;

#nullable enable
namespace Hangman.FileSystem
{
	/// <summary>
	/// Class for easier file handling
	/// </summary>
	public abstract class File
	{
		private readonly string					m_path;
		private readonly FileMode				m_fileMode;
		private readonly FileStream			m_stream;
		protected readonly StreamReader m_reader;
		protected readonly StreamWriter m_writer;

		public File(string path, FileMode fileMode)
		{
			m_path = path;
			m_fileMode = fileMode;
			m_stream = new FileStream(m_path, m_fileMode);
			m_reader = new StreamReader(m_stream);
			m_writer = new StreamWriter(m_stream);
		}

		/// <summary>
		/// Reads the next character from the input stream and advances the character position by one character.
		/// </summary>
		/// <returns>The next character from the input stream represented as an <see cref="int" /> object, or -1 if no more characters are available.</returns>
		/// <exception cref="IOException" />
		public int Read() => m_reader.Read();
		/// <summary>
		/// Reads a line of characters from the current stream and returns the data as a string
		/// </summary>
		/// <returns>The next line from the input stream, or <see cref="null" /> if the end of the input stream is reached</returns>
		/// <exception cref="OutOfMemoryException" />
		/// <exception cref="IOException" />
		public string? ReadLine() => m_reader.ReadLine();

		/// <summary>
		/// Reads all the remaning lines from the current stream and returns the data as a list of strings
		/// </summary>
		/// <returns>All the remaning lines of the input sream in a list</returns>
		/// <exception cref="ObjectDisposedException" />
		/// <exception cref="OutOfMemoryException" />
		/// <exception cref="IOException" />
		public List<string> ReadAllLines()
		{
			List<string> lines = new List<string>();

			while (!m_reader.EndOfStream)
			{
				string line = ReadLine()!;

				if (!string.IsNullOrWhiteSpace(line))
					lines.Add(line);
			}

			return lines;
		}

		/// <summary>
		/// Writes a string to the stream
		/// </summary>
		/// <exception cref="ObjectDisposedException" />
		/// <exception cref="NotSupportedException" />
		/// <exception cref="IOException" />
		public void Write(string? value) => m_writer.Write(value);
		public void WriteLine() => m_writer.WriteLine();
		public void WriteLine(string? value) => m_writer.WriteLine(value);
	}
}
