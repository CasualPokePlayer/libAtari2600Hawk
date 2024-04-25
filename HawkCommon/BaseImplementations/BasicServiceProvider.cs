using System;
using System.Collections.Generic;
using System.Linq;

using HawkCommon.Interfaces;

namespace HawkCommon.BaseImplementations;

/// <summary>
/// A generic implementation of IEmulatorService provider that provides
/// this functionality to any core.
/// The provider will scan an IEmulator and register all IEmulatorServices
/// that the core object itself implements.  In addition it provides
/// a Register() method to allow the core to pass in any additional services
/// </summary>
/// <seealso cref="IEmulatorServiceProvider"/>
public class BasicServiceProvider : IEmulatorServiceProvider
{
	private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
	
	public BasicServiceProvider(IEmulator core)
	{
		_services.Add(typeof(IEmulator), core);
	}
	
	/// <summary>the core can call this to register an additional service</summary>
	/// <typeparam name="T">The <see cref="IEmulatorService"/> to register</typeparam>
	/// <exception cref="ArgumentNullException"><paramref name="provider"/> is null</exception>
	public void Register<T>(T provider)
		where T : class, IEmulatorService
	{
		_services[typeof(T)] = provider;
	}
	
	public T GetService<T>()
		where T : IEmulatorService
		=> (T) GetService(typeof(T))!;
	
	public object? GetService(Type t)
	{
		return _services.TryGetValue(t, out var service) ? service : null;
	}
	
	public bool HasService<T>()
		where T : IEmulatorService
	{
		return HasService(typeof(T));
	}
	
	public bool HasService(Type t)
	{
		return _services.ContainsKey(t);
	}
	
	public IEnumerable<Type> AvailableServices =>_services.Select(d => d.Key);
}
