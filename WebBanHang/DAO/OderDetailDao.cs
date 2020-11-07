using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanHang.Models;
namespace WebBanHang.DAO
{
    public class OderDetailDao
    {
        private readonly MyDBContext _context;

        public OderDetailDao(MyDBContext context)
        {
            _context = context;
        }
        public bool Insert1(OderDetail detail)
        {
            try {
                _context.OderDetails.Add(detail);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
            
        }
    }
}
