using System;

namespace CatEye.Core
{
	/// <summary>
	/// Used to report prograss to caller. If caller returns false,
	/// the callee should interrupt the process
	/// </summary>
	public delegate bool ProgressReporter(double progress);
}

