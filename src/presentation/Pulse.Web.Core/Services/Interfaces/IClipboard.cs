using System.Threading.Tasks;

namespace Pulse.Web.Core.Services.Interfaces;

public interface IClipboard
{
    Task<bool> CopyTo(string value, bool withMessage = false);
}