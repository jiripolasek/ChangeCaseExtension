// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

using JPSoftworks.CommandPalette.Extensions.Toolkit.Logging;

namespace JPSoftworks.ChangeCaseExtension.Helpers.Transformers;

internal static class BatchTransformer
{
    public static Dictionary<TransformationDefinition, string[]> TransformAll(string input)
    {
        var results = new Dictionary<TransformationDefinition, string[]>();

        var transformations = TransformationRegistry.GetTransformations();
        foreach (var transformationsByCategory in transformations.GroupBy(static t => t.Category))
        {
            var inputTemp = input;
            if (transformationsByCategory.Key.FilePreprocessor != null)
            {
                inputTemp = transformationsByCategory.Key.FilePreprocessor(inputTemp);
            }

            var linesOfWords = new List<string[]>();
            foreach (var line in inputTemp.ToLines())
            {
                string[] words;
                var transformationCategory = transformationsByCategory.Key;
                if (transformationCategory.LinePreprocessor != null)
                {
                    words = transformationCategory.LinePreprocessor(line);
                }
                else
                {
                    words = line.Split();
                }

                linesOfWords.Add(words.Where(static word => word.Length > 0).ToArray());
            }


            foreach (var transformation in transformationsByCategory)
            {
                try
                {
                    if (transformation is TransformationDefinitionWords wordTransformation)
                    {
                        results[transformation] = [.. linesOfWords.Select(wordTransformation.Transform)];
                    }
                    else if (transformation is TransformationDefinitionAll allTransformation)
                    {
                        results[transformation] = [allTransformation.Transform(inputTemp)];
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
#if DEBUG
                    results[transformation] = ["Transformation failed", ex.Message, ex.ToString()];
#endif
                }
            }
        }

        return results;
    }
}