using System;

namespace Meldpunt.Models.ViewModels
{

  public class RecaptchRequestModel
  {
    public bool Success { get; set; }

    public DateTimeOffset ChallengeTs { get; set; }

    public string Hostname { get; set; }

  }
}
