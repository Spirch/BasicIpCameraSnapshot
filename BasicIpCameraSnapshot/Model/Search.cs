using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BasicIpCameraSnapshot.Model;

[XmlRoot(ElementName = "timeSpan", Namespace = "http://www.hikvision.com/ver20/XMLSchema")]
public class TimeSpan
{

    [XmlElement(ElementName = "startTime", Namespace = "http://www.hikvision.com/ver20/XMLSchema")]
    public DateTime StartTime { get; set; }

    [XmlElement(ElementName = "endTime", Namespace = "http://www.hikvision.com/ver20/XMLSchema")]
    public DateTime EndTime { get; set; }
}

[XmlRoot(ElementName = "mediaSegmentDescriptor", Namespace = "http://www.hikvision.com/ver20/XMLSchema")]
public class MediaSegmentDescriptor
{

    [XmlElement(ElementName = "contentType", Namespace = "http://www.hikvision.com/ver20/XMLSchema")]
    public string ContentType { get; set; }

    [XmlElement(ElementName = "codecType", Namespace = "http://www.hikvision.com/ver20/XMLSchema")]
    public string CodecType { get; set; }

    [XmlElement(ElementName = "playbackURI", Namespace = "http://www.hikvision.com/ver20/XMLSchema")]
    public string PlaybackURI { get; set; }
}

[XmlRoot(ElementName = "metadataMatches", Namespace = "http://www.hikvision.com/ver20/XMLSchema")]
public class MetadataMatches
{

    [XmlElement(ElementName = "metadataDescriptor", Namespace = "http://www.hikvision.com/ver20/XMLSchema")]
    public string MetadataDescriptor { get; set; }
}

[XmlRoot(ElementName = "searchMatchItem", Namespace = "http://www.hikvision.com/ver20/XMLSchema")]
public class SearchMatchItem
{

    [XmlElement(ElementName = "sourceID", Namespace = "http://www.hikvision.com/ver20/XMLSchema")]
    public string SourceID { get; set; }

    [XmlElement(ElementName = "trackID", Namespace = "http://www.hikvision.com/ver20/XMLSchema")]
    public int TrackID { get; set; }

    [XmlElement(ElementName = "timeSpan", Namespace = "http://www.hikvision.com/ver20/XMLSchema")]
    public TimeSpan TimeSpan { get; set; }

    [XmlElement(ElementName = "mediaSegmentDescriptor", Namespace = "http://www.hikvision.com/ver20/XMLSchema")]
    public MediaSegmentDescriptor MediaSegmentDescriptor { get; set; }

    [XmlElement(ElementName = "metadataMatches", Namespace = "http://www.hikvision.com/ver20/XMLSchema")]
    public MetadataMatches MetadataMatches { get; set; }
}

[XmlRoot(ElementName = "matchList", Namespace = "http://www.hikvision.com/ver20/XMLSchema")]
public class MatchList
{

    [XmlElement(ElementName = "searchMatchItem", Namespace = "http://www.hikvision.com/ver20/XMLSchema")]
    public List<SearchMatchItem> SearchMatchItem { get; set; }
}

[XmlRoot(ElementName = "CMSearchResult", Namespace = "http://www.hikvision.com/ver20/XMLSchema")]
public class CMSearchResult
{

    [XmlElement(ElementName = "searchID", Namespace = "http://www.hikvision.com/ver20/XMLSchema")]
    public string SearchID { get; set; }

    [XmlElement(ElementName = "responseStatus", Namespace = "http://www.hikvision.com/ver20/XMLSchema")]
    public bool ResponseStatus { get; set; }

    [XmlElement(ElementName = "responseStatusStrg", Namespace = "http://www.hikvision.com/ver20/XMLSchema")]
    public string ResponseStatusStrg { get; set; }

    [XmlElement(ElementName = "numOfMatches", Namespace = "http://www.hikvision.com/ver20/XMLSchema")]
    public int NumOfMatches { get; set; }

    [XmlElement(ElementName = "matchList", Namespace = "http://www.hikvision.com/ver20/XMLSchema")]
    public MatchList MatchList { get; set; }

    [XmlAttribute(AttributeName = "version", Namespace = "")]
    public double Version { get; set; }

    [XmlAttribute(AttributeName = "xmlns", Namespace = "")]
    public string Xmlns { get; set; }

    [XmlText]
    public string Text { get; set; }
}
