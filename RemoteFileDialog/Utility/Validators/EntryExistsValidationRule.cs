﻿using System.Globalization;
using System.Windows.Controls;
using RemoteFileDialog.Services;

namespace RemoteFileDialog.Utility.Validators
{
    public class EntryExistsValidationRule : ValidationRule
    {
        private readonly IEntryService _entryService;

        public EntryExistsValidationRule(IEntryService entryService)
        {
            _entryService = entryService;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (_entryService.EntryExists((string) value))
            {
                return new ValidationResult(true, null);
            }

            return new ValidationResult(false, "Entry doesn't exist");
        }
    }
}