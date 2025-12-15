
using Microsoft.AspNetCore.Http;

namespace Benzeny.Domain.IRepository
{
    public interface IMailRepository
    {
        Task SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile> attachments = null);
    }
}
