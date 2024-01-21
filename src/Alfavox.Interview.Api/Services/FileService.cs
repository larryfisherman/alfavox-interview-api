using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alfavox.Interview.Api.Services
{
    public interface IFileService
    {
        void WriteAllText(string path, string contents);
    }
    public class FileService : IFileService
    {
        public void WriteAllText(string path, string contents)
        {
            File.WriteAllText(path, contents);
        }
    }
}


