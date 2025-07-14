// ------------------------------------------------------------
//
// Copyright (c) Jiří Polášek. All rights reserved.
//
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension.Commands;

internal sealed partial class PinFavoriteTransformationCommand : InvokableCommand
{
    private readonly TransformationDefinition _transformationDefinition;
    private readonly PinnedTransformationsManager _pinnedColorsManager = PinnedTransformationsManager.Instance;

    public PinFavoriteTransformationCommand(TransformationDefinition transformationDefinition)
    {
        this.Name = Strings.Command_Pin_Title;
        this.Icon = Icons.Pin;
        this._transformationDefinition = transformationDefinition;
    }

    public override ICommandResult Invoke()
    {
        this._pinnedColorsManager.Pin(this._transformationDefinition.Type);
        return CommandResult.ShowToast(new ToastArgs { Message = Strings.Command_PinnedTransformation_Pinned, Result = CommandResult.KeepOpen() });
    }
}