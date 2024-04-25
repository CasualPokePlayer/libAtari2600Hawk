#nullable disable

using HawkCommon.Interfaces;

namespace HawkCommon.BaseImplementations;

/// <summary>
/// A empty implementation of IController that represents the lack of
/// a controller interface
/// </summary>
/// <seealso cref="IController" />
public class NullController : IController
{
	public ControllerDefinition Definition { get; } = new ControllerDefinition("Null Controller").MakeImmutable();

	public bool IsPressed(string button) => false;
	
	public int AxisValue(string name) => 0;
	
	public static readonly NullController Instance = new NullController();
}
