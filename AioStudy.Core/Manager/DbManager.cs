using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using AioStudy.Data.EF;

namespace AioStudy.Core.Manager
{
    public class DbManager
    {
        public DbManager()
        {
            
        }

        public static bool InitializeDatabase()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    db.Database.EnsureCreated();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
