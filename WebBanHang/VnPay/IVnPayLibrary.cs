namespace WebBanHang.VnPay
{
    public interface IVnPayLibrary
    {
        void AddRequestData(string key, string value);
        void AddResponseData(string key, string value);
        string GetResponseData(string key);
        string CreateRequestUrl(string baseUrl, string vnp_HashSecret);
        bool ValidateSignature(string inputHash, string secretKey);
    }
}