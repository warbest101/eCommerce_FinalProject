using System.Threading.Tasks;

namespace WebBanHang.VnPay
{
    public interface IUtils
    {
        string Md5(string sInput);
        string Sha256(string data);
        string GetIpAddress();
    }
}