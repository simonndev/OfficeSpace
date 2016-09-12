using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeSpace.DataAccess;
using OfficeSpace.DataAccess.Models;

namespace OfficeSpace.BusinessLogic
{
    public abstract class ServiceBase
    {
        protected OfficeSpaceContext _dbContext;

        public ServiceBase(OfficeSpaceContext dbContext)
        {
            this._dbContext = dbContext;
        }
    }
}
