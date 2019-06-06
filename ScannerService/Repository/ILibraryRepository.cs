using ScannerService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScannerService.Repository
{
    public interface ILibraryRepository : IDisposable
    {
        void AddBook(Book newBook);
        int CountBooks();
    }
}
