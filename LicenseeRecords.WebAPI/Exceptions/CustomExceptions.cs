using System.Diagnostics.CodeAnalysis;

namespace LicenseeRecords.WebAPI.Exceptions;
[ExcludeFromCodeCoverage(Justification = "No Need to Unit Test Exceptions")]
public static class CustomExceptions
{
	public class NotFoundException : Exception
	{
		public NotFoundException() : base() { }
		public NotFoundException(string message) : base(message) { }

		public NotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
	}

	public class BadRequestException : Exception
	{
		public BadRequestException() : base() { }
		public BadRequestException(string message) : base(message) { }
		public BadRequestException(string? message, Exception? innerException) : base(message, innerException) { }
	}

	public class ConflictException : Exception
	{
		public ConflictException() : base() { }
		public ConflictException(string message) : base(message) { }
		public ConflictException(string? message, Exception? innerException) : base(message, innerException) { }
	}
}