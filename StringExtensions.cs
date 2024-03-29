﻿using Loxifi.Services;
using System.Globalization;
using System.Reflection;

namespace Loxifi
{
	/// <summary>
	///
	/// </summary>
	public static class StringExtensions
	{
		private static IEnumerable<string> SplitEnumString(string toSplit)
		{
			string thisVal = string.Empty;
			for (int i = 0; i < toSplit.Length; i++)
			{
				char c = toSplit[i];

				if (char.IsLetter(c) || (char.IsDigit(c) && thisVal.Length > 0))
				{
					thisVal += c;
				}
				else
				{
					if (IsValidEnumValue(thisVal))
					{
						yield return thisVal;
					}

					thisVal = string.Empty;
				}
			}

			if (IsValidEnumValue(thisVal))
			{
				yield return thisVal;
			}
		}

		private static HashSet<Type> CantChange { get; set; } = new HashSet<Type>();

		static StringExtensions()
		{
			_ = CantChange.Add(typeof(DateTimeOffset));
		}

		private static bool IsValidEnumValue(string toCheck) => !string.IsNullOrWhiteSpace(toCheck) && (int.TryParse(toCheck, out _) || char.IsLetter(toCheck[0]));

		/// <summary>
		/// Attempts to convert a string to the specified type
		/// </summary>
		/// <typeparam name="T">The type to cast the return value as</typeparam>
		/// <param name="s">The string value</param>
		/// <param name="ignoreCase">Whether or not case should be ignored (enum)</param>
		/// <returns>A casted representation of the string value</returns>
		public static T? Convert<T>(this string s, bool ignoreCase = false) => (T?)s.Convert(typeof(T), ignoreCase);

		/// <summary>
		/// Converts a string to the requested type. Handles nullables.
		/// </summary>
		/// <param name="s">The string value</param>
		/// <param name="t">The type to cast the value as</param>
		/// <param name="ignoreCase">Whether or not case should be ignored (enum)</param>
		/// <returns></returns>
		public static object? Convert(this string s, Type t, bool ignoreCase = false)
		{
			//TODO: Caching a bunch of this stuff can really speed up conversions
			if (t is null)
			{
				throw new ArgumentNullException(nameof(t));
			}

			if (t == typeof(string))
			{
				return s;
			}

			if(Nullable.GetUnderlyingType(t) != null && string.IsNullOrWhiteSpace(s))
			{
				return null;
			}

			foreach (MethodInfo mi in t.GetMethods())
			{
				//Must be an explicit or implicit converter
				if (mi.Name is not "op_Implicit" and not "op_Explicit")
				{
					continue;
				}

				//Of which the return type matches out target type
				if (mi.ReturnType != t)
				{
					continue;
				}

				ParameterInfo[] parameters = mi.GetParameters();

				//It must have a single parameter
				if (parameters.Length != 1)
				{
					continue;
				}

				//And that parameter must be a string
				if (parameters[0].ParameterType != typeof(string))
				{
					continue;
				}

				//This is our matching conversion method,
				//If we're still here.
				return mi.Invoke(null, new object[] { s });
			}

			if (t == typeof(bool))
			{
				return s == "1" || string.Equals(s, "yes", StringComparison.OrdinalIgnoreCase)
					? true
					: s == "0" || string.Equals(s, "no", StringComparison.OrdinalIgnoreCase) ? false : (object)bool.Parse(s);
			}

			//I feel like this could be done better by leveraging system types.
			if (string.IsNullOrWhiteSpace(s) && t.IsValueType)
			{
				return t.IsValueType ? ObjectService.IsDefault(t) : null;
			}

			if (t is null)
			{
				throw new ArgumentNullException(nameof(t));
			}

			if (t.IsEnum)
			{
				StringComparison comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

				if (char.IsDigit(s[0]))
				{
					return Enum.Parse(t, s);
				}
				else
				{
					object enumValue = Enum.GetValues(t).Cast<object>().FirstOrDefault(e => string.Equals(s, e.ToString(), comparison));

					if (enumValue != null)
					{
						return enumValue;
					}
					else
					{
						if (t.GetCustomAttribute<FlagsAttribute>() is null)
						{
							throw new Exception($"Enum value {s} not found on type {t}");
						}
						else
						{
							long value = 0;
							Dictionary<string, long> enumValues = new();

							foreach (object val in Enum.GetValues(t))
							{
								object underlyingType = System.Convert.ChangeType(val, Enum.GetUnderlyingType(t));

								enumValues.Add(val.ToString(), System.Convert.ToInt64(underlyingType));
							}

							foreach (string toAdd in SplitEnumString(s))
							{
								value |= enumValues[toAdd];
							}

							return Enum.ToObject(t, value);
						}
					}
				}
			}

			if (t == typeof(System.Guid))
			{
				return System.Guid.Parse(s);
			}

			if (Nullable.GetUnderlyingType(t) != null)
			{
				return Activator.CreateInstance(t, s.Convert(t.GetGenericArguments()[0]));
			}
			else
			{
				if (!CantChange.Contains(t))
				{
					try
					{
						return System.Convert.ChangeType(s, t, CultureInfo.CurrentCulture);
					}
					catch (InvalidCastException)
					{
						_ = CantChange.Add(t);
					}
				}

				foreach (MethodInfo m in t.GetMethods())
				{
					if (m.Name == "Parse" && m.IsStatic)
					{
						ParameterInfo[] @params = m.GetParameters();

						if (@params.Length == 1 && @params.Single().ParameterType == typeof(string))
						{
							return m.Invoke(null, new object[] { s });
						}
					}
				}
			}

			throw new Exception($"No valid cast path known for type {t}");
		}
	}
}