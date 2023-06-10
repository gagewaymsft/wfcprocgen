using System;
using System.Xml;
using System.Linq;
using System.ComponentModel;

static class Stuff
{
	public static int Random(this double[] a, double r)
	{
		double sum = a.Sum();

		if (sum == 0)
		{
			for (int j = 0; j < a.Count(); j++) a[j] = 1;
			sum = a.Sum();
		}

		for (int j = 0; j < a.Count(); j++) a[j] /= sum;

		int i = 0;
		double x = 0;

		while (i < a.Count())
		{
			x += a[i];
			if (r <= x) return i;
			i++;
		}

		return 0;
	}

	public static long Power(int a, int n)
	{
		long product = 1;
		for (int i = 0; i < n; i++) product *= a;
		return product;
	}

	public static T Get<T>(this XmlNode node, string attribute, T defaultT = default(T))
	{
		string s = ((XmlElement)node).GetAttribute(attribute);
		var converter = TypeDescriptor.GetConverter(typeof(T));
		return s == "" ? defaultT : (T)converter.ConvertFromString(s);
	}

	public static T[] SubArray<T>(this T[] data, int index, int length)
	{
	    T[] result = new T[length];
	    Array.Copy(data, index, result, 0, length);
	    return result;
	}
}