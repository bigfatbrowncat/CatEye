using System;

namespace CatEye.Core
{
	public delegate IStageOperationHolder StageOperationHolderFactory(IStageOperationParametersEditor editor);
	
	public interface IStageOperationHolder
	{
		// Editing mode	is active/passive
		bool Edit { get; set; }
		bool Sensitive { get; set; }
		bool Freeze { get; set; }
		bool FrozenButtonsState { get; set; }
		
		IStageOperationParametersEditor StageOperationParametersEditor { get; }
	}
}

