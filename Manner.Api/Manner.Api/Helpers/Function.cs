namespace Manner.Api.Helpers;

public static class Function
{
    public const string InvalidPostcodeMessage = "Invalid Postcode.";
    public static string GetOutwardCode(string postcode, out List<string> errors)
    {
        errors = new List<string>();
        postcode = postcode.Trim();

        if (string.IsNullOrWhiteSpace(postcode))
        {
            errors.Add("Postcode should not be empty");
        }

        if (postcode.Length <= 4)
        {
            if (postcode.Length < 2)
            {
                errors.Add("Invalid postcode. Outward postcode length should be greater than 2");
            }
            // If postcode length is 4 or less, use it as is (after trimming)            
            return postcode.ToUpper();
        }
        else if (postcode.Length > 4)
        {
            postcode = postcode.Replace(" ", "").ToUpper();
            // Outward = everything except last 3 characters
            return postcode.Substring(0, postcode.Length - 3);
        }
        else
        {
            errors.Add("Invalid postcode format.");
            return string.Empty;
        }
    }

}
