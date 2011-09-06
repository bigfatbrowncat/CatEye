using System;
using CatEye.Core;

namespace CatEye.UI.Base
{
	public delegate void QueueProgressMessageReporter(string source, string destination, double progress, string status, IBitmapCore image);
}

