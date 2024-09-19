using System.ComponentModel.DataAnnotations;
using System.Globalization;

public class EmojiValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is string emojiString && IsEmoji(emojiString))
        {
            return ValidationResult.Success;
        }
        return new ValidationResult("The field must contain a valid emoji.");
    }

    private bool IsEmoji(string input)
    {
        if (string.IsNullOrEmpty(input)) return false;

        // Use StringInfo to handle surrogate pairs (for emojis made up of multiple code points)
        var stringInfo = new StringInfo(input);

        for (int i = 0; i < stringInfo.LengthInTextElements; i++)
        {
            string textElement = stringInfo.SubstringByTextElements(i, 1);
            int codePoint = char.ConvertToUtf32(textElement, 0);

            if (!IsEmojiCodePoint(codePoint))
            {
                return false;
            }
        }
        return true;
    }

    private bool IsEmojiCodePoint(int codePoint)
    {
        return (codePoint >= 0x1F600 && codePoint <= 0x1F64F) || // Emoticons
               (codePoint >= 0x1F300 && codePoint <= 0x1F5FF) || // Miscellaneous Symbols and Pictographs
               (codePoint >= 0x1F680 && codePoint <= 0x1F6FF) || // Transport and Map Symbols
               (codePoint >= 0x2600 && codePoint <= 0x26FF) ||   // Miscellaneous Symbols
               (codePoint >= 0x2700 && codePoint <= 0x27BF) ||   // Dingbats
               (codePoint >= 0x1F900 && codePoint <= 0x1F9FF) || // Supplemental Symbols and Pictographs
               (codePoint >= 0x1FA70 && codePoint <= 0x1FAFF) || // Symbols and Pictographs Extended-A
               (codePoint >= 0x1F1E6 && codePoint <= 0x1F1FF);   // Regional Indicator Symbols (for flags)
    }
}
