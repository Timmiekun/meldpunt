using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.Text.RegularExpressions;

namespace Meldpunt.Models
{

  public class MailModel
  {
    [Required]
    [EmailAddressAttribute(ErrorMessage = "Ongeldig e-mail adres")]
    public string Email { get; set; }
    //[Required(ErrorMessage = "Vul a.u.b. een naam in")]
    public string Name { get; set; }
    //[Required(ErrorMessage = "Vul a.u.b. een bericht in")]
    public string Message { get; set; }

  }

  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
  public class EmailAddressAttribute : DataTypeAttribute
  {
    private readonly Regex regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.Compiled);

    public EmailAddressAttribute()
      : base(DataType.EmailAddress)
    {

    }

    public override bool IsValid(object value)
    {

      string str = Convert.ToString(value, CultureInfo.CurrentCulture);
      if (string.IsNullOrEmpty(str))
        return true;

      Match match = regex.Match(str);
      return ((match.Success && (match.Index == 0)) && (match.Length == str.Length));
    }
  }
}
