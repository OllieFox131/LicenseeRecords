using FluentValidation.Results;
using System.Text;

namespace LicenseeRecords.WebAPI.Helpers;
public static class ValidationFailureHelper
{
	public static string ConvertErrorArrayToString(List<ValidationFailure> errors)
	{
		StringBuilder sb = new();
		foreach (ValidationFailure error in errors)
		{
			sb.AppendLine(error.ErrorMessage);
		}

		return sb.ToString();
	}
}
