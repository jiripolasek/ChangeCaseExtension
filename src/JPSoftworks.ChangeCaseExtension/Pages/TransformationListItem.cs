// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension.Pages;

internal sealed partial class TransformationListItem(
    IDynamicListPage parentList,
    TransformationDefinition definition,
    string[] lines,
    HistoryManager historyManager,
    PinnedTransformationsManager pinnedTransformationsManager)
    : TransformationListItemBase(parentList, definition, lines, historyManager, pinnedTransformationsManager, definition.Category.DisplayName);