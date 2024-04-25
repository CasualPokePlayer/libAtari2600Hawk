using HawkCommon.BaseImplementations;
using HawkCommon.Interfaces;
using HawkCommon.Interfaces.Services;

namespace Atari2600Hawk;

public partial class Atari2600 : IInputPollable
{
	public int LagCount
	{
		get => _lagCount;
		set => _lagCount = value;
	}
	
	public bool IsLagFrame
	{
		get => _islag;
		set => _islag = value;
	}
	
	public IInputCallbackSystem InputCallbacks { get; } = new InputCallbackSystem();
	
	private bool _islag = true;
	private int _lagCount;
}