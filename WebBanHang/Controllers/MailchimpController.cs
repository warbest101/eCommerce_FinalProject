using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailChimp.Net;
using MailChimp.Net.Core;
using MailChimp.Net.Interfaces;
using MailChimp.Net.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebBanHang.Models;
using WebBanHang.ViewModels;

namespace WebBanHang.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MailchimpController : Controller
    {
        private readonly MyDBContext _context;
        private readonly string admin = "Admin";
        private readonly string _apiKey;
        private readonly string _listId;

        public MailchimpController(MyDBContext context, IConfiguration config)
        {
            _context = context;
            _apiKey = config["MailchimpSettings:ApiKey"];
            _listId = config["MailchimpSettings:ListId"];
        }

        public async Task<IActionResult> Index()
        {

            if (User.Identity.Name != admin)
            {
                return RedirectToAction("Index", "TrangChus");
            }
            IMailChimpManager mailChimpManager = new MailChimpManager(_apiKey);
            var members = await mailChimpManager.Members.GetAllAsync(_listId).ConfigureAwait(false);
            ViewBag.model = members;

            return View(ViewBag.model);
        }

        public IActionResult Create()
        {
            if (User.Identity.Name != admin)
            {
                return RedirectToAction("Index", "TrangChus");
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Member model, 
            string fname = "", 
            string lname = "",
            string addr1 = "",
            string addr2 = "",
            string city = "",
            string state = "",
            string zip = "",
            string country = "US",
            string phone = "",
            string birth_month = "",
            string birth_date = "")
        {
            var checkErr = false;
            
            
            IMailChimpManager mailChimpManager = new MailChimpManager(_apiKey);

            var member = new Member
            {
                EmailAddress = model.EmailAddress,
            };

            var members = await mailChimpManager.Members.GetAllAsync(_listId).ConfigureAwait(false);
            var check = members.FirstOrDefault(x => x.EmailAddress == member.EmailAddress);
            if(!string.IsNullOrEmpty(fname)) member.MergeFields.Add("FNAME", fname);
            if(!string.IsNullOrEmpty(lname)) member.MergeFields.Add("LNAME", lname);
            if(!string.IsNullOrEmpty(addr1) 
                && !string.IsNullOrEmpty(addr2)
                && !string.IsNullOrEmpty(city)
                && !string.IsNullOrEmpty(state)
                && !string.IsNullOrEmpty(zip)
                && !string.IsNullOrEmpty(country))
            {
                var address = new Dictionary<string, object>
                {
                    {"addr1", addr1 },
                    {"addr2", addr2 },
                    {"city", city },
                    {"state", state },
                    {"zip", zip },
                    {"country", country }
                };

                member.MergeFields.Add("ADDRESS", address);
            }
            if (!string.IsNullOrEmpty(phone))
            {
                var isNumeric = int.TryParse(phone, out _);
                if (!isNumeric)
                {
                    checkErr = true;
                    ModelState.AddModelError("", "Phone number must be numeric");
                }
                member.MergeFields.Add("PHONE", phone);
            }
            if (!string.IsNullOrEmpty(birth_month) && !string.IsNullOrEmpty(birth_month))
            {
                var numericMonth = int.TryParse(birth_month, out _);
                var numericDate = int.TryParse(birth_date, out _);
                if (!numericDate || !numericMonth)
                {
                    checkErr = true;
                    ModelState.AddModelError("", "Date and Month must be numeric");
                }
                var birthday = birth_month + "/" + birth_date;
                member.MergeFields.Add("BIRTHDAY", birthday);
            }
            
            if (checkErr == false)
            {
                if (ModelState.IsValid)
                {
                    if (check == null)
                    {
                        member.Status = Status.Pending;
                    }
                    else
                    {
                        member.Status = check.Status;
                    }
                    try
                    {
                        await mailChimpManager.Members.AddOrUpdateAsync(_listId, member);
                        return RedirectToAction(nameof(Index));
                    }
                    catch (MailChimpException mce)
                    {
                        ModelState.AddModelError("", mce.Message);
                    }

                }
            }
            
            return View(model);
            
        }

        public async Task<IActionResult> Details(string id)
        {
            if(id == null)
            {
                return NotFound();
            }
            IMailChimpManager mailChimpManager = new MailChimpManager(_apiKey);

            var members = await mailChimpManager.Members.GetAllAsync(_listId).ConfigureAwait(false);
            var member = members.First(x => x.EmailAddress == id);

            if(member == null)
            {
                return NotFound();
            }
            return View(member);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            IMailChimpManager mailChimpManager = new MailChimpManager(_apiKey);

            var members = await mailChimpManager.Members.GetAllAsync(_listId).ConfigureAwait(false);
            var member = members.First(x => x.EmailAddress == id);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            IMailChimpManager mailChimpManager = new MailChimpManager(_apiKey);
            var members = await mailChimpManager.Members.GetAllAsync(_listId).ConfigureAwait(false);
            var member = members.First(x => x.EmailAddress == id);
            if (member == null)
            {
                return NotFound();
            }
            if(member.Status == Status.Pending)
            {
                ModelState.AddModelError("", "You cannot delete this member. This member's email is pending");
            }
            else
            {
                try
                {
                    await mailChimpManager.Members.DeleteAsync(_listId, member.EmailAddress);
                    return RedirectToAction(nameof(Index));
                }
                catch (MailChimpException mce)
                {
                    ModelState.AddModelError("", mce.Message);
                }
            }

            return View(member);
        }

        public async Task<IActionResult> ExportAllMember()
        {
            IMailChimpManager mailChimpManager = new MailChimpManager(_apiKey);
            var members = await mailChimpManager.Members.GetAllAsync(_listId).ConfigureAwait(false);
            var csvName = "allmember_" + DateTime.UtcNow.Ticks + ".csv";
            var builder = new StringBuilder();
            builder.AppendLine("Email Address,First Name, Last Name");
            foreach(var item in members)
            {
                builder.AppendLine($"{item.EmailAddress},{item.MergeFields["FNAME"].ToString()},{item.MergeFields["LNAME"].ToString()}");
            }
            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", csvName);
        }
        public async Task<IActionResult> ExportMember(string id)
        {
            IMailChimpManager mailChimpManager = new MailChimpManager(_apiKey);
            var members = await mailChimpManager.Members.GetAllAsync(_listId).ConfigureAwait(false);
            var member = members.FirstOrDefault(p => p.EmailAddress == id);
            var csvName = "member_" + DateTime.UtcNow.Ticks + ".csv";
            var builder = new StringBuilder();
            builder.AppendLine("Email Address,First Name, Last Name");
            builder.AppendLine($"{member.EmailAddress},{member.MergeFields["FNAME"].ToString()},{member.MergeFields["LNAME"].ToString()}");
            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", csvName);
        }
    }
}