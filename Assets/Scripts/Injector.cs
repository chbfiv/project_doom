using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Injector {

	private static volatile Injector _instance;
	private static readonly object _lock = new object();

	private List<object> _objs = new List<object>();

	private Injector() {}
	
	private static Injector instance
	{
		get 
		{
			if (_instance == null) 
			{
				lock (_lock) 
				{
					if (_instance == null) 
						_instance = new Injector();
				}
			}
			
			return _instance;
		}
	}

	private void RegisterInternal<T>(T obj) {
		if (obj == null) {
			throw new InvalidOperationException ("Cant register a null obj");
		}

		T lookup = Get<T> ();

		if (lookup != null) {
			throw new InvalidOperationException ("Registered more than one service by that can be found with this type");
		} else 	{
			_objs.Add(obj);
		}
	}

	public static void Register<T>(T obj) {
		instance.RegisterInternal<T> (obj);
	}

	public T GetInternal<T>() {
		//HACK: just for services ATM. Not really safe outside of that
		return (T)_objs.FirstOrDefault (o => o.GetType ().IsAssignableFrom(typeof(T)));
	}
	
	public static T Get<T>() {
		return instance.GetInternal<T> ();
	}
}
