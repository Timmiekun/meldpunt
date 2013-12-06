﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.Web;

namespace Meldpunt.Models
{
  public class PlaatsModel
  {
    public string Gemeentenaam { get; set; }
    public List<String> Plaatsen { get; set; }
  }
}
