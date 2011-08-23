using System;

namespace CatEye.Core
{
	public class UserCancelException : Exception 
	{  
		public UserCancelException() : base("User has cancelled the operation") {}
		public UserCancelException(string message): base(message) {}
	}
//	public class UserCancelAllException : UserCancelException
//	{
//		public UserCancelAllException() : base("User has cancelled all operations") {}
//	}
}

