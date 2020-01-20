using HomeLibServices.ExtensionsMethods;
using HomeLibServices.Models;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace HomeLibServices.FileSystem
{
    /// <summary>
    /// Read file.fb2 and create object Book
    /// </summary>
    internal class FbReader
    {
        /// <summary>
        /// Overload ReadBook accept path to fb2 file and create file stream
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Book ReadBook(string path)
        {
            using (FileStream fs = File.OpenRead(path))
            {
                return ReadBook(fs);
            }
        }
        /// <summary>
        /// Create obj Book, read xml doc and write Book
        /// </summary>
        /// <param name="streamEntity">Readable file</param>
        /// <returns></returns>
        public Book ReadBook(Stream streamEntity)
        {
            Book newBook = new Book();
            XPathDocument doc = new XPathDocument(streamEntity);

            XPathNavigator navigator = doc.CreateNavigator();
            XmlNamespaceManager manager = new XmlNamespaceManager(navigator.NameTable);
            manager.AddNamespace("ns", "http://www.gribuser.ru/xml/fictionbook/2.0");
            GetAuthors(navigator.Select("//ns:title-info/ns:author", manager), newBook);
            GetTitle(navigator.SelectSingleNode("//ns:title-info/ns:book-title", manager)?.SelectDescendants(XPathNodeType.Element, false), newBook);
            GetAnnotation(navigator.SelectSingleNode("//ns:title-info/ns:annotation", manager)?.SelectDescendants(XPathNodeType.Element, false), newBook);
            GetYear(navigator.SelectSingleNode("//ns:publish-info/ns:year", manager)?.SelectDescendants(XPathNodeType.Element, false), newBook);
            GetISBN(navigator.SelectSingleNode("//ns:publish-info/ns:isbn", manager)?.SelectDescendants(XPathNodeType.Element, false), newBook);
            GetCover(navigator.SelectSingleNode("//ns:binary[@id='cover.jpg']", manager)?.SelectDescendants(XPathNodeType.Element, false), newBook);
            return newBook;
        }

        #region Get Info for Book

        /// <summary>
        /// Pull from file target nodes and writes it in obj Book
        /// </summary>
        /// <param name="iterator"></param>
        /// <param name="book"></param>
        private void GetAuthors(XPathNodeIterator iterator, Book book)
        {
            while (iterator.MoveNext())
            {
                if (iterator?.Current.Name == "author")
                {
                    StringBuilder fullName = new StringBuilder();
                    XPathNodeIterator desc = iterator.Current.SelectChildren(XPathNodeType.Element);
                    desc.MoveNext();
                    var newAuthor = new Author();
                    if (desc.Current.Name == "first-name")
                    {
                        string name = desc.Current.Value.FormatName();
                        if (name != "Unknown")
                        {
                            fullName.Append(name);
                        }
                        newAuthor.FirstName = name;
                        desc.MoveNext();
                    }
                    if (desc.Current.Name == "middle-name")
                    {
                        string name = desc.Current.Value.FormatName();
                        if (name != "Unknown")
                        {
                            fullName.Append(" " + name);
                        }
                        newAuthor.MiddleName = name;
                        desc.MoveNext();
                    }

                    if (desc.Current.Name == "last-name")
                    {
                        string name = desc.Current.Value.FormatName();
                        if (name != "Unknown")
                        {
                            fullName.Insert(0, name + " ");
                        }
                        newAuthor.LastName = name;
                    }
                    newAuthor.FullName = fullName.ToString();
                    book.Authorships.Add(new Authorship { Author = newAuthor });
                }
            }

            if (book.Authorships.Count == 0)
            {
                book.Authorships.Add(new Authorship() { Author = new Author() { FullName = "Unknown" } });
            }
        }

        private void GetTitle(XPathNodeIterator iterator, Book book)
        {
            if (iterator?.Current.Name == "book-title")
            {
                book.Title = iterator.Current.Value;
            }
        }

        private void GetAnnotation(XPathNodeIterator iterator, Book book)
        {
            if (iterator?.Current.Name == "annotation")
            {
                book.Annotation = iterator.Current.Value;
            }
        }
        private void GetYear(XPathNodeIterator iterator, Book book)
        {
            if (iterator?.Current.Name == "year")
            {
                book.Year = iterator.Current.Value;
            }
        }

        private void GetISBN(XPathNodeIterator iterator, Book book)
        {
            if (iterator?.Current.Name == "isbn")
            {
                book.Isbn = iterator.Current.Value.Replace("-", "");
            }
        }

        private void GetCover(XPathNodeIterator iterator, Book book)
        {
            if (iterator?.Current.Name == "binary")
            {
                book.Cover = iterator.Current.Value;
            }
        }

        #endregion
    }
}

