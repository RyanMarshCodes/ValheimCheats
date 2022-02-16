using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ValheimClient
{
    static class Utilities
    {
		public static void SetPrivateField(this object obj, string fieldName, object value)
		{
			obj.GetType().GetField(fieldName, Utilities.BindFlags).SetValue(obj, value);
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00007174 File Offset: 0x00005374
		public static T GetPrivateField<T>(this object obj, string fieldName)
		{
			return (T)((object)obj.GetType().GetField(fieldName, Utilities.BindFlags).GetValue(obj));
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00007192 File Offset: 0x00005392
		public static void SetPrivateProperty(this object obj, string propertyName, object value)
		{
			obj.GetType().GetProperty(propertyName, Utilities.BindFlags).SetValue(obj, value, null);
		}

		// Token: 0x06000064 RID: 100 RVA: 0x000071AD File Offset: 0x000053AD
		public static void InvokePrivateMethod(this object obj, string methodName, object[] methodParams)
		{
			obj.GetType().GetMethod(methodName, Utilities.BindFlags).Invoke(obj, methodParams);
		}

		public static BindingFlags BindFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
	}
}
