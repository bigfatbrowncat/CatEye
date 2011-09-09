using System;

namespace CatEye
{
	public class StageOperationParametersEditModeSupportedAttribute : Attribute
	{
		private bool mSupported;
		public bool Supported { get { return mSupported; }  }
		public StageOperationParametersEditModeSupportedAttribute(bool supported) { mSupported = supported; }
	}
	
	public class StageOperationIDAttribute : Attribute
	{
		private string mID;
		public string ID { get { return mID; } }
		public StageOperationIDAttribute(string id) { mID = id; }
		
		public static string GetTypeID(Type stageOperationType)
		{
			object[] attrs = stageOperationType.GetCustomAttributes(typeof(StageOperationIDAttribute), true);
			if (attrs.Length == 0)
			{
				return null;
			}
			string id = ((StageOperationIDAttribute)attrs[0]).ID;
			return id;
		}
		public static Type FindTypeByID(Type[] stageOperationTypes, string id)
		{
			for (int i = 0; i < stageOperationTypes.Length; i++)
			{
				if (GetTypeID(stageOperationTypes[i]) == id) 
					return stageOperationTypes[i];
			}
			return null;
		}
	}
}
