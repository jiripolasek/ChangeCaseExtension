// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

namespace JPSoftworks.ChangeCaseExtension.Helpers.Transformers;

internal static class TransformationRegistry
{
    private static readonly TransformationDefinition[] TransformationDefinitions =
    [
        // text
        new TransformationDefinitionAll(TransformationType.LowerCase, "lower case", TransformationCategory.Text,
            CaseTransformers.ToLowerCase),
        new TransformationDefinitionAll(TransformationType.UpperCase, "UPPER CASE", TransformationCategory.Text,
            CaseTransformers.ToUpperCase),
        new TransformationDefinitionAll(TransformationType.SentenceCase, "Sentence case", TransformationCategory.Text,
            CaseTransformers.ToSentenceCase),
        new TransformationDefinitionAll(TransformationType.LowerFirst, "lower First", TransformationCategory.Text,
            CaseTransformers.ToLowerFirst),
        new TransformationDefinitionAll(TransformationType.UpperFirst, "Upper first", TransformationCategory.Text,
            CaseTransformers.ToUpperFirst),
        new TransformationDefinitionAll(TransformationType.CapitalCase, "Capital Case", TransformationCategory.Text,
            CaseTransformers.ToCapitalCase),
        new TransformationDefinitionAll(TransformationType.TitleCase, "Title Case (Chicago style)", TransformationCategory.Text,
            ChicagoTitleCaseConverter.ToChicagoTitleCase),
        new TransformationDefinitionAll(TransformationType.SwapCase, "sWAP cASE", TransformationCategory.Text,
            CaseTransformers.ToSwapCase),
        new TransformationDefinitionAll(TransformationType.RandomCase, "rAndOm cAsE", TransformationCategory.Text,
            CaseTransformers.ToRandomCase),

        // technical
        new TransformationDefinitionWords(TransformationType.CamelCase, "camelCase", TransformationCategory.Technical,
            CaseTransformersOnArray.ToCamelCase),
        new TransformationDefinitionWords(TransformationType.PascalCase, "PascalCase", TransformationCategory.Technical,
            CaseTransformersOnArray.ToPascalCase),
        new TransformationDefinitionWords(TransformationType.SnakeCase, "snake_case", TransformationCategory.Technical,
            CaseTransformersOnArray.ToSnakeCase),
        new TransformationDefinitionWords(TransformationType.UpperSnakeCase, "UPPER_SNAKE_CASE",
            TransformationCategory.Technical, CaseTransformersOnArray.ToUpperSnakeCase),
        new TransformationDefinitionWords(TransformationType.PascalSnakeCase, "Pascal_Snake_Case",
            TransformationCategory.Technical, CaseTransformersOnArray.ToPascalSnakeCase),
        new TransformationDefinitionWords(TransformationType.ConstantCase, "CONSTANT_CASE",
            TransformationCategory.Technical, CaseTransformersOnArray.ToConstantCase),
        new TransformationDefinitionWords(TransformationType.KebabCase, "kebab-case", TransformationCategory.Technical,
            CaseTransformersOnArray.ToKebabCase),
        new TransformationDefinitionWords(TransformationType.KebabUpperCase, "KEBAB-UPPER-CASE",
            TransformationCategory.Technical, CaseTransformersOnArray.ToKebabUpperCase),
        new TransformationDefinitionWords(TransformationType.HeaderCase, "Header-Case",
            TransformationCategory.Technical, CaseTransformersOnArray.ToHeaderCase),
        new TransformationDefinitionWords(TransformationType.UpperFirstSnakeCase, "Upper first Snake_Case",
            TransformationCategory.Technical, CaseTransformersOnArray.ToUpperFirstSnakeCase),
        new TransformationDefinitionWords(TransformationType.UpperFirstKebabCase, "Upper first Kebab-Case",
            TransformationCategory.Technical, CaseTransformersOnArray.ToUpperFirstKebabCase),

        // separators
        new TransformationDefinitionWords(TransformationType.SpaceCase, "space case",
            TransformationCategory.Separators, CaseTransformersOnArray.ToSpaceCase),
        new TransformationDefinitionWords(TransformationType.DotCase, "dot.case",
            TransformationCategory.Separators, CaseTransformersOnArray.ToDotCase),
        new TransformationDefinitionWords(TransformationType.PathCase, "path/case",
            TransformationCategory.Separators, CaseTransformersOnArray.ToPathCase),
        new TransformationDefinitionWords(TransformationType.PathBackslashCase, @"path\case\backslash",
            TransformationCategory.Separators, CaseTransformersOnArray.ToPathBackslashCase),

        // special
        new TransformationDefinitionAll(TransformationType.RemoveDiacritics, "Remove diacritics",
            TransformationCategory.Cleanup, StringNormalizer.RemoveDiacritics),
        new TransformationDefinitionAll(TransformationType.RemoveSpecialCharacters, "Remove special characters",
            TransformationCategory.Cleanup, StringNormalizer.RemoveSpecialCharacters),
        new TransformationDefinitionAll(TransformationType.RemoveDuplicateWhitespace, "Remove duplicate spaces, lines and tabs",
            TransformationCategory.Cleanup, StringNormalizer.RemoveDuplicateWhitespacePerformance),
        new TransformationDefinitionAll(TransformationType.RemoveDuplicateSpaces, "Remove duplicate spaces",
            TransformationCategory.Cleanup, StringNormalizer.RemoveDuplicateSpaces)
    ];

    public static IReadOnlyList<TransformationDefinition> GetTransformations()
    {
        return TransformationDefinitions;
    }
}