// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension.Commands;

internal sealed partial class UnpinFavoriteTransformationCommand : InvokableCommand
{
    private readonly TransformationDefinition _transformationDefinition;
    private readonly PinnedTransformationsManager _pinnedColorsManager = PinnedTransformationsManager.Instance;
    public UnpinFavoriteTransformationCommand(TransformationDefinition transformationDefinition)
    {
        this.Name = Strings.Command_Unpin_Title;
        this.Icon = Icons.Unpin;
        this._transformationDefinition = transformationDefinition;
    }

    public override ICommandResult Invoke()
    {
        this._pinnedColorsManager.Unpin(this._transformationDefinition.Type);
        return CommandResult.ShowToast(new ToastArgs { Message = Strings.Command_Unpin_Result!, Result = CommandResult.KeepOpen() });
    }
}