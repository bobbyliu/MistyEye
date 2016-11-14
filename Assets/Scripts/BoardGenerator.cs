using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;

public class CardInfo
{
    [XmlAttribute("image_name")]
    public string image_name_;
    [XmlAttribute("group_id")]
    public int group_index_;
}

public class GroupInfo
{
    [XmlArray("Items"), XmlArrayItem("Item")]
    public List<int> index_list_;
    [XmlAttribute("ordered")]
    public bool in_order_; // Ignored for now.

    public GroupInfo() { }

    public GroupInfo(List<int> index_list)
    {
        index_list_ = index_list;
    }

    public enum MatchStatus
    {
        NOT_MATCH,
        PARTIAL_MATCH,
        FULL_MATCH
    }

    // Assume income without the last element is a partial match. 
    public MatchStatus IncrementalMatch(List<int> income)
    {
        int last_element = income[income.Count - 1];
        bool is_partial_match;
        if (in_order_)
        {
            is_partial_match = (index_list_[income.Count - 1] == last_element);
        }
        else
        {
            int count = 0;
            for (int i = 0; i < index_list_.Count; i++)
            {
                if (index_list_[i] == last_element)
                {
                    count++;
                }
            }

            for (int i = 0; i < income.Count; i++)
            {
                if (income[i] == last_element)
                {
                    count--;
                }
            }
            is_partial_match = (count >= 0);
        }

        if (!is_partial_match)
        {
            return MatchStatus.NOT_MATCH;
        }
        else if (income.Count < index_list_.Count)
        {
            return MatchStatus.PARTIAL_MATCH;
        }
        else
        {
            return MatchStatus.FULL_MATCH;
        }
    }
}

[XmlRoot("Board")]
public class Board
{
    [XmlElement("Row")]
    public int Row;
    public int Column;

    [XmlArray("Cards"), XmlArrayItem("Card")]
    public List<CardInfo> cards_info_;

    [XmlArray("Groups"), XmlArrayItem("Group")]
    public List<GroupInfo> groups_info_;

    [XmlElement("TimeLimit")]
    public float TimeLimit;

    [XmlElement("Formula")]
    public string formula_image_name_;

    [XmlElement("BackgroundPrefix")]
    public string background_image_prefix_;

    public enum Mode
    {
        NORMAL,
        ENDLESS1,
        ENDLESS2
    }

    [XmlElement("Mode")]
    public Mode PlayMode = Mode.NORMAL;

    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(Board));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }

   public static Board Load(TextAsset asset)
    {
        var serializer = new XmlSerializer(typeof(Board));
        using (var reader = new System.IO.StringReader(asset.text))
        {
            return serializer.Deserialize(reader) as Board;
        }
    }
}

public class BoardGenerator : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
