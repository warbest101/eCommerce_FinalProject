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

namespace WebBanHang.Controllers
{
    [Route("cart")]
    public class CartController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly MyDBContext _context;
        
        public CartController(
            MyDBContext context, 
            UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
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
        public int Insert(Oder oder)
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
        public async Task<IActionResult> ThanhToan(string shipName, string address)
        {
            var model = _context.loais.ToList();
            ViewBag.model = model;

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if(user.PhoneNumber == null)
            {
                ViewBag.NoPhoneNumber = "You dont have Phone Number. Pleave add your Phone Number in Manage Account.";
                return View();
            }

            var oder = new Oder();
            oder.CreatedDate = DateTime.Now;
            oder.CustomerID = user.Id;
            oder.ShipName = shipName;
            oder.ShipAddress = address;
            oder.ShipMobile = user.PhoneNumber;
            oder.ShipEmail = user.Email;

            try
            {
                var id = Insert(oder);
                var cart = SessionHelper.Get<List<Item>>(HttpContext.Session, "cart");
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
            }
            SessionHelper.Set(HttpContext.Session, "cart", "");
            return View("HoanThanh",ViewBag.model);

        }
        public async Task< IActionResult> xemdonhang()
        {
            return View(await _context.loais.ToListAsync());
        }

    }
}