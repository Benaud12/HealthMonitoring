﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace HealthMonitoring.SelfHost.Entities
{
    public static class EntityValidator
    {
        public static void ValidateModel(this object model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            Validator.ValidateObject(model, new ValidationContext(model));
        }
    }

    public class TagsAllowedSymbols
    {
        public static ValidationResult ValidateTags(string[] tags)
        {
            const string allowedSymbols = "_";

            if (tags == null || tags.All(tag => tag.All(symbol => char.IsLetterOrDigit(symbol) || allowedSymbols.Contains(symbol))))
            {
                return ValidationResult.Success;
            }

            throw new ArgumentException("Tags contains unallowed symbols.");
        }
    }
}