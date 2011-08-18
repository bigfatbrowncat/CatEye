using System;

namespace CatEye.Core
{
#region Factory delegates
	public delegate IStageOperationParametersEditor StageOperationParametersEditorFactory(StageOperationParameters so);
	public delegate IBitmapCore BitmapCoreFactory(PPMLoader ppl, ProgressReporter callback);
	public delegate StageOperation StageOperationFactory(StageOperationParameters parameters);
	public delegate StageOperationParameters StageOperationParametersFactory(string id);
	public delegate IStageOperationHolder StageOperationHolderFactory(IStageOperationParametersEditor editor);
#endregion	
	/// <summary>
	/// Used to report progress to caller. If caller returns false,
	/// the callee should interrupt the process
	/// </summary>
	public delegate bool ProgressReporter(double progress);
}

