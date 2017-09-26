using Newtonsoft.Json;
using System;

namespace AirtableApiClient
{
  public class AirtableAttachment
  {
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("filename")]
    public string Filename { get; set; }

    [JsonProperty("size")]
    public long? Size { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("thumbnails")]
    public Thumbnails Thumbnails { get; set; }

    public override bool Equals(Object obj)
    {
      return obj is AirtableAttachment && this == (AirtableAttachment)obj;
    }
    public override int GetHashCode()
    {
      return Id.GetHashCode() ^ Url.GetHashCode() ^ Filename.GetHashCode() ^ Size.GetHashCode() ^ Type.GetHashCode();
    }
    public static bool operator ==(AirtableAttachment x, AirtableAttachment y)
    {
      if(Object.ReferenceEquals(x, y))
      {
        return true;
      }
      if((object)x == null && (object)y != null) { return false; }
      if ((object)y == null && (object)x != null) { return false; }

      return x.Id == y.Id && x.Url == y.Url && x.Filename == y.Filename && x.Size == y.Size && x.Type == y.Type && x.Thumbnails == y.Thumbnails;
    }
    public static bool operator !=(AirtableAttachment x, AirtableAttachment y)
    {
      return !(x == y);
    }
  }
  //--------------------------------------------------------

  public class Thumbnails
  {
    [JsonProperty("small")]
    public Thumbnail Small { get; internal set; }

    [JsonProperty("large")]
    public Thumbnail Large { get; internal set; }

    public override bool Equals(Object obj)
    {
      return obj is Thumbnails && this == (Thumbnails)obj;
    }
    public override int GetHashCode()
    {
      return Small.GetHashCode() ^ Small.GetHashCode();
    }
    public static bool operator ==(Thumbnails x, Thumbnails y)
    {
      if (Object.ReferenceEquals(x, y))
      {
        return true;
      }
      if ((object)x == null && (object)y != null) { return false; }
      if ((object)y == null && (object)x != null) { return false; }

      return x.Small == y.Small && x.Large == y.Large;
    }
    public static bool operator !=(Thumbnails x, Thumbnails y)
    {
      return !(x == y);
    }
  }


  //--------------------------------------------------------

  public class Thumbnail
  {
    [JsonProperty("url")]
    public string Url { get; internal set; }

    [JsonProperty("width")]
    public long Width { get; internal set; }

    [JsonProperty("height")]
    public long Height { get; internal set; }

    public override bool Equals(Object obj)
    {
      return obj is Thumbnail && this == (Thumbnail)obj;
    }
    public override int GetHashCode()
    {
      return Url.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();
    }
    public static bool operator ==(Thumbnail x, Thumbnail y)
    {
      if (Object.ReferenceEquals(x, y))
      {
        return true;
      }
      if ((object)x == null && (object)y != null) { return false; }
      if ((object)y == null && (object)x != null) { return false; }

      return x.Url == y.Url && x.Url == y.Url && x.Width == y.Width && x.Height == y.Height;
    }
    public static bool operator !=(Thumbnail x, Thumbnail y)
    {
      return !(x == y);
    }
  }

}
