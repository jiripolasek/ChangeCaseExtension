// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension.Helpers.Transformers;

internal enum TransformationType
{
    AsIs = 0,
    LowerCase = 1,
    UpperCase = 2,
    CamelCase = 3,
    PascalCase = 4,
    CapitalCase = 5,
    SnakeCase = 6,
    UpperSnakeCase = 7,
    PascalSnakeCase = 8,
    ConstantCase = 9,
    KebabCase = 10,
    KebabUpperCase = 11,
    HeaderCase = 12,
    DotCase = 13,
    PathCase = 14,
    PathBackslashCase = 15,
    SentenceCase = 16,
    LowerFirst = 17,
    UpperFirst = 18,
    NoCase = 19,
    SwapCase = 20,
    RandomCase = 21,
    TitleCase = 22,
    TitleCaseInvariant = 23,
    UpperFirstSnakeCase = 24,
    UpperFirstKebabCase = 25,
    RemoveDiacritics = 26,
    RemoveSpecialCharacters = 27,
    RemoveDuplicateWhitespace = 28,
    SpaceCase = 29,
    RemoveDuplicateSpaces = 30
}