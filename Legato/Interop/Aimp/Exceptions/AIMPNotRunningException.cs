using System;

namespace AimpArtwork.Exception
{
	public class AIMPNotRunningException : ApplicationException
	{
		public AIMPNotRunningException() { }

		public AIMPNotRunningException(string message) : base(message) { }

		public AIMPNotRunningException(string message, System.Exception inner) : base(message, inner) { }
	}
}
