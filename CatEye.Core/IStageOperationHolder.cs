using System;

namespace CatEye.Core
{
	public interface IStageOperationHolder
	{
		// Editing mode	is active/passive
		bool Edit { get; set; }
		bool Sensitive { get; set; }
		bool Freeze { get; set; }
		bool FrozenButtonsState { get; set; }
		
		event EventHandler<EventArgs> StageActiveButtonClicked;
		event EventHandler<EventArgs> UpTitleButtonClicked;
		event EventHandler<EventArgs> DownTitleButtonClicked;
		event EventHandler<EventArgs> EditButtonClicked;
		event EventHandler<EventArgs> FreezeButtonClicked;
		event EventHandler<EventArgs> RemoveButtonClicked;
		
		IStageOperationParametersEditor StageOperationParametersEditor { get; }
	}
}

