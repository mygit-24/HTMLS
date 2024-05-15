
using HTML_serializer;
using System;
using System.Text.RegularExpressions;


var html = await loadHTML("https://learn.malkabruk.co.il/practicode/projects/pract-2/");

var cleanHtml = new Regex("\\s+").Replace(html, " ");
cleanHtml = new Regex("/\\*\\*(.*?)\\*\\*/").Replace(cleanHtml, "");
var htmlLines = new Regex("<(.*?)>").Split(cleanHtml).Where(l => (l.Length > 0 && l != " "));

HtmlElement tree = createTreeElements(htmlLines);

Selector selector1 = Selector.convertStringToSelector("div ul .md-tabs__item");
Console.ReadLine();


List<HtmlElement> descendants = tree.Descendants().ToList();
Console.ReadLine();

List<HtmlElement> ancestors = tree.Children.ToList()[0].Children.ToList()[0].Ancestors().ToList();
Console.ReadLine();


HashSet<HtmlElement> result1 =  HtmlElementExtention.findElementBySelector(tree, selector1);
Console.ReadLine();
foreach (HtmlElement element in result1) { Console.WriteLine(element); }
Console.WriteLine("-------------------------------------");



async Task<string> loadHTML(string url)
{
    HttpClient client = new();
    var res = await client.GetAsync(url);
    var html = await res.Content.ReadAsStringAsync();
    return html;
}


HtmlElement createTreeElements(IEnumerable<string> htmlLines)
{
    var helper = HtmlHelper.Instance;
    HtmlElement root = new();
    HtmlElement current = root;

    foreach (var htmlLine in htmlLines)
    {
        if (htmlLine == "/html")
            return root;

        if (htmlLine.StartsWith("/"))
        {
            current = current.Parent;
            continue;
        }

        string tagName = htmlLine.Split(" ")[0];
        if (helper.HtmlTags.Contains(tagName))
        {
            var newTag = createHtmlElement(current, htmlLine);
            current = newTag;

            if (helper.HtmlVoidTags.Contains(tagName) || tagName.EndsWith("/"))
            {
                current = current.Parent;
            }
        }

        else
        {
            //innerHTML
            current.InnerHTML = htmlLine;
        }

    }
    return root;
}


HtmlElement createHtmlElement(HtmlElement current, string tag)
{
    var newTag = new HtmlElement() 
    { 
        Parent = current, 
        Name = tag.Split(" ")[0],
        Children = new HashSet<HtmlElement>()
    };

    current.Children?.Add(newTag);
   
    var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(tag).ToList();
    foreach (var attribute in attributes)
    {
        string attr = attribute.Groups[1].Value;
        string value = attribute.Groups[2].Value;

        if (attr=="id")
            newTag.Id =value;

        else if (attr == "class")
        {
            newTag.Classes = value.Split(" ").ToList();
        }

        else
            newTag.Attributes.Add(attr, value);
    }

    return newTag;
}