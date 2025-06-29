// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension.Pages;

internal sealed partial class RecentTransformationListItem : TransformationListItemBase
{
    public RecentTransformationListItem(
        IDynamicListPage parentList,
        TransformationDefinition definition,
        string[] lines,
        HistoryManager historyManager)
        : base(
            parentList, 
            definition, 
            lines, 
            historyManager,
            extraSubject: " • " + Strings.Command_RecentTranformations_RecentlyUsed)
    {
        this.Icon = Icons.History;
    }


}