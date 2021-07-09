using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZigitServer3.Services
{
    public class DataValidator
    { 
    private Regex _EmailRegex { get; set; }
    public DataValidator()
    {
        this._EmailRegex = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.CultureInvariant | RegexOptions.Singleline);
    }
    /*
     * .שדה הסיסמא גם כן יכול להכיל כל סוג טקסט, אשר מוסתר ומופיע ככוכביות בעת ההזנה. שדה הסיסמא צריך
      להיות לפחות שמונה תוים וצריך להכיל בתוכו לפחות מספר אחד ואות גדולה )CAPITAL )אחת.
*/
    public bool ValidateEmail(string email)
    {
       
        return _EmailRegex.IsMatch(email);
    }
    public bool VaildatePassword(string password)
    {
        return password.Any(char.IsUpper) && password.Any(char.IsNumber) && (password.Length > 7);
    }
}
}
