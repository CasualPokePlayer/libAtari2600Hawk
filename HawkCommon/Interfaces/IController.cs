#nullable disable

namespace HawkCommon.Interfaces;

public interface IController
{
	/// <summary>
	/// Returns the current state of a boolean control
	/// </summary>
	bool IsPressed(string button);
	
	/// <summary>
	/// Returns the state of an axis control
	/// </summary>
	int AxisValue(string name);
}
