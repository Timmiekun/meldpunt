﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meldpunt.Models
{
  public class BasePageModel
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// last part of the url
    /// </summary>
    public string UrlPart { get; set; }   
    public DateTimeOffset? Published { get; set; }

    public DateTimeOffset? LastModified { get; set; }
    public string MetaTitle { get; set; }
    public string MetaDescription { get; set; }   
    public string Content { get; set; }    

    /// <summary>
    /// Used to store all the serialized jsonstore components
    /// </summary>
    public string Components { get; set; }  
  }
}
