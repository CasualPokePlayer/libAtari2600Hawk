namespace HawkCommon.Interfaces;

public interface IInputDisplayGenerator
{
	/// <summary>
	/// Generates a display friendly version of the input log entry
	/// </summary>
	string Generate();
}