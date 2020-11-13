using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Models;
using WebBanHang.DAO;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PayPal.Core;
using Microsoft.Extensions.Configuration;
using PayPal.v1.Payments;
using Item = WebBanHang.Models.Item;
using item = PayPal.v1.Payments.Item;
using BraintreeHttp;
using WebBanHang.VnPay;
using WebBanHang.ViewModels;

namespace WebBanHang.Controllers
{
    [Route("cart")]
    public class CartController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly MyDBContext _context;

        //Order Id
        private long _orderId = 0;

        //Paypal
        private readonly string _clientId;
        private readonly string _secretKey;

        //VNPay
        private readonly string _url;
        private readonly string _tmnCode;
        private readonly string _hashSecret;
        private readonly IUtils _utils;
        private readonly IVnPayLibrary _vnPayLibrary;

        public double tyGiaUSD = 23300;
        public CartController(
            MyDBContext context, 
            UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager,
            IUtils utils,
            IVnPayLibrary vnPayLibrary,
            IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;

            //Paypal
            _clientId = config["PaypalSettings:ClientId"];
            _secretKey = config["PaypalSettings:SecretKey"];

            //VNPay
            _url = config["VnPaySettings:Url"];
            _tmnCode = config["VnPaySettings:TmnCode"];
            _hashSecret = config["VnPaySettings:HashSecret"];
            _utils = utils;
            _vnPayLibrary = vnPayLibrary;
        }
        [Route("showsp")]
        public async Task<IActionResult> Showsp(int? id)
        {
            var model = _context.loais.ToList();
            ViewBag.model = model;

            var loai = _context.HangHoas
                .Where(m => m.MaLoai == id).AsNoTracking().OrderBy(p => p.TenHH);

            return View(loai);
        }
        [Route("index")]
        public async Task<IActionResult> Index()
        {
           
            var model = _context.loais.ToList();
            ViewBag.model = model;
            var cart = SessionHelper.Get<List<Item>>(HttpContext.Session, "cart");
            if(cart == null)
            {
                return View("View");
            }
            else
            ViewBag.cart = cart;
            ViewBag.gia = cart.Sum(item => (item.Product.DonGia * item.Quantity));
            ViewBag.giamgia = cart.Sum(x => (x.Product.GiamGia*x.Product.DonGia)/100);
            ViewBag.total = cart.Sum(item => (item.Product.DonGia * item.Quantity)-((item.Product.GiamGia)*(item.Product.DonGia))/100);
            ViewBag.quantity = cart.Sum(item => item.Quantity);
            return View();
        }

        [Route("buy/{id}")]
        public IActionResult Buy(int id,int soluong)
        {
            
            if (SessionHelper.Get<List<Item>>(HttpContext.Session, "cart") == null)
            {
                List<Item> cart = new List<Item>();
                cart.Add(new Item { Product = _context.HangHoas.Find(id), Quantity = 1 });
                
                SessionHelper.Set(HttpContext.Session, "cart", cart);
            }
            else
            {
                List<Item> cart = SessionHelper.Get<List<Item>>(HttpContext.Session, "cart");
                int index = Exists(id);
                if (index != -1)
                {
                    cart[index].Quantity++;
                }
                else
                {
                    cart.Add(new Item { Product = _context.HangHoas.Find(id), Quantity = 1 });
                }
                SessionHelper.Set(HttpContext.Session, "cart", cart);
            }

            return RedirectToAction("Index");
        }

        [Route("buydetail/{id}")]
        public IActionResult BuyDetail(int id, int soluong)
        {

            if (SessionHelper.Get<List<Item>>(HttpContext.Session, "cart") == null)
            {
                List<Item> cart = new List<Item>();
                cart.Add(new Item { Product = _context.HangHoas.Find(id), Quantity = soluong });

                SessionHelper.Set(HttpContext.Session, "cart", cart);
            }
            else
            {
                List<Item> cart = SessionHelper.Get<List<Item>>(HttpContext.Session, "cart");
                int index = Exists(id);
                if (index != -1)
                {
                    cart[index].Quantity = cart[index].Quantity + soluong;
                }
                else
                {
                    cart.Add(new Item { Product = _context.HangHoas.Find(id), Quantity = soluong });
                }
                SessionHelper.Set(HttpContext.Session, "cart", cart);
            }

            return RedirectToAction("Index");
        }

        [Route("remove/{id}")]
        public IActionResult Remove(int id)
        {
            List<Item> cart = SessionHelper.Get<List<Item>>(HttpContext.Session, "cart");
            int index = Exists(id);
            cart.RemoveAt(index);
            SessionHelper.Set(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index");
        }

        private int Exists(int id)
        {
            List<Item> cart = SessionHelper.Get<List<Item>>(HttpContext.Session, "cart");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Product.MaHH == id)
                {
                    return i;
                }
            }
            return -1;
        }

       
        [HttpPost("a")]
        public IActionResult Update(IFormCollection fc)
        {
            string[] quantites = fc["quantity"];
            var cart = SessionHelper.Get<List<Item>>(HttpContext.Session, "cart");
            for(int i = 0; i < cart.Count; i++)
            {
                cart[i].Quantity = Convert.ToInt32(quantites[i]);
            }
            SessionHelper.Set(HttpContext.Session, "cart", cart);

            return RedirectToAction("Index");
        }
        public IActionResult DeleteAll()
        {
            SessionHelper.Set(HttpContext.Session, "cart", "");
            return RedirectToAction("Index");
        }
        public long Insert(Oder oder)
        {
            _context.Oders.Add(oder);
            _context.SaveChanges();
            return oder.ID;
        }
        public bool Insert1(OderDetail detail)
        {
            try
            {
                _context.OderDetails.Add(detail);
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.Write(e);
                return false;
            }

        }
        public bool Insert2(int id,HangHoa hangHoa)
        {
            try
            {
             var hh=   _context.HangHoas.Where(x => x.MaHH == id);
               
                return true;
            }
            catch (Exception e)
            {
                Console.Write(e);
                return false;
            }

        }
        [HttpGet, Authorize]
        [Route("thanh-toan")]
        public async Task<IActionResult> ThanhToan()
        {
            var model = _context.loais.ToList();
            ViewBag.model = model;
            var cart = SessionHelper.Get<List<Item>>(HttpContext.Session, "cart");
            if (cart == null)
            {
                return View("View");
            }
            else
            {
                ViewBag.cart = cart;
            }
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.PhoneNumber = user.PhoneNumber;
            ViewBag.total = cart.Sum(item => item.Product.DonGia * item.Quantity);
            return View();
        }

        [HttpPost, Authorize]
        [Route("thanh-toan")]
        public async Task<IActionResult> ThanhToan(string shipName, string address)
        {
            var model = _context.loais.ToList();
            ViewBag.model = model;

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if(user.PhoneNumber == null)
            {
                ViewBag.NoPhoneNumber = "You dont have Phone Number. Pleave add your Phone Number in Manage Account.";
                //return View();
            }

            var cart = SessionHelper.Get<List<Item>>(HttpContext.Session, "cart");

            var oder = new Oder();
            oder.ID = DateTime.Now.Ticks;
            oder.CreatedDate = DateTime.Now;
            oder.CustomerID = user.Id;
            oder.ShipName = shipName;
            oder.ShipAddress = address;
            oder.ShipMobile = user.PhoneNumber;
            oder.ShipEmail = user.Email;
            oder.CheckOutType = "Normal";

            var subTotal = cart.Sum( item => (item.Product.DonGia * item.Quantity) );
            var giamGia = cart.Sum( item => (item.Product.GiamGia * item.Product.DonGia * item.Quantity) / 100);

            oder.Total = Math.Round(subTotal - giamGia, 0);
            _orderId = oder.ID;


            try
            {
                var id = Insert(oder);
                foreach (var item in cart)
                {
                    var oderDetail = new OderDetail();
                    oderDetail.MaHH = item.Product.MaHH;
                    oderDetail.OderID = id;
                    oderDetail.Gia = item.Product.DonGia;
                    oderDetail.Quantity = item.Quantity;
                    Insert1(oderDetail);

                    var hanghoas = _context.HangHoas.Where(x => x.MaHH == item.Product.MaHH).First();
                    
                   
                        hanghoas.DaMua += item.Quantity;
                        _context.Update(hanghoas);
                        _context.SaveChanges();
                   
                }
            }
            catch(Exception e)
            {
                Console.Write(e);
                return View("ThatBai");
            }

            oder.Status = true;
            _context.Update(oder);
            _context.SaveChanges();

            return View("HoanThanh");

        }

        [Authorize]
        [Route("thanh-toan-paypal")]
        public async Task<IActionResult> ThanhToanPaypal()
        {
            var model = _context.loais.ToList();
            ViewBag.model = model;

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var environment = new SandboxEnvironment(_clientId, _secretKey);
            var client = new PayPalHttpClient(environment);

            #region Create Paypal Order
            var cart = SessionHelper.Get<List<Item>>(HttpContext.Session, "cart");
            var itemList = new ItemList()
            {
                Items = new List<item>()
            };
            //var total = Math.Round(cart.Sum(item => (item.Product.DonGia * item.Quantity) - ((item.Product.GiamGia) * (item.Product.DonGia)) / 100) / tyGiaUSD, 2);
            var total = Math.Round(cart.Sum((item => (item.Product.DonGia * item.Quantity))) / tyGiaUSD, 2);
            foreach (var item in cart)
            {
                itemList.Items.Add(new item()
                {
                    Name = item.Product.TenHH,
                    Currency = "USD",
                    Price = Math.Round(item.Product.DonGia / tyGiaUSD, 2).ToString(),
                    Quantity = item.Quantity.ToString(),
                    Sku = "sku",
                    Tax = "0"
                });
            }
            #endregion


            var paypalOrderId = DateTime.Now.Ticks;
            var hostname = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var payment = new Payment()
            {
                Intent = "sale",
                Transactions = new List<Transaction>()
                {
                    new Transaction()
                    {
                        Amount = new Amount()
                        {
                            Total = total.ToString(),
                            Currency = "USD",
                            Details = new AmountDetails
                            {
                                Tax = "0",
                                Shipping = "0",
                                Subtotal = total.ToString()
                            }
                        },
                        ItemList = itemList,
                        Description = $"Invoice #{paypalOrderId}",
                        InvoiceNumber = paypalOrderId.ToString()
                    }
                },
                RedirectUrls = new RedirectUrls()
                {
                    CancelUrl = $"{hostname}/cart/that-bai",
                    ReturnUrl = $"{hostname}/cart/hoan-thanh"
                },
                Payer = new Payer()
                {
                    PaymentMethod = "paypal"
                }
            };

            #region Insert Order to Database
            var oder = new Oder();
            oder.ID = paypalOrderId;
            oder.ShipAddress = "None";
            oder.ShipName = "Paypal User";
            oder.ShipMobile = "None";
            oder.ShipEmail = user.Email;
            oder.CheckOutType = "Paypal";
            oder.CustomerID = user.Id;
            oder.CreatedDate = DateTime.Now;

            var subTotal = cart.Sum(item => (item.Product.DonGia * item.Quantity));
            var giamGia = cart.Sum(item => (item.Product.GiamGia * item.Product.DonGia * item.Quantity) / 100);

            oder.Total = Math.Round(subTotal - giamGia, 0);

            _orderId = oder.ID;
            SessionHelper.Set(HttpContext.Session, "orderId", _orderId);

            try
            {
                var id = Insert(oder);
                foreach (var item in cart)
                {
                    var oderDetail = new OderDetail();
                    oderDetail.MaHH = item.Product.MaHH;
                    oderDetail.OderID = id;
                    oderDetail.Gia = item.Product.DonGia;
                    oderDetail.Quantity = item.Quantity;
                    Insert1(oderDetail);

                    var hanghoas = _context.HangHoas.Where(x => x.MaHH == item.Product.MaHH).First();

                    hanghoas.DaMua += item.Quantity;
                    _context.Update(hanghoas);
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.Write(e);
                return View("ThatBai");
            }
            #endregion

            PaymentCreateRequest request = new PaymentCreateRequest();
            request.RequestBody(payment);

            try
            {
                var response = await client.Execute(request);
                var statusCode = response.StatusCode;
                Payment result = response.Result<Payment>();

                var links = result.Links.GetEnumerator();
                string paypalRedirectUrl = null;
                while (links.MoveNext())
                {
                    LinkDescriptionObject lnk = links.Current;
                    if (lnk.Rel.ToLower().Trim().Equals("approval_url"))
                    {
                        paypalRedirectUrl = lnk.Href;
                    }
                }

                return Redirect(paypalRedirectUrl);
            }
            catch (HttpException httpException)
            {
                var statusCode = httpException.StatusCode;
                var debugId = httpException.Headers.GetValues("PayPal-Debug-Id").FirstOrDefault();

                return Redirect("/cart/that-bai");
            }
        }

        [Authorize]
        [Route("thanh-toan-vnpay")]
        public async Task<IActionResult> ThanhToanVnPay()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var hostname = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

            var oder = new Oder();
            oder.ID = DateTime.Now.Ticks;
            oder.ShipName = "VNPay User";
            oder.ShipMobile = "None";
            oder.ShipAddress = "None";
            oder.ShipEmail = user.Email;
            oder.CustomerID = user.Id;
            oder.CreatedDate = DateTime.Now;
            oder.CheckOutType = "VNPay";

            var cart = SessionHelper.Get<List<Item>>(HttpContext.Session, "cart");
            var subTotal = cart.Sum(item => (item.Product.DonGia * item.Quantity));
            var giamGia = cart.Sum(item => (item.Product.GiamGia * item.Product.DonGia * item.Quantity) / 100);

            oder.Total = Math.Round(subTotal - giamGia, 0);

            try
            {
                var id = Insert(oder);
                foreach (var item in cart)
                {
                    var oderDetail = new OderDetail();
                    oderDetail.MaHH = item.Product.MaHH;
                    oderDetail.OderID = id;
                    oderDetail.Gia = item.Product.DonGia;
                    oderDetail.Quantity = item.Quantity;
                    Insert1(oderDetail);

                    var hanghoas = _context.HangHoas.Where(x => x.MaHH == item.Product.MaHH).First();

                    hanghoas.DaMua += item.Quantity;
                    _context.Update(hanghoas);
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.Write(e);
                return View("ThatBai");
            }

            _vnPayLibrary.AddRequestData("vnp_Version", "2.0.0");
            _vnPayLibrary.AddRequestData("vnp_Command", "pay");
            _vnPayLibrary.AddRequestData("vnp_TmnCode", _tmnCode);
            _vnPayLibrary.AddRequestData("vnp_Amount", (oder.Total * 100).ToString());
            _vnPayLibrary.AddRequestData("vnp_BankCode", "NCB");
            _vnPayLibrary.AddRequestData("vnp_CreateDate", oder.CreatedDate.ToString("yyyyMMddHHmmss"));
            _vnPayLibrary.AddRequestData("vnp_CurrCode", "VND");
            _vnPayLibrary.AddRequestData("vnp_IpAddr", _utils.GetIpAddress());
            _vnPayLibrary.AddRequestData("vnp_Locale", "vn");
            _vnPayLibrary.AddRequestData("vnp_OrderInfo", "Noi dung thanh toan:" + DateTime.Now.ToString("yyyyMMddHHmmss"));
            _vnPayLibrary.AddRequestData("vnp_OrderType", "130001"); //default value: other
            _vnPayLibrary.AddRequestData("vnp_ReturnUrl", $"{hostname}/cart/ket-qua-vnpay");
            _vnPayLibrary.AddRequestData("vnp_TxnRef", oder.ID.ToString());

            string paymentUrl = _vnPayLibrary.CreateRequestUrl(_url, _hashSecret);

            return Redirect(paymentUrl);
        }

        //VN Pay
        [HttpGet]
        [Route("ket-qua-vnpay")]
        public IActionResult KetQuaVnPay()
        {
            var model = _context.loais.ToList();
            ViewBag.model = model;
            if(Request.QueryString != null)
            {
                string vnp_Amount = Request.Query["vnp_Amount"];
                string vnp_BankCode = Request.Query["vnp_BankCode"];
                string vnp_BankTranNo = Request.Query["vnp_BankTranNo"];
                string vnp_CardType = Request.Query["vnp_CardType"];
                string vnp_OrderInfo = Request.Query["vnp_OrderInfo"];
                string vnp_PayDate = Request.Query["vnp_PayDate"];
                string vnp_ResponseCode = Request.Query["vnp_ResponseCode"];
                string vnp_TmnCode = Request.Query["vnp_TmnCode"];
                string vnp_TransactionNo = Request.Query["vnp_TransactionNo"];
                string vnp_TxnRef = Request.Query["vnp_TxnRef"];
                string vnp_SecureHashType = Request.Query["vnp_SecureHashType"];
                string vnp_SecureHash = Request.Query["vnp_SecureHash"];

                _vnPayLibrary.AddResponseData("vnp_Amount", vnp_Amount);
                _vnPayLibrary.AddResponseData("vnp_BankCode", vnp_BankCode);
                _vnPayLibrary.AddResponseData("vnp_BankTranNo", vnp_BankTranNo);
                _vnPayLibrary.AddResponseData("vnp_CardType", vnp_CardType);
                _vnPayLibrary.AddResponseData("vnp_OrderInfo", vnp_OrderInfo);
                _vnPayLibrary.AddResponseData("vnp_PayDate", vnp_PayDate);
                _vnPayLibrary.AddResponseData("vnp_ResponseCode", vnp_ResponseCode);
                _vnPayLibrary.AddResponseData("vnp_TmnCode", vnp_TmnCode);
                _vnPayLibrary.AddResponseData("vnp_TransactionNo", vnp_TransactionNo);
                _vnPayLibrary.AddResponseData("vnp_TxnRef", vnp_TxnRef);
                _vnPayLibrary.AddResponseData("vnp_SecureHashType", vnp_SecureHashType);
                _vnPayLibrary.AddResponseData("vnp_SecureHash", vnp_SecureHash);

                long orderId = Convert.ToInt64(_vnPayLibrary.GetResponseData("vnp_TxnRef"));
                bool checkSignature = _vnPayLibrary.ValidateSignature(vnp_SecureHash, _hashSecret);
                
                if (checkSignature)
                {
                    if(vnp_ResponseCode == "00")
                    {
                        ViewBag.KetQua = "Thanh toán thành công";
                        var oder = _context.Oders.SingleOrDefault(m => m.ID == orderId);
                        oder.Status = true;
                        _context.Update(oder);
                        _context.SaveChanges();

                    }
                    else
                    {
                        ViewBag.KetQua = "Thanh toán không thành công. Có lỗi xảy ra trong quá trình xử lý.";
                    }
                }
                else
                {
                    ViewBag.KetQua = "Thanh toán không thành công. Có lỗi xảy ra trong quá trình xử lý.";
                }
            }
            SessionHelper.Set(HttpContext.Session, "cart", "");
            return View();
        }

        public async Task<IActionResult> xemdonhang()
        {
            return View(await _context.loais.ToListAsync());
        }

        [HttpGet]
        [Route("hoan-thanh")]
        public IActionResult HoanThanh()
        {
            var model = _context.loais.ToList();
            ViewBag.model = model;

            if(_orderId == 0)
            {
                var orderId = SessionHelper.Get<long>(HttpContext.Session, "orderId");
                _orderId = orderId;
            }
            
            SessionHelper.Set(HttpContext.Session, "orderId", 0); 

            var oder = _context.Oders.SingleOrDefault(m => m.ID == _orderId);

            if(oder == null)
            {
                return NotFound();
            }

            oder.Status = true;
            _context.Update(oder);
            _context.SaveChanges();

            SessionHelper.Set(HttpContext.Session, "cart", "");

            return View();
        }

        [HttpGet]
        [Route("that-bai")]
        public IActionResult ThatBai()
        {
            var model = _context.loais.ToList();
            ViewBag.model = model;

            SessionHelper.Set(HttpContext.Session, "cart", "");
            SessionHelper.Set(HttpContext.Session, "orderId", 0);

            return View();
        }


    }
}