// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension.Pages;

internal sealed partial class PinnedTransformationListItem : TransformationListItemBase
{
    public PinnedTransformationListItem(
        IDynamicListPage parentList,
        TransformationDefinition definition,
        string[] lines,
        HistoryManager historyManager,
        PinnedTransformationsManager pinnedTransformationsManager)
        : base(
            parentList, 
            definition, 
            lines, 
            historyManager,
            pinnedTransformationsManager,
            extraSubject: " • " + Strings.Command_PinnedTransformation_Pinned!)
    {
        this.Icon = Icons.Pin;
    }
}