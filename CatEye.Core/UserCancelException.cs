using System;

namespace CatEye.Core
{
	public class UserCancelException : Exception 
	{  
		public UserCancelException() : base("User has cancelled the operation") {}
	}
}

