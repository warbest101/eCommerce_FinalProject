using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.Models;
namespace WebBanHang.DAO
{
    public class OderDao
    {
        MyDBContext _context;

        public OderDao(MyDBContext context)
        {
            _context = context;
        }
        public int Insert(Oder oder)
        {
            _context.Oders.Add(oder);
            _context.SaveChanges();
            return oder.ID;
        }
    }
}
