using PromptDetector.Domain.Models;
using PromptDetector.Exceptions;
using System.Reflection;

namespace PromptDetector.Validations
{
    public static class DetectionInputsValidations
    {
        public static void IsValid(this DetectionRequest detectionRequest)
        {
            if (detectionRequest == null)
                throw new InputValidationException("Detection Request should not be null");

            if (string.IsNullOrEmpty(detectionRequest.prompt))
                throw new InputValidationException("Prompt is empty");

            var truePropertyNames = detectionRequest.settings.GetType()
               .GetProperties(BindingFlags.Public | BindingFlags.Instance)
               .Where(prop => prop.PropertyType == typeof(bool) && (bool)prop.GetValue(detectionRequest.settings))
               .Select(prop => prop.Name)
               .ToArray();

            if (truePropertyNames.Length == 0)
                throw new InputValidationException("Detection Request's settings should contain at least 1 true topic to check");
        }

    }
}
