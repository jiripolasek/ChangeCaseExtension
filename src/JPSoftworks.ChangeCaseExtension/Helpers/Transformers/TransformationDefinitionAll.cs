// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension.Helpers.Transformers;

internal record TransformationDefinitionAll(
    TransformationType Type,
    string Title,
    TransformationCategory Category,
    Func<string, string> Transform)
    : TransformationDefinition(Type, Title, Category);