// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension.Helpers.Transformers;

internal enum TransformationType
{
    AsIs,
    LowerCase,
    UpperCase,
    CamelCase,
    PascalCase,
    CapitalCase,
    SnakeCase,
    UpperSnakeCase,
    PascalSnakeCase,
    ConstantCase,
    KebabCase,
    KebabUpperCase,
    HeaderCase,
    DotCase,
    PathCase,
    PathBackslashCase,
    SentenceCase,
    LowerFirst,
    UpperFirst,
    NoCase,
    SwapCase,
    RandomCase,
    TitleCase,
    TitleCaseInvariant,
    UpperFirstSnakeCase,
    UpperFirstKebabCase,
    RemoveDiacritics,
    RemoveSpecialCharacters,
    RemoveDuplicateWhitespace,
    SpaceCase
}