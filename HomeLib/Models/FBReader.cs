using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using HomeLib.Infrostructure;

namespace HomeLib.Models
{
    public static class FBReader
    {
        public static Book ReadBook(Stream inputStream)//, string pathArchive,string fileName)
        {

            Book book = new Book();
            XPathDocument doc = new XPathDocument(inputStream);
            XPathNavigator navigator = doc.CreateNavigator();
            XmlNamespaceManager manager = new XmlNamespaceManager(navigator.NameTable);
            manager.AddNamespace("ns", "http://www.gribuser.ru/xml/fictionbook/2.0");
            XPathNodeIterator iterator = navigator.Select("//ns:title-info", manager);
            XPathNodeIterator descendants;
            bool firstAuthor = true;
            while (iterator.MoveNext())
            {
                descendants = iterator.Current.SelectDescendants(XPathNodeType.Element, false);
                while (descendants.MoveNext())
                {
                    if (descendants.Current.Name == "author" && firstAuthor)
                    {
                        XPathNodeIterator authorsDescendants = descendants.Current.SelectDescendants(XPathNodeType.Element, false);
                        while (authorsDescendants.MoveNext())
                        {
                            if (authorsDescendants.Current.Name == "first-name") { string name = authorsDescendants.Current.Value.FormatName(); book.Authtor.FirstName = name; }
                            if (authorsDescendants.Current.Name == "middle-name") { string name = authorsDescendants.Current.Value.FormatName(); book.Authtor.MiddleName = name; }
                            if (authorsDescendants.Current.Name == "last-name") { string name = authorsDescendants.Current.Value.FormatName(); book.Authtor.LastName = name; }
                        }
                        firstAuthor = false;
                    }
                    if (descendants.Current.Name == "book-title") { book.Title = descendants.Current.Value; }
                }
            }

            iterator = navigator.Select("//ns:isbn", manager);

            while (iterator.MoveNext())
            {
                descendants = iterator.Current.SelectDescendants(XPathNodeType.Element, false);
                while (descendants.MoveNext())
                {
                    if (descendants.Current.Name == "isbn")
                    {
                        book.Isbn = descendants.Current.Value;
                    }
                }
            }
            //book.PathArchive = GetArchiveName();
            //book.PathBook = GetFileName();
            return book;
        }
    }
}
