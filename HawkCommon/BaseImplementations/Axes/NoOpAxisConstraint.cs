namespace HawkCommon.BaseImplementations.Axes;

public sealed class NoOpAxisConstraint : AxisConstraint
{
	public string? Class { get; } = null;
	
	public string? PairedAxis { get; }
	
	public NoOpAxisConstraint(string pairedAxis) => PairedAxis = pairedAxis;
}